using RulesEngine.Core;

namespace RulesEngine.Core;

/// <summary>
/// Represents a named collection of rules that can be executed together with a specific execution mode.
/// </summary>
/// <typeparam name="T">The type of input the rules evaluate</typeparam>
public class RuleSet<T>
{
    public string Name { get; }
    public IReadOnlyList<IRule<T>> Rules { get; }
    public ExecutionMode ExecutionMode { get; }

    public RuleSet(string name, IEnumerable<IRule<T>> rules, ExecutionMode executionMode = ExecutionMode.AllPass)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("RuleSet name cannot be null or empty", nameof(name));
        
        Name = name;
        Rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
        ExecutionMode = executionMode;
    }

    /// <summary>
    /// Evaluates all rules in this RuleSet using an internal RuleEngine
    /// </summary>
    /// <param name="input">The input to evaluate against the rules</param>
    /// <param name="outcomeResolver">Optional outcome resolver for decision making</param>
    /// <returns>The evaluation result</returns>
    public RuleEvaluationResult Evaluate(T input, RuleOutcomeResolver? outcomeResolver = null)
    {
        var engine = new RuleEngine<T>(Rules, outcomeResolver);
        return engine.Evaluate(input, ExecutionMode);
    }

    /// <summary>
    /// Gets the execution logs from the last evaluation
    /// </summary>
    /// <param name="input">The input to evaluate against the rules</param>
    /// <param name="outcomeResolver">Optional outcome resolver for decision making</param>
    /// <returns>The execution logs</returns>
    public List<RuleExecutionLog> GetExecutionLogs(T input, RuleOutcomeResolver? outcomeResolver = null)
    {
        var engine = new RuleEngine<T>(Rules, outcomeResolver);
        engine.Evaluate(input, ExecutionMode);
        return engine.ExecutionLogs;
    }
}