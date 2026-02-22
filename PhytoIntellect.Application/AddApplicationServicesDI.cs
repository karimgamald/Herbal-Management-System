using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
namespace PhytoIntellect.Application;

public static class AddApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();


        return services;
    }
}
