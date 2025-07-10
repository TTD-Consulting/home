namespace WeekTwoRulesEngine.Rules;

public class AgeUnder25Rule : IRule<UnderwritingInput>
{
    public RuleResult Evaluate(UnderwritingInput input)
    {
        if (input.Age < 25)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = "Applicant under 25 requires manual review."
            };
        }

        return new RuleResult { IsSuccessful = true };
    }
}