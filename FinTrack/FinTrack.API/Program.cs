using Microsoft.OpenApi.Models;
using FinTrack.API.Infrastructure.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.API.Infrastructure.Identity.Services;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Infrastructure.Services;
using FinTrack.API.Infrastructure.Identity.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinTrack.API.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace FinTrack.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                .Build();

            ConfigureServices(builder.Services, builder.Configuration);


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinTrack API v1");
                });
                app.UseCors("DevPolicy");
            }
            

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }


        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            var keyProvider = new JwtKeyService();
            

            services.AddControllers();

            //Authorization and authetication
            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = config["JwtOptions:Issuer"],
                    ValidAudience = config["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(keyProvider.GetKey()),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role
                    
                };
                opt.MapInboundClaims = false;

            });
            
            //MedidtR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
            });

            //Configuration
            services.Configure<JwtOptions>(config.GetSection("JwtOptions"));

            //Services
            services.AddScoped<TransferService>();
            services.AddSingleton<JwtKeyService>(keyProvider);
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IPasswordHasher, PBKDF2PasswordHasher>();

            //AutoMapper
            services.AddAutoMapper(typeof(Infrastructure.AssemblyReference).Assembly);

            //Data
            services.AddDbContext<DatabaseClient>(opt =>
            {
                var connectionString = config.GetConnectionString("postgres")
                    ?? throw new InvalidOperationException("connection string was not found");
                connectionString = connectionString
                                    .Replace("{DB_USER}", Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "")
                                    .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "");

                opt.UseNpgsql(connectionString);
            });
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();


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
        }
    }
}
