using DeliveryApp.API.Extensions;
using DeliveryApp.Infrastructure;

namespace DeliveryApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Register Infrastructure Services(Services + ApplicationDbContext)
            // Read Connection String From Configurations(UserSecrets ,appsetting ,env)
            builder.Services.AddInfrastructure(builder.Configuration);  
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Swagger With Jwt
            builder.Services.AddSwaggerWithJwt();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}