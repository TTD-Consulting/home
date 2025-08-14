namespace RulesEngine;

public class RuleExecutionLog
{
    public string RuleName { get; set; }
    public bool Passed { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}