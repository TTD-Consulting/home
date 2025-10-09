using RulesEngine.Core;
using System.Collections.Concurrent;

namespace RulesEngine.Core;

/// <summary>
/// Thread-safe registry for managing named RuleSets
/// </summary>
/// <typeparam name="T">The type of input the rules evaluate</typeparam>
public class RuleSetRegistry<T> : IRuleSetRegistry<T>
{
    private readonly ConcurrentDictionary<string, RuleSet<T>> _ruleSets = new();

    /// <inheritdoc />
    public void AddRuleSet(RuleSet<T> ruleSet)
    {
        if (ruleSet == null)
            throw new ArgumentNullException(nameof(ruleSet));

        _ruleSets.AddOrUpdate(ruleSet.Name, ruleSet, (key, existing) => ruleSet);
    }

    /// <inheritdoc />
    public RuleSet<T>? GetRuleSet(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        _ruleSets.TryGetValue(name, out var ruleSet);
        return ruleSet;
    }

    /// <inheritdoc />
    public RuleSet<T> GetRequiredRuleSet(string name)
    {
        var ruleSet = GetRuleSet(name);
        if (ruleSet == null)
            throw new KeyNotFoundException($"RuleSet '{name}' not found in registry");

        return ruleSet;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetRuleSetNames()
    {
        return _ruleSets.Keys.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<RuleSet<T>> GetAllRuleSets()
    {
        return _ruleSets.Values.ToList();
    }

    /// <inheritdoc />
    public bool ContainsRuleSet(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return _ruleSets.ContainsKey(name);
    }

    /// <inheritdoc />
    public bool RemoveRuleSet(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return _ruleSets.TryRemove(name, out _);
    }
}