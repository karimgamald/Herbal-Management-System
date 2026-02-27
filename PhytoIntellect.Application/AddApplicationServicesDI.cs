using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
using System.Reflection;
namespace PhytoIntellect.Application;

public static class AddApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPatientService, PatientService>();

        // 2. تسجيل الـ AutoMapper (السطر السحري ده بيقرا كل الـ Profiles لوحده)
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
