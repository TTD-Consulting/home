using Microsoft.Extensions.DependencyInjection;
using RulesEngine.Core;
using RulesEngine.DependencyInjection;
using RulesEngine.Models;
using RulesEngine.Rules;

namespace RulesEngine.Examples;

public static class RuleSetExamples
{
    public static void RunRuleSetDemo()
    {
        Console.WriteLine("RuleSet Demo - Modular Rules Engine");
        Console.WriteLine("===================================");
        Console.WriteLine();

        // Create test applicants
        var applicants = new[]
        {
            new UnderwritingInput { Age = 30, IsSmoker = false, Income = 75000, RequestedCover = 250000 },  // Should pass all
            new UnderwritingInput { Age = 22, IsSmoker = true, Income = 30000, RequestedCover = 400000 },   // Age + Smoker + High ratio
            new UnderwritingInput { Age = 24, IsSmoker = false, Income = 120000, RequestedCover = 500000 }, // Suspicious income
            new UnderwritingInput { Age = 45, IsSmoker = false, Income = 50000, RequestedCover = 1200000 },  // Excessive cover
        };

        // Demo 1: Manual RuleSet creation and registry
        Console.WriteLine("Demo 1: Manual RuleSet Creation");
        Console.WriteLine(new string('-', 40));
        DemonstrateManualRuleSets(applicants);
        Console.WriteLine();

        // Demo 2: Using Dependency Injection
        Console.WriteLine("Demo 2: Dependency Injection with RuleSets");
        Console.WriteLine(new string('-', 40));
        DemonstrateRuleSetDependencyInjection(applicants);
    }

    private static void DemonstrateManualRuleSets(UnderwritingInput[] applicants)
    {
        // Create registry
        var registry = new RuleSetRegistry<UnderwritingInput>();

        // Create Life Cover RuleSet (standard underwriting rules)
        var lifeCoverRules = new List<IRule<UnderwritingInput>>
        {
            new AgeUnder25Rule(),
            new SmokerRule(),
            new MinimumIncomeRule(15000),
            new CoverToIncomeRatioRule(10.0m),
            new MaximumAgeRule(65)
        };
        var lifeCoverRuleSet = new RuleSet<UnderwritingInput>("LifeCoverRules", lifeCoverRules, ExecutionMode.AllPass);
        registry.AddRuleSet(lifeCoverRuleSet);

        // Create Fraud Check RuleSet (additional verification rules)
        var fraudCheckRules = new List<IRule<UnderwritingInput>>
        {
            new SuspiciousIncomeRule(),
            new ExcessiveCoverRule(),
            new IncomeVerificationRule()
        };
        var fraudCheckRuleSet = new RuleSet<UnderwritingInput>("FraudCheckRules", fraudCheckRules, ExecutionMode.FirstFail);
        registry.AddRuleSet(fraudCheckRuleSet);

        // Evaluate each applicant against both rule sets
        for (int i = 0; i < applicants.Length; i++)
        {
            var applicant = applicants[i];
            Console.WriteLine($"Applicant {i + 1}: Age {applicant.Age}, Smoker: {applicant.IsSmoker}, " +
                             $"Income: {applicant.Income:C}, Cover: {applicant.RequestedCover:C}");

            // Evaluate Life Cover Rules
            var lifeCoverResult = registry.GetRequiredRuleSet("LifeCoverRules").Evaluate(applicant);
            Console.WriteLine($"  LifeCoverRules: {(lifeCoverResult.AllPassed ? "PASS" : "FAIL")}");
            if (!lifeCoverResult.AllPassed)
            {
                foreach (var message in lifeCoverResult.FailedMessages)
                {
                    Console.WriteLine($"    - {message}");
                }
            }

            // Evaluate Fraud Check Rules
            var fraudCheckResult = registry.GetRequiredRuleSet("FraudCheckRules").Evaluate(applicant);
            Console.WriteLine($"  FraudCheckRules: {(fraudCheckResult.AllPassed ? "PASS" : "FAIL")}");
            if (!fraudCheckResult.AllPassed)
            {
                foreach (var message in fraudCheckResult.FailedMessages)
                {
                    Console.WriteLine($"    - {message}");
                }
            }

            // Overall decision
            var overallDecision = lifeCoverResult.AllPassed && fraudCheckResult.AllPassed ? "APPROVE" : "REFER/REJECT";
            Console.WriteLine($"  Overall Decision: {overallDecision}");
            Console.WriteLine();
        }
    }

    private static void DemonstrateRuleSetDependencyInjection(UnderwritingInput[] applicants)
    {
        // Setup DI container
        var services = new ServiceCollection();
        
        // Add RuleSet registry
        services.AddRuleSetRegistry<UnderwritingInput>();
        
        // Add other required services
        services.AddScoped<RuleOutcomeResolver>();

        // Build service provider and configure rule sets
        var serviceProvider = services.BuildServiceProvider();
        var registry = serviceProvider.GetRequiredService<IRuleSetRegistry<UnderwritingInput>>();

        // Configure rule sets through DI
        ConfigureRuleSets(registry);

        // Evaluate applicants
        foreach (var applicant in applicants.Take(2)) // Just show first 2 for brevity
        {
            Console.WriteLine($"Applicant: Age {applicant.Age}, Smoker: {applicant.IsSmoker}, " +
                             $"Income: {applicant.Income:C}, Cover: {applicant.RequestedCover:C}");

            // Get rule sets from registry
            var lifeCoverRuleSet = registry.GetRequiredRuleSet("LifeCoverRules");
            var fraudCheckRuleSet = registry.GetRequiredRuleSet("FraudCheckRules");

            // Evaluate with outcome resolver
            var outcomeResolver = serviceProvider.GetRequiredService<RuleOutcomeResolver>();
            var lifeCoverResult = lifeCoverRuleSet.Evaluate(applicant, outcomeResolver);
            var fraudCheckResult = fraudCheckRuleSet.Evaluate(applicant, outcomeResolver);

            Console.WriteLine($"  LifeCoverRules: {(lifeCoverResult.AllPassed ? "PASS" : "FAIL")} " +
                             $"(Decision: {lifeCoverResult.DecisionOutcome})");
            Console.WriteLine($"  FraudCheckRules: {(fraudCheckResult.AllPassed ? "PASS" : "FAIL")} " +
                             $"(Decision: {fraudCheckResult.DecisionOutcome})");
            Console.WriteLine();
        }

        // Show registry information
        Console.WriteLine("Registry Information:");
        Console.WriteLine($"  Total RuleSets: {registry.GetRuleSetNames().Count()}");
        foreach (var name in registry.GetRuleSetNames())
        {
            var ruleSet = registry.GetRequiredRuleSet(name);
            Console.WriteLine($"  - {name}: {ruleSet.Rules.Count} rules, Mode: {ruleSet.ExecutionMode}");
        }
    }

    private static void ConfigureRuleSets(IRuleSetRegistry<UnderwritingInput> registry)
    {
        // Life Cover Rules
        var lifeCoverRules = new List<IRule<UnderwritingInput>>
        {
            new AgeUnder25Rule(),
            new SmokerRule(),
            new MinimumIncomeRule(15000),
            new CoverToIncomeRatioRule(10.0m),
            new MaximumAgeRule(65)
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("LifeCoverRules", lifeCoverRules, ExecutionMode.AllPass));

        // Fraud Check Rules
        var fraudCheckRules = new List<IRule<UnderwritingInput>>
        {
            new SuspiciousIncomeRule(),
            new ExcessiveCoverRule(),
            new IncomeVerificationRule()
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("FraudCheckRules", fraudCheckRules, ExecutionMode.FirstFail));
    }
}