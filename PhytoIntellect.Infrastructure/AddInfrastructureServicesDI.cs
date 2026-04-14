using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Services;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.ExternalApi;
using PhytoIntellect.Infrastructure.Presistence;
using PhytoIntellect.Infrastructure.Repository;
using PhytoIntellect.Infrastructure.UOW;
using Refit;

namespace PhytoIntellect.Infrastructure;

public static class AddInfrastructureServicesDI
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISubOrderRepository, SubOrderRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAiPredictionService, AiPredictionWrapperService>();

        services.AddExternalApiAiServices(configuration);
        return services;
    }

    public static IServiceCollection AddDbContextServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    }));

        return services;
    }

    public static IServiceCollection AddExternalApiAiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var aiBaseUrl = configuration["AiSettings:BaseUrl"];

        services.AddRefitClient<IAiFlaskClient>()//<IAiPredictionService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(aiBaseUrl!));

        return services;
    }
}
