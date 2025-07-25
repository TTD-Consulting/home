namespace WeekTwoRulesEngine.Rules;

public class HealthRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {

        if (input.HealthStatus == "Good")
        {
            return new RuleResult { IsSuccessful = true };
        }
        return new RuleResult { IsSuccessful = false, Message = $"Causionary as {input.HealthStatus}" };
    }
}
