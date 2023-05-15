using System.Text.Json.Serialization;

namespace FinancialAccountManagementApi.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrencyCode
{
    RUB,
    USD,
    EUR,
}