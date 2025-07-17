namespace RulesEngine.Rules.Underwriting;

public class CoverToIncomeRatioRule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.RequestedCover > input.Income * 10)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = "Requested cover exceeds 10x the applicant's income."
            };
        }

        return new RuleResult { IsSuccessful = true };
    }
}