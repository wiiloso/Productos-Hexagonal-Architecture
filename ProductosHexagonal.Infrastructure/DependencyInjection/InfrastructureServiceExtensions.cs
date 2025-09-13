using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductosHexagonal.Domain.Ports.Outbound;
using ProductosHexagonal.Infrastructure.Adapters.Outbound;
using ProductosHexagonal.Infrastructure.Data;

namespace ProductosHexagonal.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Configurar Entity Framework con SQL Server
            services.AddDbContext<ProductosDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ProductosDbContext).Assembly.FullName)
                ));

            // Registrar repositorios
            services.AddScoped<IProductoRepository, SqlServerProductoRepository>();

            return services;
        }
    }
}