using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRecruitment.Domain.Interfaces;
using SmartRecruitment.Infrastructure.Repositories;
using SmartRecruitment.Infrastructure.Repository;

namespace SmartRecruitment.Infrastructure.Extensions;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SmartRecruitmentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}