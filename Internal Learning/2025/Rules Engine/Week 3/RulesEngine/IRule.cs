namespace RulesEngine;

public interface IRule<T>
{
    RuleResult Evaluate(T input);
}