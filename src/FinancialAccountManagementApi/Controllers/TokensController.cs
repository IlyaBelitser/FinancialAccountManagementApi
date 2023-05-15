using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FinancialAccountManagementApi.Models.Authentication.Request;
using FinancialAccountManagementApi.Models.Authentication.Response;
using FinancialAccountManagementApi.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccountManagementApi.Controllers;

public class TokensController : BaseApiController
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [TranslateResultToActionResult]
    public async Task<Result<TokenDto>> GetTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        return await _tokenService.RefreshTokenAsync(request.Jwt, request.RefreshToken, cancellationToken);
    }
}