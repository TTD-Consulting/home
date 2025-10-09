using RulesEngine.Core;
using RulesEngine.Models;

namespace RulesEngine.Rules;

public class SuspiciousIncomeRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        // Flag unusually high income for young applicants
        if (input.Age < 25 && input.Income > 100000)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Unusually high income {input.Income:C} for age {input.Age} - requires verification" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Income appears normal for age group" };
    }
}

public class ExcessiveCoverRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        // Flag excessive cover amounts that might indicate fraud
        if (input.RequestedCover > 1000000)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Requested cover amount {input.RequestedCover:C} exceeds typical limits - requires additional verification" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Cover amount within normal range" };
    }
}

public class IncomeVerificationRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        // Check for round number incomes which might indicate estimation rather than actual figures
        if (input.Income % 10000 == 0 && input.Income >= 50000)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Income {input.Income:C} appears to be estimated - requires documentation" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Income figure appears to be specific/documented" };
    }
}