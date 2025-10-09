using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Configuration;
using RulesEngine.Core;

namespace RulesEngine.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRulesEngine<T>(this IServiceCollection services, string configurationFilePath)
    {
        services.AddSingleton<JsonRuleLoader<T>>();
        services.AddScoped<RuleOutcomeResolver>();
        
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
        services.AddScoped<RuleOutcomeResolver>();
        services.AddScoped(rulesFactory);
        services.AddScoped<RuleEngine<T>>();
        
        return services;
    }

    /// <summary>
    /// Adds RuleSet registry services to the DI container
    /// </summary>
    /// <typeparam name="T">The type of input the rules evaluate</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRuleSetRegistry<T>(this IServiceCollection services)
    {
        services.AddSingleton<IRuleSetRegistry<T>, RuleSetRegistry<T>>();
        return services;
    }

    /// <summary>
    /// Adds RulesEngine with RuleSet registry support
    /// </summary>
    /// <typeparam name="T">The type of input the rules evaluate</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configurationFilePath">Path to the JSON configuration file</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRulesEngineWithRuleSets<T>(this IServiceCollection services, string configurationFilePath)
    {
        services.AddRulesEngine<T>(configurationFilePath);
        services.AddRuleSetRegistry<T>();
        return services;
    }

    /// <summary>
    /// Adds RulesEngine with RuleSet registry support using a custom rules factory
    /// </summary>
    /// <typeparam name="T">The type of input the rules evaluate</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="rulesFactory">Factory function for creating rules</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRulesEngineWithRuleSets<T>(this IServiceCollection services, Func<IServiceProvider, IEnumerable<IRule<T>>> rulesFactory)
    {
        services.AddRulesEngine<T>(rulesFactory);
        services.AddRuleSetRegistry<T>();
        return services;
    }
}