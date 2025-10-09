using Microsoft.Extensions.DependencyInjection;
using RulesEngine;
using RulesEngine.Core;
using RulesEngine.DependencyInjection;
using RulesEngine.Examples;
using RulesEngine.Models;

Console.WriteLine("JSON-Based Rules Engine - Week 5+6 Demo");
Console.WriteLine("=======================================");
Console.WriteLine();

var applicants = new[]
{
    new UnderwritingInput { Age = 22, IsSmoker = true, Income = 30000, RequestedCover = 400000 },
    new UnderwritingInput { Age = 63, IsSmoker = false, Income = 12000, RequestedCover = 100000 },
    new UnderwritingInput { Age = 45, IsSmoker = false, Income = 8000, RequestedCover = 90000 },
    new UnderwritingInput { Age = 35, IsSmoker = true, Income = 20000, RequestedCover = 150000 },
    new UnderwritingInput { Age = 27, IsSmoker = false, Income = 50000, RequestedCover = 600000 },
    new UnderwritingInput { Age = 35, IsSmoker = false, Income = 75000, RequestedCover = 250000 } // This applicant should pass all rules
};

Console.WriteLine("Dependency Injection with JSON Rules");
Console.WriteLine(new string('-', 40));
DemonstrateDependencyInjection(applicants);

Console.WriteLine("\n" + new string('=', 60) + "\n");

// New RuleSet functionality demo
RuleSetExamples.RunRuleSetDemo();

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Advanced RuleSet demo
AdvancedRuleSetExample.RunAdvancedDemo();

static void DemonstrateDependencyInjection(UnderwritingInput[] applicants)
{
    var provider = RulesEngineServiceProvider.CreateProvider<UnderwritingInput>("Configuration/underwriting-rules.json");
    var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
    
    Console.WriteLine("Using dependency injection pattern");
    EvaluateApplicants(engine, applicants);
}

static void EvaluateApplicants(RuleEngine<UnderwritingInput> engine, UnderwritingInput[] applicants)
{
    foreach (var applicant in applicants)
    {
        var result = engine.Evaluate(applicant, ExecutionMode.Scored);

        Console.WriteLine($"  Applicant: Age {applicant.Age}, Smoker {applicant.IsSmoker}, " +
                         $"Income {applicant.Income:C}, Cover {applicant.RequestedCover:C}");

        if (result.DecisionOutcome.HasValue)
        {
            var outcome = result.DecisionOutcome.Value;
            var outcomeColor = outcome switch
            {
                DecisionOutcome.Approve => ConsoleColor.Green,
                DecisionOutcome.Refer => ConsoleColor.Yellow,
                DecisionOutcome.Reject => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
            
            Console.ForegroundColor = outcomeColor;
            Console.WriteLine($"  Decision: {outcome}");
            Console.ResetColor();
        }

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
        
        Console.WriteLine($"  Score: {result.Score} out of {engine.ExecutionLogs.Count}");
        Console.WriteLine();
    }
}




