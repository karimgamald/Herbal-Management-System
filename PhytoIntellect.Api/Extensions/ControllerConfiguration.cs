using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Api.Extensions;

public static class ControllerConfiguration
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


        return services;
    }
}
