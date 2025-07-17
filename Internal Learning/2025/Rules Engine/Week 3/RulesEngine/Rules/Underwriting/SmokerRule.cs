namespace RulesEngine.Rules.Underwriting;

public class SmokerRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.IsSmoker)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = "Smoker applicants require premium loading."
            };
        }

        return new RuleResult { IsSuccessful = true };
    }
}