namespace WeekTwoRulesEngine.Rules;

public interface IRule<T>
{
    RuleResult Evaluate(T input);
}

