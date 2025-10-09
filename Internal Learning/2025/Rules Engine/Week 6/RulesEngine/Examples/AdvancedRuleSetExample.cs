using RulesEngine.Core;
using RulesEngine.Models;
using RulesEngine.Rules;

namespace RulesEngine.Examples;

/// <summary>
/// Advanced example showing how to use RuleSets with scoring and complex evaluation scenarios
/// </summary>
public static class AdvancedRuleSetExample
{
    public static void RunAdvancedDemo()
    {
        Console.WriteLine("Advanced RuleSet Demo - Scoring and Complex Evaluation");
        Console.WriteLine("======================================================");
        Console.WriteLine();

        // Create a more complex applicant
        var applicant = new UnderwritingInput 
        { 
            Age = 32, 
            IsSmoker = false, 
            Income = 65000, 
            RequestedCover = 350000 
        };

        // Create registry
        var registry = new RuleSetRegistry<UnderwritingInput>();

        // Setup different rule sets with different execution modes
        SetupAdvancedRuleSets(registry);

        Console.WriteLine($"Evaluating Applicant: Age {applicant.Age}, Smoker: {applicant.IsSmoker}, " +
                         $"Income: {applicant.Income:C}, Cover: {applicant.RequestedCover:C}");
        Console.WriteLine();

        // Evaluate each rule set
        foreach (var ruleSetName in registry.GetRuleSetNames())
        {
            var ruleSet = registry.GetRequiredRuleSet(ruleSetName);
            var result = ruleSet.Evaluate(applicant);
            var logs = ruleSet.GetExecutionLogs(applicant);

            Console.WriteLine($"RuleSet: {ruleSet.Name} (Mode: {ruleSet.ExecutionMode})");
            Console.WriteLine($"  Result: {(result.AllPassed ? "PASS" : "FAIL")}");
            
            if (ruleSet.ExecutionMode == ExecutionMode.Scored)
            {
                Console.WriteLine($"  Score: {result.Score} out of {ruleSet.Rules.Count} ({(double)result.Score / ruleSet.Rules.Count * 100:F1}%)");
            }

            Console.WriteLine("  Rule Details:");
            foreach (var log in logs)
            {
                var status = log.Passed ? "Pass" : "Fail";
                Console.WriteLine($"    {status} {log.RuleName}: {log.Message}");
            }

            if (!result.AllPassed && result.FailedMessages.Any())
            {
                Console.WriteLine("  Failed Messages:");
                foreach (var message in result.FailedMessages)
                {
                    Console.WriteLine($"    - {message}");
                }
            }

            Console.WriteLine();
        }

        // Demonstrate complex evaluation logic
        DemonstrateComplexEvaluation(registry, applicant);
    }

    private static void SetupAdvancedRuleSets(IRuleSetRegistry<UnderwritingInput> registry)
    {
        // Basic eligibility (must all pass)
        var basicEligibilityRules = new List<IRule<UnderwritingInput>>
        {
            new MaximumAgeRule(65),
            new MinimumIncomeRule(12000)
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("BasicEligibility", basicEligibilityRules, ExecutionMode.AllPass));

        // Risk assessment (scored - the more that pass, the better)
        var riskAssessmentRules = new List<IRule<UnderwritingInput>>
        {
            new AgeUnder25Rule(),       // Passes if NOT under 25 (lower risk)
            new SmokerRule(),           // Passes if NOT a smoker
            new CoverToIncomeRatioRule(8.0m)  // Conservative ratio
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("RiskAssessment", riskAssessmentRules, ExecutionMode.Scored));

        // Fraud detection (first fail - stop on first suspicious item)
        var fraudDetectionRules = new List<IRule<UnderwritingInput>>
        {
            new SuspiciousIncomeRule(),
            new ExcessiveCoverRule(),
            new IncomeVerificationRule()
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("FraudDetection", fraudDetectionRules, ExecutionMode.FirstFail));

        // Premium adjustment factors (all evaluated for pricing)
        var premiumFactorRules = new List<IRule<UnderwritingInput>>
        {
            new SmokerRule(),           // Affects premium
            new AgeUnder25Rule()        // Young driver discount/penalty
        };
        registry.AddRuleSet(new RuleSet<UnderwritingInput>("PremiumFactors", premiumFactorRules, ExecutionMode.AllPass));
    }

    private static void DemonstrateComplexEvaluation(IRuleSetRegistry<UnderwritingInput> registry, UnderwritingInput applicant)
    {
        Console.WriteLine("Complex Evaluation Logic:");
        Console.WriteLine(new string('-', 40));

        // Step 1: Check basic eligibility (must pass)
        var basicEligibility = registry.GetRequiredRuleSet("BasicEligibility").Evaluate(applicant);
        if (!basicEligibility.AllPassed)
        {
            Console.WriteLine("Application REJECTED - Failed basic eligibility");
            return;
        }
        Console.WriteLine("Basic eligibility: PASSED");

        // Step 2: Check for fraud (must pass)
        var fraudDetection = registry.GetRequiredRuleSet("FraudDetection").Evaluate(applicant);
        if (!fraudDetection.AllPassed)
        {
            Console.WriteLine("Application REFERRED - Potential fraud detected");
            return;
        }
        Console.WriteLine("Fraud detection: PASSED");

        // Step 3: Risk assessment (scored)
        var riskAssessment = registry.GetRequiredRuleSet("RiskAssessment").Evaluate(applicant);
        var riskScore = (double)riskAssessment.Score / registry.GetRequiredRuleSet("RiskAssessment").Rules.Count;
        
        Console.WriteLine($"Risk assessment score: {riskAssessment.Score}/{registry.GetRequiredRuleSet("RiskAssessment").Rules.Count} ({riskScore * 100:F1}%)");

        // Step 4: Determine final decision based on risk score
        string finalDecision;
        string reasoning;

        if (riskScore >= 0.8)
        {
            finalDecision = "APPROVED - Standard Terms";
            reasoning = "Low risk profile";
        }
        else if (riskScore >= 0.6)
        {
            finalDecision = "APPROVED - Modified Terms";
            reasoning = "Moderate risk profile - adjusted premium";
        }
        else if (riskScore >= 0.4)
        {
            finalDecision = "REFERRED";
            reasoning = "Higher risk profile - manual review required";
        }
        else
        {
            finalDecision = "REJECTED";
            reasoning = "High risk profile";
        }

        Console.WriteLine($" Final Decision: {finalDecision}");
        Console.WriteLine($"   Reasoning: {reasoning}");

        // Step 5: Calculate premium factors
        var premiumFactors = registry.GetRequiredRuleSet("PremiumFactors").Evaluate(applicant);
        Console.WriteLine($"Premium factors to consider: {premiumFactors.FailedRules.Count} risk factors identified");
    }
}