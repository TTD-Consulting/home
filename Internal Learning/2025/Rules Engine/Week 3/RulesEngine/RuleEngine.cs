namespace RulesEngine;

public class RuleEngine<T>
{
    private readonly IEnumerable<IRule<T>> _rules;
    public List<RuleExecutionLog> ExecutionLogs { get; } = new();

    public RuleEngine(IEnumerable<IRule<T>> rules)
    {
        _rules = rules;
    }

    public RuleEvaluationResult Evaluate(T input, ExecutionMode mode = ExecutionMode.AllPass)
    {
        var result = new RuleEvaluationResult();
        int score = 0;

        foreach (var rule in _rules)
        {
            var ruleResult = rule.Evaluate(input);
            ExecutionLogs.Add(new RuleExecutionLog
            {
                RuleName = rule.GetType().Name,
                Passed = ruleResult.IsSuccessful,
                Message = ruleResult.Message,
                Timestamp = DateTime.UtcNow
            });

            if (!ruleResult.IsSuccessful)
            {
                result.FailedRules.Add(ruleResult);
                result.FailedMessages.Add(ruleResult.Message);

                if (mode == ExecutionMode.FirstFail)
                    break;
            }
            else if (mode == ExecutionMode.Scored)
            {
                score++;
            }
        }

        if (mode == ExecutionMode.Scored)
        {
            result.Score = score;
        }

        return result;
    }
}
