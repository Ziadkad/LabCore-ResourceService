using MassTransit;
using Microsoft.Extensions.Configuration;
using ResourceService.Application.Common.Interfaces;
using Shared.Models;

namespace ResourceService.Infrastructure.ExternalServices.BrokerService;

public class ReservationProducer(ISendEndpointProvider sendEndpointProvider, IConfiguration configuration) : IReservationProducer
{
    public async Task SendAsync(ResourceReservationMessage message)
    {
        var brokerSettings =  configuration.GetSection("BrokerSettings");
        string queueName = brokerSettings["AddReservationEndpoint"];
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name must not be empty", nameof(queueName));
        
        var uri = new Uri($"queue:{queueName}");
        
        var endpoint = await sendEndpointProvider.GetSendEndpoint(uri);
        
        await endpoint.Send(message);
    }
    
    
    public async Task SendDeleteAsync(ResourceReservationMessage message)
    {
        var brokerSettings =  configuration.GetSection("BrokerSettings");
        string queueName = brokerSettings["DeleteReservationEndpoint"];
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name must not be empty", nameof(queueName));
        
        var uri = new Uri($"queue:{queueName}");
        
        var endpoint = await sendEndpointProvider.GetSendEndpoint(uri);
        
        await endpoint.Send(message);
    }
}