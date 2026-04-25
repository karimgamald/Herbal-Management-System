using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Contracts.Accounts;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
using PhytoIntellect.Application.Settings;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using YourProject.Contracts.Users;
namespace PhytoIntellect.Application;

public static class AddApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
        services.AddScoped<IHerbalistService, HerbalistService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IDiseaseService, DiseaseService>();
        services.AddScoped<IHerbService, HerbService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IReviewRecipeService, ReviewRecipeService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ISubOrderService, SubOrderService>();
        services.AddScoped<IAiRecipeService, AiRecipeService>();
        services.AddScoped<IHerbalistAiRecipeService, HerbalistAiRecipeService>();

        //for Email
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        services.AddFluentValidationServices();

        return services;
    }

    private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UpdatePatientValidator>();
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateAiRecipeValidator>();
        //services.AddValidatorsFromAssemblyContaining<ManageUserAddressValidator>();

        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        services.AddValidatorsFromAssemblyContaining<RegisterUserAuthValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}
