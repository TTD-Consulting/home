namespace RulesEngine.Core;

public class RuleOutcomeResolver
{
    public DecisionOutcome Resolve(List<RuleExecutionLog> logs)
    {
        if (logs.Any(l => !l.Passed && l.Severity == RuleSeverity.Critical))
            return DecisionOutcome.Reject;

        if (logs.Any(l => !l.Passed && l.Severity == RuleSeverity.Soft))
            return DecisionOutcome.Refer;

        return DecisionOutcome.Approve;
    }
}