using RulesEngine.Core;
using RulesEngine.Configuration;

namespace RulesEngine;

public class RuleEngine<T>
{
    private readonly IEnumerable<IRule<T>> _rules;
    private readonly RuleOutcomeResolver? _outcomeResolver;
    public List<RuleExecutionLog> ExecutionLogs { get; } = new();

    public RuleEngine(IEnumerable<IRule<T>> rules, RuleOutcomeResolver? outcomeResolver = null)
    {
        _rules = rules;
        _outcomeResolver = outcomeResolver;
    }

    public RuleEvaluationResult Evaluate(T input, ExecutionMode mode = ExecutionMode.AllPass)
    {
        var result = new RuleEvaluationResult();
        int score = 0;
        ExecutionLogs.Clear(); // Clear previous logs

        foreach (var rule in _rules)
        {
            var ruleResult = rule.Evaluate(input);
            
            string ruleName = rule.GetType().Name;
            RuleSeverity severity = RuleSeverity.Soft; // default
            
            if (rule is DynamicRule<T> dynamicRule)
            {
                ruleName = dynamicRule.Name;
                severity = dynamicRule.Severity;
            }
            
            ExecutionLogs.Add(new RuleExecutionLog
            {
                RuleName = ruleName,
                Passed = ruleResult.IsSuccessful,
                Message = ruleResult.Message,
                Severity = severity,
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

        // Automatically resolve decision outcome if resolver is available
        if (_outcomeResolver != null)
        {
            result.DecisionOutcome = _outcomeResolver.Resolve(ExecutionLogs);
        }

        return result;
    }

    public DecisionOutcome GetDecisionOutcome()
    {
        if (_outcomeResolver == null)
            throw new InvalidOperationException("RuleOutcomeResolver not configured. Use dependency injection or provide resolver in constructor.");
        
        return _outcomeResolver.Resolve(ExecutionLogs);
    }
}