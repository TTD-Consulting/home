using Microsoft.Extensions.DependencyInjection;
using RulesEngine;
using RulesEngine.Rules.Underwriting;

var services = new ServiceCollection();

services.AddScoped<IRule<UnderwritingInput>, AgeUnder25Rule>();
services.AddScoped<IRule<UnderwritingInput>, SmokerRule>();
services.AddScoped<IRule<UnderwritingInput>, CoverToIncomeRatioRule>();
services.AddScoped<IRule<UnderwritingInput>, SeniorCitizenRule>();
services.AddScoped<IRule<UnderwritingInput>, IncomeMinimumRule>();

services.AddScoped<RuleEngine<UnderwritingInput>>();

var provider = services.BuildServiceProvider();
var engine = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();

var applicants = new[]
{
    new UnderwritingInput { Age = 22, IsSmoker = true, Income = 30000, RequestedCover = 400000 },
    new UnderwritingInput { Age = 63, IsSmoker = false, Income = 12000, RequestedCover = 100000 },
    new UnderwritingInput { Age = 45, IsSmoker = false, Income = 8000, RequestedCover = 90000 },
    new UnderwritingInput { Age = 35, IsSmoker = true, Income = 20000, RequestedCover = 150000 },
    new UnderwritingInput { Age = 27, IsSmoker = false, Income = 50000, RequestedCover = 300000 }
};

var index = 1;
foreach (var input in applicants)
{
    var engineInstance = provider.GetRequiredService<RuleEngine<UnderwritingInput>>();
    var result = engineInstance.Evaluate(input, ExecutionMode.Scored);

    Console.WriteLine($"\n=== Applicant #{index++} ===");
    Console.WriteLine($"Age: {input.Age}, Smoker: {input.IsSmoker}, Income: {input.Income}, Cover: {input.RequestedCover}");

    if (!result.AllPassed)
    {
        Console.WriteLine("Failed Rules:");
        foreach (var message in result.FailedMessages)
        {
            Console.WriteLine(" - " + message);
        }
    }
    else
    {
        Console.WriteLine("All rules passed");
    }

    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine("Execution Logs:");
    foreach (var log in engineInstance.ExecutionLogs)
    {
        Console.WriteLine($"{log.Timestamp:u} - {log.RuleName}: {(log.Passed ? "Passed" : "Failed")} - {log.Message}");
    }

    Console.WriteLine(new string('-', 50));
}
