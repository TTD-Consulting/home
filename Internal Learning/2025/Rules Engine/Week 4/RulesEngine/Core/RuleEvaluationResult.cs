namespace RulesEngine.Core;

public class RuleEvaluationResult
{
    public bool AllPassed => FailedRules.Count == 0;
    public List<string> FailedMessages { get; } = new();
    public List<RuleResult> FailedRules { get; } = new();
    public int Score { get; set; }
}