namespace RulesEngine.Rules.Underwriting;

public class UnderwritingInput
{
    public int Age { get; set; }
    public bool IsSmoker { get; set; }
    public decimal Income { get; set; }
    public decimal RequestedCover { get; set; }
}