using Microsoft.Extensions.DependencyInjection;
using RulesEngine;
using RulesEngine.Configuration;
using RulesEngine.Core;
using RulesEngine.Models;

Console.WriteLine("JSON-Based Rules Engine - Week 4 Demo");
Console.WriteLine("=====================================");
Console.WriteLine();

var applicants = new[]
{
    new UnderwritingInput { Age = 22, IsSmoker = true, Income = 30000, RequestedCover = 400000 },
    new UnderwritingInput { Age = 63, IsSmoker = false, Income = 12000, RequestedCover = 100000 },
    new UnderwritingInput { Age = 45, IsSmoker = false, Income = 8000, RequestedCover = 90000 },
    new UnderwritingInput { Age = 35, IsSmoker = true, Income = 20000, RequestedCover = 150000 },
    new UnderwritingInput { Age = 27, IsSmoker = false, Income = 50000, RequestedCover = 300000 }
};

Console.WriteLine("Demo 1: Direct JSON File Loading");
Console.WriteLine(new string('-', 40));
DemonstrateFileBasedRules(applicants);

Console.WriteLine();
Console.WriteLine("Demo 2: Dependency Injection with JSON Rules");
Console.WriteLine(new string('-', 40));
DemonstrateDependencyInjection(applicants);

Console.WriteLine();
Console.WriteLine("Demo 3: Runtime JSON String Loading");
Console.WriteLine(new string('-', 40));
DemonstrateRuntimeRules(applicants);

static void DemonstrateFileBasedRules(UnderwritingInput[] applicants)
{
    var ruleLoader = new JsonRuleLoader<UnderwritingInput>();
    var rules = ruleLoader.LoadRulesFromFile("Configuration/underwriting-rules.json");
    
    Console.WriteLine($"Loaded {rules.Count} rules from configuration file");
    
    var engine = new RuleEngine<UnderwritingInput>(rules);
    EvaluateApplicants(engine, applicants.Take(2).ToArray());
}

static void DemonstrateDependencyInjection(UnderwritingInput[] applicants)
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
    
    Console.WriteLine("Using dependency injection pattern");
    EvaluateApplicants(engine, applicants.Skip(2).Take(2).ToArray());
}

static void DemonstrateRuntimeRules(UnderwritingInput[] applicants)
{
    var runtimeRules = """
    [
      {
        "field": "Age",
        "operator": "<",
        "value": 18,
        "message": "Must be 18 or older"
      },
      {
        "field": "Income",
        "operator": "<",
        "value": 5000,
        "message": "Minimum income not met"
      }
    ]
    """;

    var ruleLoader = new JsonRuleLoader<UnderwritingInput>();
    var rules = ruleLoader.LoadRulesFromJson(runtimeRules);
    
    Console.WriteLine($"Loaded {rules.Count} rules from runtime JSON string");
    
    var engine = new RuleEngine<UnderwritingInput>(rules);
    EvaluateApplicants(engine, applicants.Skip(4).Take(1).ToArray());
}

static void EvaluateApplicants(RuleEngine<UnderwritingInput> engine, UnderwritingInput[] applicants)
{
    foreach (var applicant in applicants)
    {
        var result = engine.Evaluate(applicant, ExecutionMode.Scored);

        Console.WriteLine($"  Applicant: Age {applicant.Age}, Smoker {applicant.IsSmoker}, " +
                         $"Income {applicant.Income:C}, Cover {applicant.RequestedCover:C}");

        if (result.AllPassed)
        {
            Console.WriteLine("  Result: All rules passed");
        }
        else
        {
            Console.WriteLine("  Result: Failed rules:");
            foreach (var message in result.FailedMessages)
            {
                Console.WriteLine($"    - {message}");
            }
        }

        Console.WriteLine($"  Score: {result.Score} out of {engine.ExecutionLogs.Count} rules passed");
        Console.WriteLine();
        
        engine.ExecutionLogs.Clear();
    }
}



