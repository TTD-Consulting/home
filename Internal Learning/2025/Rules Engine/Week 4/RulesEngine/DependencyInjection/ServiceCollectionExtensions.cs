using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Configuration;
using RulesEngine.Core;

namespace RulesEngine.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRulesEngine<T>(this IServiceCollection services, string configurationFilePath)
    {
        services.AddSingleton<JsonRuleLoader<T>>();
        
        services.AddScoped<IEnumerable<IRule<T>>>(provider =>
        {
            var loader = provider.GetRequiredService<JsonRuleLoader<T>>();
            return loader.LoadRulesFromFile(configurationFilePath);
        });
        
        services.AddScoped<RuleEngine<T>>();
        
        return services;
    }
    
    public static IServiceCollection AddRulesEngine<T>(this IServiceCollection services, Func<IServiceProvider, IEnumerable<IRule<T>>> rulesFactory)
    {
        services.AddSingleton<JsonRuleLoader<T>>();
        services.AddScoped(rulesFactory);
        services.AddScoped<RuleEngine<T>>();
        
        return services;
    }
}