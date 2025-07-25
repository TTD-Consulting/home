namespace WeekTwoRulesEngine.Rules;

public class IncomeRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Income > 0)
        {
            return new RuleResult { IsSuccessful = true };
        }
        return new RuleResult { IsSuccessful = false, Message = $"Client has no income" };
    }
}
