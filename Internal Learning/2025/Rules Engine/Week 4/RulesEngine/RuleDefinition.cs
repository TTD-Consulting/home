using System.Text.Json.Serialization;

namespace RulesEngine;

public class RuleDefinition
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
    
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public object Value { get; set; } = new();
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}