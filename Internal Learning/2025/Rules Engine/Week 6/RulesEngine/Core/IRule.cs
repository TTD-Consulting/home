namespace RulesEngine.Core;

public interface IRule<T>
{
    RuleResult Evaluate(T input);
}