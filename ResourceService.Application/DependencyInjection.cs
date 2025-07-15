using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResourceService.Application.Resource;
using ResourceService.Application.ResourceReservation;
using ResourceService.Domain.Common;

namespace ResourceService.Application;

public static class DependencyInjection
{
    public static void RegisterApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(BaseModel))!));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(typeof(ResourceMapper).Assembly);
        services.AddAutoMapper(typeof(ResourceReservationMapper).Assembly);

    }
}