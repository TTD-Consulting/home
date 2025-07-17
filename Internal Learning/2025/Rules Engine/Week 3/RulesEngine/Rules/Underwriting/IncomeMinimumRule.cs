namespace RulesEngine.Rules.Underwriting;

public class IncomeMinimumRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Income < 10000)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = "Applicant income below minimum threshold."
            };
        }

        return new RuleResult { IsSuccessful = true };
    }
}