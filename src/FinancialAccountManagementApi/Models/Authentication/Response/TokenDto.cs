namespace FinancialAccountManagementApi.Models.Authentication.Response;

public record TokenDto(
    string Jwt,
    string RefreshToken,
    DateTime RefreshTokenExpiryTime
    );