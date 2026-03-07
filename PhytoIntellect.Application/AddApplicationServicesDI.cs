using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Application.Contracts.Users;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using YourProject.Contracts.Users;
namespace PhytoIntellect.Application;

public static class AddApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
        services.AddScoped<IHerbalistService, HerbalistService>();

        // 2. تسجيل الـ AutoMapper (السطر السحري ده بيقرا كل الـ Profiles لوحده)
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddFluentValidationServices();


        return services;
    }

    private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        // 1. تسجيل كل الـ Validators اللي في البروجيكت بتاعك مرة واحدة
        services.AddValidatorsFromAssemblyContaining<PatientValidator>();
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        //services.AddValidatorsFromAssemblyContaining<ManageUserAddressValidator>();

        // Register all validators 
        services.AddValidatorsFromAssemblyContaining<RegisterAccountValidator>();

        // 2. تفعيل الـ Auto Validation باستخدام مكتبة SharpGrip
        services.AddFluentValidationAutoValidation();

        return services;
    }
}
