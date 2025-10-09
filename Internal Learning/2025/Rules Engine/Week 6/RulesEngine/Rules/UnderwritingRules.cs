using RulesEngine.Core;
using RulesEngine.Models;

namespace RulesEngine.Rules;

public class AgeUnder25Rule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Age < 25)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Age {input.Age} is under 25 - higher risk category" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Age requirement met" };
    }
}

public class SmokerRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.IsSmoker)
        {
            return new RuleResult { IsSuccessful = false, Message = "Smoker status increases risk profile" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Non-smoker status acceptable" };
    }
}

public class MinimumIncomeRule : IRule<UnderwritingInput>
{
    private readonly decimal _minimumIncome;

    public MinimumIncomeRule(decimal minimumIncome = 15000)
    {
        _minimumIncome = minimumIncome;
    }

    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Income < _minimumIncome)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Income {input.Income:C} is below minimum requirement of {_minimumIncome:C}" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Income requirement met" };
    }
}

public class CoverToIncomeRatioRule : IRule<UnderwritingInput>
{
    private readonly decimal _maxRatio;

    public CoverToIncomeRatioRule(decimal maxRatio = 10.0m)
    {
        _maxRatio = maxRatio;
    }

    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Income <= 0)
        {
            return new RuleResult { IsSuccessful = false, Message = "Income must be greater than zero for ratio calculation" };
        }

        var ratio = input.RequestedCover / input.Income;
        if (ratio > _maxRatio)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Cover to income ratio {ratio:F1}:1 exceeds maximum allowed ratio of {_maxRatio}:1" };
        }

        return new RuleResult { IsSuccessful = true, Message = $"Cover to income ratio {ratio:F1}:1 is acceptable" };
    }
}

public class MaximumAgeRule : IRule<UnderwritingInput>
{
    private readonly int _maximumAge;

    public MaximumAgeRule(int maximumAge = 65)
    {
        _maximumAge = maximumAge;
    }

    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Age > _maximumAge)
        {
            return new RuleResult { IsSuccessful = false, Message = $"Age {input.Age} exceeds maximum age limit of {_maximumAge}" };
        }

        return new RuleResult { IsSuccessful = true, Message = "Age within acceptable range" };
    }
}