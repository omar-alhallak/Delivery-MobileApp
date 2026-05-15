using Microsoft.OpenApi.Models;

namespace DeliveryApp.API.Extensions
{
    // مع آلية عمله [Authorize] زر Swagger إضافة زر ل 
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            // ل Swagger تعديل إعدادات 
            services.AddSwaggerGen(options =>
            {
                // API تحديد أسم و إصدار ال 
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DeliveryApp.API",
                    Version = "v1"
                });

                // Swagger إدخال نظام الحماية ل 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token"
                });

                // API ربط نظام الحماية بال 
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            });

            return services;
        }
    }
}