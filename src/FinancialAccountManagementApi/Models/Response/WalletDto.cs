namespace FinancialAccountManagementApi.Models.Response;

public record WalletDto(
    int Id,
    decimal Balance,
    CurrencyCode Currency,
    DateTime CreateDate,
    string UserId
    );