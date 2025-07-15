using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResourceService.Infrastructure.ExternalServices;

public static class BrokerDependencyInjection
{
    public static void RegisterBrokerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var brokerSettings = configuration.GetSection("BrokerSettings");
        var brokerUsername = brokerSettings["Username"];
        var brokerPassword = brokerSettings["Password"];
        var brokerHost = brokerSettings["Host"];
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(brokerHost, "/", h =>
                {
                    h.Username(brokerUsername);
                    h.Password(brokerPassword);
                });
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                cfg.UseInMemoryOutbox(); 
            });
        });

        services.AddMassTransitHostedService();
        
    }
}