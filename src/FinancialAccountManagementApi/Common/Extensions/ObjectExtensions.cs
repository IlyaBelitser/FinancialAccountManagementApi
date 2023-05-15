using System.Text;
using System.Text.Json;

namespace FinancialAccountManagementApi.Common.Extensions;

public static class ObjectExtensions
{    
    public static StringContent ToStringContent(this object obj)
    {
        string serialize = JsonSerializer.Serialize(obj);
        
        return new StringContent(serialize, Encoding.Default, "application/json");
    }
}