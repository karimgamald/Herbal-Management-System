using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PhytoIntellect.Api.Extensions;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // معلومات API
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PhytoIntellect API",
                Version = "v1",
                Description = "API for managing herbalists, users, and authentication"
            });

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // عرض Enums كأسماء بدل الأرقام
            c.UseInlineDefinitionsForEnums();

            // لجعل TimeSpan يظهر بصيغة HH:mm
            c.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "time",
                Example = OpenApiAnyFactory.CreateFromJson("\"00:00\"")
            });

            // إذا حابب، تضيف تعليقات XML
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}