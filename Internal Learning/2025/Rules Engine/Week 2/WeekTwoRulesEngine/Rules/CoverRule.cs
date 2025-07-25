namespace WeekTwoRulesEngine.Rules;

public class CoverRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        decimal maxCover = 0;

        switch (input.Age)
        {
            case < 20:
                maxCover = 0;
                break;

            case >= 20 and < 45:
                maxCover = 1500000;
                break;

            case >= 45 and < 55:
                maxCover = 2500000;
                break;

            case >= 55:
                maxCover = 3500000;
                break;
        }

        if (input.RequestedCover <= maxCover)
        {
            return new RuleResult { IsSuccessful = true };
        }
        return new RuleResult { IsSuccessful = false, Message = $"Max Cover for {input.Age} is  {maxCover}" };
    }
}
