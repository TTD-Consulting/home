namespace RulesEngine.Rules.Underwriting;

public class SeniorCitizenRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Age >= 60)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = "Applicants 60 or older require manual review."
            };
        }

        return new RuleResult { IsSuccessful = true };
    }
}