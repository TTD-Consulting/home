using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Configuration;
using RulesEngine.Core;

namespace RulesEngine.DependencyInjection;

public class RulesEngineServiceProvider
{
    public static IServiceProvider CreateProvider<T>(string configurationFilePath)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine<T>(configurationFilePath);
        return services.BuildServiceProvider();
    }
    
    public static IServiceProvider CreateProvider<T>(Func<IServiceProvider, IEnumerable<IRule<T>>> rulesFactory)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine<T>(rulesFactory);
        return services.BuildServiceProvider();
    }
}