using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Services;
using FinTrack.API.Infrastructure.Caching.Decorators;
using FinTrack.API.Infrastructure.Caching.DTO;
using FinTrack.API.Infrastructure.Caching.Services;
using FinTrack.API.Infrastructure.Data;
using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Identity.Services;
using FinTrack.API.Infrastructure.Interfaces;
using FinTrack.API.Infrastructure.Messaging;
using FinTrack.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Extensions.Microsoft.DependencyInjection;
using NATS.Net;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Polly;
using Polly.CircuitBreaker;
using Serilog;
using StackExchange.Redis;
using System.Reflection;
using System.Threading.RateLimiting;

namespace FinTrack.API
{
    public class Program
    {
        
        private static string serviceName = "FinTrack-API";
        private static string serviceVersion = "1.1.0";
        private static string environment = "development";


        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            bool isTesting = builder.Environment.IsEnvironment("Testing");

            if (!isTesting)
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .Enrich.FromLogContext()
                    .CreateBootstrapLogger();
            }
            
            
            try
            {
                
                serviceName = builder.Environment.ApplicationName;
                environment = builder.Environment.EnvironmentName;

                var vaultCredsPath = Environment.GetEnvironmentVariable("ROLEID_PATH");
                builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

                if (!String.IsNullOrEmpty(vaultCredsPath)){
                    builder.Configuration.AddJsonFile(vaultCredsPath, optional: true);
                }

                builder.Configuration.AddEnvironmentVariables();

                builder.Host.UseSerilog((ctx, services, lc) =>
                {
                    lc.ReadFrom.Configuration(builder.Configuration).
                        ReadFrom.Services(services);
                }, preserveStaticLogger: isTesting);
                

                ConfigureServices(builder.Services, builder.Configuration);
                var app = builder.Build();
                app.MapPrometheusScrapingEndpoint();
                app.MapHealthChecks("/health");

                if (!isTesting)
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        try
                        {
                            var db = scope.ServiceProvider.GetRequiredService<DatabaseClient>();
                            await db.Database.MigrateAsync();

                            var natsPublisher = scope.ServiceProvider.GetRequiredService<NatsMessagePublisher>();
                            await natsPublisher.InitializeStream();
                        }
                        catch (Exception ex)
                        {
                            
                            Log.Logger.Fatal(ex, "Failed to migrate database");
                            return;
                        }
                    }
                }
                            
                
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinTrack API v1");
                    });
                    app.UseCors("DevPolicy");
                }
                app.UseSerilogRequestLogging(options =>
                {
                    options.EnrichDiagnosticContext = (dc, ctx) =>
                    {
                        dc.Set("ClientIP", ctx.Connection.RemoteIpAddress?.ToString() ?? "undefined");
                        dc.Set("UserAgent", ctx.Request.Headers.UserAgent.ToString());
                    };

                    options.GetLevel = (httpContext, elapsed, ex) =>
                    {
                        if (httpContext.Response.StatusCode >= 500 || ex != null)
                        {
                            return Serilog.Events.LogEventLevel.Warning;
                        }
                        return Serilog.Events.LogEventLevel.Information;
                    };
                });

                app.UseExceptionHandler();

                app.UseRouting();

                app.UseRateLimiter();

                app.UseAuthentication();
                app.UseAuthorization();
                app.UseMiddleware<UserEnrichmentMiddleware>();

                app.UseMiddleware<UserExistenceMiddleware>();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectly");
                if (isTesting) throw;
            }
            finally
            {
                if (!isTesting)
                {
                    Log.CloseAndFlush();
                }
            }
        }




        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddHealthChecks();

            //Authorization and authetication
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole(UserRoles.Admin);
                    policy.RequireClaim(JwtRegisteredClaimNames.Amr, "mfa");
                });

                options.AddPolicy("AccessToken", policy =>
                {
                    policy.RequireClaim("token_type", "access");
                });

                options.AddPolicy("MfaPending", policy =>
                {
                    policy.RequireClaim("token_type", "2fa_pending");
                });

                options.AddPolicy("VerifiedEmail", policy =>
                {
                    policy.RequireClaim(JwtRegisteredClaimNames.Amr, "mfa");
                });

            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = config["JwtOptions:Issuer"],
                    ValidAudience = config["JwtOptions:Audience"],
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    SignatureValidator = (token, _) => new JsonWebTokenHandler().ReadJsonWebToken(token),

                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = "role",

                };
                opt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {                        
                        var signingService = context.HttpContext.RequestServices.GetRequiredService<IJwtSigningService>();
                        var rawToken = context.SecurityToken as JsonWebToken ?? throw new NullReferenceException("Unable to retrive jwt token");
                        bool success = await signingService.VerifyTokenAsync(rawToken.EncodedToken);
                        if (!success)
                        {
                            throw new SecurityTokenInvalidSignatureException("Signature was rejected");
                        }
                    }
                };
                opt.MapInboundClaims = false;
            });



            //Rate limmiters
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("MfaCompleteLimit", context =>
                {
                    var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(2),
                            QueueLimit = 0
                            
                        });
                });


                options.AddPolicy("MfaResendCodeLimit", context =>
                {
                    var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "unknown";
                    return RateLimitPartition.GetSlidingWindowLimiter(userId, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(30),
                        SegmentsPerWindow = 10,
                        QueueLimit = 0
                    });
                });

                options.AddPolicy("EmailVerificationSendLimit", context =>
                {
                    var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("EmailVerificationCheckLimit", context =>
                {
                    var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        });
                });

            });
                

            //MedidtR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
            });

            //Configuration
            services.Configure<JwtOptions>(config.GetSection("JwtOptions"));
            services.Configure<VaultOptions>(config.GetSection("HashicorpVaultOptions"));
            services.Configure<CacheOptions>(config.GetSection("RedisOptions"));
            services.Configure<MessageStreamOptions>(config.GetSection("NatsOptions:StreamOptions"));

            //Resilience
            services.AddResiliencePipeline("cache-pipeline", (builder, context) =>
            {
                var cacheOptions = context.ServiceProvider.GetRequiredService<IOptions<CacheOptions>>().Value;
                var breakerOptions = cacheOptions.CircuitBreakerOptions;

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    MinimumThroughput = breakerOptions.MinimumThroughput,
                    SamplingDuration = breakerOptions.SamplingDuration,
                    FailureRatio = breakerOptions.FailureRatio,
                    BreakDuration = breakerOptions.BreakDuration

                });
            });

            //Services
            services.AddScoped<TransferService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IJwtSigningService, JwtSigningService>();
            services.AddSingleton<IPasswordHasher, PBKDF2PasswordHasher>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<ITotpService, TotpService>();
            services.AddSingleton<IChallengeTokenTracker, ChallengeTokenTracker>();

            //Hashicorp Vault 
            services.AddSingleton<IVaultTokenProvider, VaultTokenProvider>();
            services.AddTransient<VaultTokenHeaderHandler>();

            var vaultAddress = config.GetConnectionString("Vault") ?? "http://localhost:8200";
            services.AddHttpClient("SigningService", client =>
            {
                client.BaseAddress = new Uri(vaultAddress);

            }).AddHttpMessageHandler<VaultTokenHeaderHandler>();
            services.AddHttpClient("VaultTokenProvider", client =>
            {
                client.BaseAddress = new Uri(vaultAddress);

            });

            //NATS
            services.AddNatsClient(options =>
            {
                var url = config.GetConnectionString("Nats") ?? "nats://localhost:4222";
                options.ConfigureOptions(builder =>
                {
                    builder.Configure(opts =>
                    {
                        opts.Opts = opts.Opts with { Url = url };
                    });
                });
            });
            services.AddSingleton<INatsJSContext>(sp =>
            {
                var conn = sp.GetRequiredService<INatsConnection>();
                return conn.CreateJetStreamContext();
            });
            services.AddSingleton<NatsMessagePublisher>();
            services.AddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<NatsMessagePublisher>());

            //AutoMapper
            services.AddAutoMapper(typeof(Infrastructure.AssemblyReference).Assembly);

            //Data
            services.AddDbContext<DatabaseClient>(opt =>
            {
                var connectionString = config.GetConnectionString("postgres")
                    ?? throw new InvalidOperationException("connection string was not found");
                connectionString = connectionString
                                    .Replace("{DB_USER}", Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "")
                                    .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "")
                                    .Replace("{DB_PORT}", Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "")
                                    .Replace("{DB_HOST}", Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "");

                opt.UseNpgsql(connectionString);
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();  
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepsitory>();

            //Redis cache
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                string connectionString = config.GetConnectionString("Redis") ?? "localhost:6379";
                var options = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    Password = "",
                    ConnectTimeout = 2000,
                    SyncTimeout = 2000
                };
                options.EndPoints.Add(connectionString);
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<ICacheKeyProvider, RedisKeyProvider>();
            services.Decorate<ICacheService, CacheResilienceDecorator>();


            services.Decorate<IUserRepository, CachedUserRepositoryDecorator>();
            services.Decorate<IAccountRepository, CachedAccountRepositoryDecorator>();
            services.Decorate<ITransactionRepository, CachedTransactionRepositoryDecorator>();


            //Docs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "FinTrack API",
                    Description = "an ASP.NET Core banking system prototype API"
                });

                var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var pathToXml = Path.Combine(AppContext.BaseDirectory, xmlFileName);
                c.IncludeXmlComments(pathToXml);
            });


            //CORS
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("DevPolicy", opt =>
                {
                    opt.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost")
                    .AllowCredentials();
                });
            });
                
            //Exception handlers
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();


            //Telemetry
            services.AddOpenTelemetry()
                .ConfigureResource(resources => resources
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddAttributes(new Dictionary<string, object> { ["environment"] = environment }))
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddPrometheusExporter());
        }
    }
}
