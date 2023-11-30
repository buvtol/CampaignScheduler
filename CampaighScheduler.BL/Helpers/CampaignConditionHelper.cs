using CampaignScheduler.Domain.Enums;
using CampaignScheduler.Domain.Models;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace CampaighScheduler.Core.Helpers;

public static class CampaignConditionHelper
{
    private static readonly Dictionary<ComparisonOperator, string> ComparisonOperators = new Dictionary<ComparisonOperator, string>
    {
        [ComparisonOperator.Equal] = "==",
        [ComparisonOperator.NotEqual] = "!=",
        [ComparisonOperator.GreaterThan] = ">",
        [ComparisonOperator.LessThan] = "<",
        [ComparisonOperator.GreaterThanOrEqual] = ">=",
        [ComparisonOperator.LessThanOrEqual] = "<="
    };

    public static bool CheckCondition(string serializedCondition, Customer customer)
    {
        var condition = JsonConvert.DeserializeObject<SerializedConditionModel>(serializedCondition);

        if (!ComparisonOperators.TryGetValue(condition.Operator, out var comparisonFunction))
        {
            throw new ArgumentException("Invalid comparison operator.");
        }

        var property = customer.GetType().GetProperty(condition.PropertyName);

        var type = property?.PropertyType;
        var propertyValue = property?.GetValue(customer, null);

        var parsedValue = ParseValue(propertyValue, condition.Value);

        var expression = DynamicExpressionParser.ParseLambda<Customer, bool>(null, true,
            $"{condition.PropertyName} {condition.Operator} @0", parsedValue);

        return expression.Compile()(customer);
    }

    private static object ParseValue(object propertyValue, string conditionValue)
    {
        return propertyValue switch
        {
            int _ => int.Parse(conditionValue),
            DateTime _ => DateTime.Parse(conditionValue),
            _ => conditionValue
        };
    }
}

public class SerializedConditionModel
{
    public string PropertyName { get; set; } = null!;
    public ComparisonOperator Operator { get; set; }
    public string Value { get; set; } = null!;
}