using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ResourceService.Domain.Common;

namespace ResourceService.Application;

public static class DependencyInjection
{
    public static void RegisterApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(BaseModel))!));

    }
}