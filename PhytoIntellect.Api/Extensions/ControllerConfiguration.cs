using System.Text.Json.Serialization;

namespace PhytoIntellect.Api.Extensions;

public static class ControllerConfiguration
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        // هنا بنحط الـ Controllers وإعدادات الـ JSON
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

        // لو عندك أي إعدادات تانية خاصة بالـ API قدام (زي الكورس CORS) حطها هنا

        return services;
    }
}
