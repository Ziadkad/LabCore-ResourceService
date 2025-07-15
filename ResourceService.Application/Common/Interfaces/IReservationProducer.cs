using Shared.Models;

namespace ResourceService.Application.Common.Interfaces;

public interface IReservationProducer
{
    Task SendAsync(ResourceReservationMessage message);
    Task SendDeleteAsync(ResourceReservationMessage message);
}