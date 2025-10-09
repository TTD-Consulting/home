using RulesEngine.Core;

namespace RulesEngine.Core;

/// <summary>
/// Interface for managing and retrieving named RuleSets
/// </summary>
/// <typeparam name="T">The type of input the rules evaluate</typeparam>
public interface IRuleSetRegistry<T>
{
    /// <summary>
    /// Adds a RuleSet to the registry
    /// </summary>
    /// <param name="ruleSet">The RuleSet to add</param>
    void AddRuleSet(RuleSet<T> ruleSet);

    /// <summary>
    /// Retrieves a RuleSet by name
    /// </summary>
    /// <param name="name">The name of the RuleSet</param>
    /// <returns>The RuleSet if found, null otherwise</returns>
    RuleSet<T>? GetRuleSet(string name);

    /// <summary>
    /// Retrieves a RuleSet by name, throwing an exception if not found
    /// </summary>
    /// <param name="name">The name of the RuleSet</param>
    /// <returns>The RuleSet</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the RuleSet is not found</exception>
    RuleSet<T> GetRequiredRuleSet(string name);

    /// <summary>
    /// Gets all registered RuleSet names
    /// </summary>
    /// <returns>Collection of RuleSet names</returns>
    IEnumerable<string> GetRuleSetNames();

    /// <summary>
    /// Gets all registered RuleSets
    /// </summary>
    /// <returns>Collection of RuleSets</returns>
    IEnumerable<RuleSet<T>> GetAllRuleSets();

    /// <summary>
    /// Checks if a RuleSet with the specified name exists
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <returns>True if the RuleSet exists, false otherwise</returns>
    bool ContainsRuleSet(string name);

    /// <summary>
    /// Removes a RuleSet from the registry
    /// </summary>
    /// <param name="name">The name of the RuleSet to remove</param>
    /// <returns>True if the RuleSet was removed, false if it didn't exist</returns>
    bool RemoveRuleSet(string name);
}