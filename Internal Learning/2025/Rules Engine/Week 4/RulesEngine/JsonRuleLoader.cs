using System.Reflection;
using System.Text.Json;

namespace RulesEngine;

public class JsonRuleLoader<T>
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonRuleLoader()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public List<IRule<T>> LoadRulesFromJson(string jsonContent)
    {
        var ruleDefinitions = JsonSerializer.Deserialize<List<RuleDefinition>>(jsonContent, _jsonOptions);
        if (ruleDefinitions == null)
            return new List<IRule<T>>();

        return ruleDefinitions.Select(rd => new DynamicRule<T>(rd)).Cast<IRule<T>>().ToList();
    }

    public List<IRule<T>> LoadRulesFromFile(string filePath)
    {
        var jsonContent = File.ReadAllText(filePath);
        return LoadRulesFromJson(jsonContent);
    }
}

public class DynamicRule<T> : IRule<T>
{
    private readonly RuleDefinition _definition;
    public string Name => $"DynamicRule_{_definition.Field}_{_definition.Operator}_{_definition.Value}";

    public DynamicRule(RuleDefinition definition)
    {
        _definition = definition;
    }

    public RuleResult Evaluate(T input)
    {
        try
        {
            var property = typeof(T).GetProperty(_definition.Field);
            if (property == null)
            {
                return new RuleResult
                {
                    IsSuccessful = false,
                    Message = $"Property '{_definition.Field}' not found on type {typeof(T).Name}"
                };
            }

            var actualValue = property.GetValue(input);
            var expectedValue = ConvertValue(_definition.Value, property.PropertyType);

            bool ruleResult = EvaluateCondition(actualValue, expectedValue, _definition.Operator);

            return new RuleResult
            {
                IsSuccessful = ruleResult,
                Message = ruleResult ? $"{_definition.Field} {_definition.Operator} {_definition.Value}" : _definition.Message
            };
        }
        catch (Exception ex)
        {
            return new RuleResult
            {
                IsSuccessful = false,
                Message = $"Error evaluating rule: {ex.Message}"
            };
        }
    }

    private object ConvertValue(object value, Type targetType)
    {
        if (value == null) return null;

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        if (value is JsonElement jsonElement)
        {
            return ConvertJsonElement(jsonElement, targetType);
        }

        return Convert.ChangeType(value, targetType);
    }

    private object ConvertJsonElement(JsonElement element, Type targetType)
    {
        return targetType.Name switch
        {
            nameof(Int32) => element.GetInt32(),
            nameof(Decimal) => element.GetDecimal(),
            nameof(Double) => element.GetDouble(),
            nameof(Boolean) => element.GetBoolean(),
            nameof(String) => element.GetString(),
            _ => element.GetRawText()
        };
    }

    private bool EvaluateCondition(object actual, object expected, string operatorSymbol)
    {
        if (actual == null || expected == null)
        {
            return operatorSymbol switch
            {
                "==" => actual == expected,
                "!=" => actual != expected,
                _ => false
            };
        }

        if (actual is IComparable comparableActual && expected is IComparable)
        {
            var comparison = comparableActual.CompareTo(expected);

            return operatorSymbol switch
            {
                "==" => comparison == 0,
                "!=" => comparison != 0,
                "<" => comparison < 0,
                "<=" => comparison <= 0,
                ">" => comparison > 0,
                ">=" => comparison >= 0,
                _ => false
            };
        }

        return operatorSymbol switch
        {
            "==" => actual.Equals(expected),
            "!=" => !actual.Equals(expected),
            _ => false
        };
    }
}