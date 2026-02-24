using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Services;
using PhytoIntellect.Core.Interfaces;
namespace PhytoIntellect.Application;

public static class AddApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();


        return services;
    }
}
