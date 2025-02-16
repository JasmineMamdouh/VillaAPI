
using Microsoft.AspNetCore.JsonPatch;

using Microsoft.OpenApi.Models;


namespace VillaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Villa API",
                    Version = "v1"
                });
                // Support JSON Patch for Swagger
                options.SupportNonNullableReferenceTypes();
                options.MapType<JsonPatchDocument>(() => new OpenApiSchema
                {
                    Type = "object",
                    Example = new Microsoft.OpenApi.Any.OpenApiString(
                        "[{\"op\": \"replace\", \"path\": \"/name\", \"value\": \"Updated Villa Name\"}]"
                    )
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
