namespace DeliveryApp.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddFrontendCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy.WithOrigins
                    (
                        "http://localhost:5173",
                        "http://localhost:8081",
                        "http://localhost:7273"


                    ).AllowAnyHeader()
                     .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}