using WeekTwoRulesEngine.Rules;

var input = new UnderwritingInput
{
    Age = 22,
    IsSmoker = true,
    Income = 30000,
    RequestedCover = 250000
};

var rules = new List<IRule<UnderwritingInput>>
{
    new AgeUnder25Rule(),
    new SmokerRule()
};

foreach (var rule in rules)
{
    var result = rule.Evaluate(input);
    Console.WriteLine(result.IsSuccessful
        ? "Rule passed"
        : $"Rule failed: {result.Message}");
}