using Microsoft.OpenApi.Models;
using System.Reflection;
namespace FinTrack.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinTrack API v1");
                });
            }
            
            app.MapControllers();

            app.Run();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
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
        }
    }
}
