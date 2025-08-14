using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Configuration;
using RulesEngine.Core;

namespace RulesEngine.DependencyInjection;

public class RulesEngineConfiguration
{
    public string ConfigurationFilePath { get; set; } = string.Empty;
    public string JsonRulesContent { get; set; } = string.Empty;
    public bool UseFileWatcher { get; set; } = false;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(30);
}

public static class RulesEngineConfigurationExtensions
{
    public static IServiceCollection AddRulesEngine<T>(this IServiceCollection services, Action<RulesEngineConfiguration> configureOptions)
    {
        var configuration = new RulesEngineConfiguration();
        configureOptions(configuration);
        
        services.AddSingleton<JsonRuleLoader<T>>();
        
        if (!string.IsNullOrEmpty(configuration.ConfigurationFilePath))
        {
            services.AddScoped<IEnumerable<IRule<T>>>(provider =>
            {
                var loader = provider.GetRequiredService<JsonRuleLoader<T>>();
                return loader.LoadRulesFromFile(configuration.ConfigurationFilePath);
            });
        }
        else if (!string.IsNullOrEmpty(configuration.JsonRulesContent))
        {
            services.AddScoped<IEnumerable<IRule<T>>>(provider =>
            {
                var loader = provider.GetRequiredService<JsonRuleLoader<T>>();
                return loader.LoadRulesFromJson(configuration.JsonRulesContent);
            });
        }
        
        services.AddScoped<RuleEngine<T>>();
        
        return services;
    }
}