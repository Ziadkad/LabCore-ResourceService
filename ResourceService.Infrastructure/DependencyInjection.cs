using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResourceService.Application.Common.Interfaces;
using ResourceService.Infrastructure.Data;
using ResourceService.Infrastructure.ExternalServices;
using ResourceService.Infrastructure.ExternalServices.BrokerService;
using ResourceService.Infrastructure.ExternalServices.UserContext;
using ResourceService.Infrastructure.Repositories;

namespace ResourceService.Infrastructure;

public static class DependencyInjection
{
    public static void RegisterDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<AppDbContext>());
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IResourceReservationRepository, ResourceReservationRepository>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IReservationProducer, ReservationProducer>();
        
        
        services.RegisterBrokerServices(configuration);
    }
}