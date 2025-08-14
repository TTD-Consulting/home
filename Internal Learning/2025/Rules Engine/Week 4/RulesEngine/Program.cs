using Microsoft.Extensions.DependencyInjection;
using RulesEngine;
using RulesEngine.Rules.Underwriting;

Console.WriteLine("JSON-Based Rules Engine - Week 4");
Console.WriteLine("=====================================\n");

// Test data
var applicants = new[]
{
    new UnderwritingInput { Age = 22, IsSmoker = true, Income = 30000, RequestedCover = 400000 },
    new UnderwritingInput { Age = 63, IsSmoker = false, Income = 12000, RequestedCover = 100000 },
    new UnderwritingInput { Age = 45, IsSmoker = false, Income = 8000, RequestedCover = 90000 },
    new UnderwritingInput { Age = 35, IsSmoker = true, Income = 20000, RequestedCover = 150000 },
    new UnderwritingInput { Age = 27, IsSmoker = false, Income = 50000, RequestedCover = 300000 },
    new UnderwritingInput { Age = 70, IsSmoker = false, Income = 15000, RequestedCover = 200000 }
};

Console.WriteLine("1. Direct JSON Rules Loading");
Console.WriteLine(new string('=', 50));
RunDirectJsonRules(applicants);

Console.WriteLine("\n\n2. Dependency Injection with JSON Rules");
Console.WriteLine(new string('=', 50));
RunDependencyInjectionWithJson(applicants);

Console.WriteLine("\n\n3. Runtime JSON Rules (String-based)");
Console.WriteLine(new string('=', 50));
RunRuntimeJsonRules(applicants);

static void RunDirectJsonRules(UnderwritingInput[] applicants)
{
    try
    {
        // Load rules directly from JSON file
        var ruleLoader = new JsonRuleLoader<UnderwritingInput>();
        var rules = ruleLoader.LoadRulesFromFile("enhanced-rules.json");
        
        Console.WriteLine($"Loaded {rules.Count} rules from enhanced-rules.json");
        
        var engine = new RuleEngine<UnderwritingInput>(rules);
        EvaluateApplicants(engine, applicants);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void RunDependencyInjectionWithJson(UnderwritingInput[] applicants)
{
    try
    {
        var services = new ServiceCollection();
        
        // Register the JSON rule loader
        services.AddSingleton<JsonRuleLoader<UnderwritingInput>>();
        
        // Register rules factory that loads from JSON
        services.AddScoped<IEnumerable<IRule<UnderwritingInput>>>(provider =>
        {
            var loader = provider.GetRequiredService<JsonRuleLoader<UnderwritingInput>>();
            return loader.LoadRulesFromFile("enhanced-rules.json");
        });
        
        // Register the rule engine
        services.AddScoped<RuleEngine<UnderwritingInput>>();
        
        var provider = services.BuildServiceProvider();
        
        Console.WriteLine("Using Dependency Injection with JSON-loaded rules");
        
        var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
        EvaluateApplicants(engine, applicants);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void RunRuntimeJsonRules(UnderwritingInput[] applicants)
{
    // Example of loading rules from a JSON string (could come from API, database, etc.)
    var dynamicRulesJson = """
    [
      {
        "field": "Age",
        "operator": ">",
        "value": 18,
        "message": "Must be over 18 years old"
      },
      {
        "field": "Age",
        "operator": "<",
        "value": 80,
        "message": "Maximum age is 79 years"
      },
      {
        "field": "Income",
        "operator": ">",
        "value": 5000,
        "message": "Minimum income requirement not met"
      }
    ]
    """;

    try
    {
        var ruleLoader = new JsonRuleLoader<UnderwritingInput>();
        var rules = ruleLoader.LoadRulesFromJson(dynamicRulesJson);
        
        Console.WriteLine($"Loaded {rules.Count} rules from runtime JSON");
        
        var engine = new RuleEngine<UnderwritingInput>(rules);
        EvaluateApplicants(engine, applicants);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void EvaluateApplicants(RuleEngine<UnderwritingInput> engine, UnderwritingInput[] applicants)
{
    var index = 1;
    foreach (var input in applicants)
    {
        var result = engine.Evaluate(input, ExecutionMode.Scored);

        Console.WriteLine($"\n--- Applicant #{index++} ---");
        Console.WriteLine($"Age: {input.Age}, Smoker: {input.IsSmoker}, Income: {input.Income:C}, Cover: {input.RequestedCover:C}");

        if (!result.AllPassed)
        {
            Console.WriteLine("Failed Rules:");
            foreach (var message in result.FailedMessages)
            {
                Console.WriteLine($"   {message}");
            }
        }
        else
        {
            Console.WriteLine("All rules passed");
        }

        Console.WriteLine($"Score: {result.Score}/{engine.ExecutionLogs.Count}");
        Console.WriteLine("Execution Details:");
        foreach (var log in engine.ExecutionLogs)
        {
            var status = log.Passed ? "Passed" : "Failed";
            Console.WriteLine($"   {status} {log.RuleName}: {log.Message}");
        }
        
        // Clear logs for next applicant
        engine.ExecutionLogs.Clear();
    }
}


