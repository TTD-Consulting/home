namespace RulesEngine.Core;

public class RuleExecutionLog
{
    public string RuleName { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}