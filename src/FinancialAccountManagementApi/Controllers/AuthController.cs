using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FinancialAccountManagementApi.Models.Authentication.Request;
using FinancialAccountManagementApi.Models.Authentication.Response;
using FinancialAccountManagementApi.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccountManagementApi.Controllers;

public class AuthController : BaseApiController
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [TranslateResultToActionResult]
    public async Task<Result<TokenDto>> GetTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken)
    {
        return await _tokenService.GetTokenAsync(request.UserName, request.Password, cancellationToken);
    }
}