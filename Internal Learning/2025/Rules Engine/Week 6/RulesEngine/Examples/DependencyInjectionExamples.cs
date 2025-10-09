using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Configuration;
using RulesEngine.Core;
using RulesEngine.DependencyInjection;
using RulesEngine.Models;

namespace RulesEngine.Examples;

public static class DependencyInjectionExamples
{
    public static void DemonstrateExtensionMethods()
    {
        Console.WriteLine("Dependency Injection Examples");
        Console.WriteLine("============================");
        Console.WriteLine();

        var applicant = new UnderwritingInput { Age = 25, IsSmoker = false, Income = 15000, RequestedCover = 100000 };

        // Example 1: Using extension method with file path
        Console.WriteLine("1. Using AddRulesEngine with configuration file:");
        DemonstrateWithConfigFile(applicant);

        Console.WriteLine();

        // Example 2: Using extension method with custom factory
        Console.WriteLine("2. Using AddRulesEngine with custom rule factory:");
        DemonstrateWithCustomFactory(applicant);

        Console.WriteLine();

        // Example 3: Manual service registration
        Console.WriteLine("3. Manual service registration:");
        DemonstrateManualRegistration(applicant);
    }

    private static void DemonstrateWithConfigFile(UnderwritingInput applicant)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine<UnderwritingInput>("Configuration/underwriting-rules.json");
        
        var provider = services.BuildServiceProvider();
        var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
        
        EvaluateAndDisplay(engine, applicant);
    }

    private static void DemonstrateWithCustomFactory(UnderwritingInput applicant)
    {
        var services = new ServiceCollection();
        services.AddRulesEngine<UnderwritingInput>(provider =>
        {
            var loader = provider.GetRequiredService<JsonRuleLoader<UnderwritingInput>>();
            var customRules = """
            [
              {
                "field": "Age",
                "operator": "<",
                "value": 30,
                "message": "Age restriction for custom policy"
              }
            ]
            """;
            return loader.LoadRulesFromJson(customRules);
        });
        
        var provider = services.BuildServiceProvider();
        var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
        
        EvaluateAndDisplay(engine, applicant);
    }

    private static void DemonstrateManualRegistration(UnderwritingInput applicant)
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<JsonRuleLoader<UnderwritingInput>>();
        services.AddScoped<IEnumerable<IRule<UnderwritingInput>>>(provider =>
        {
            var loader = provider.GetRequiredService<JsonRuleLoader<UnderwritingInput>>();
            return loader.LoadRulesFromFile("Configuration/underwriting-rules.json");
        });
        services.AddScoped<RuleEngine<UnderwritingInput>>();
        
        var provider = services.BuildServiceProvider();
        var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
        
        EvaluateAndDisplay(engine, applicant);
    }

    private static void EvaluateAndDisplay(RuleEngine<UnderwritingInput> engine, UnderwritingInput applicant)
    {
        var result = engine.Evaluate(applicant, ExecutionMode.Scored);
        
        Console.WriteLine($"  Applicant: Age {applicant.Age}, Income {applicant.Income:C}");
        Console.WriteLine($"  Result: {(result.AllPassed ? "Passed" : "Failed")}");
        Console.WriteLine($"  Score: {result.Score} out of {engine.ExecutionLogs.Count}");
        
        if (!result.AllPassed)
        {
            Console.WriteLine("  Failed rules:");
            foreach (var message in result.FailedMessages)
            {
                Console.WriteLine($"    - {message}");
            }
        }
    }
}