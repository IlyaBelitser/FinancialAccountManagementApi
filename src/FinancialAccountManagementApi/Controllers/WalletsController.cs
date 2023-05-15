using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Models;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Models.Authentication.Response;
using FinancialAccountManagementApi.Models.Request;
using FinancialAccountManagementApi.Models.Response;
using FinancialAccountManagementApi.Services;
using FinancialAccountManagementApi.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccountManagementApi.Controllers;

public class WalletsController : BaseApiController
{
    private readonly IWalletService _walletService;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WalletsController(IWalletService walletService, IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _walletService = walletService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<IList<WalletDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        return await _walletService.GetListAsync(cancellationToken);
    }
    
    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<WalletDto>> CreateAsync(CreateWalletRequest request, CancellationToken cancellationToken)
    {
        return await _walletService.CreateAsync(request.UserId, Enum.Parse<CurrencyCode>(request.Currency.ToUpper()), cancellationToken);
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<WalletDto>> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _walletService.GetAsync(id, cancellationToken);
    }
    
    [HttpGet("Me")]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result<WalletDto>> GetMeAsync( CancellationToken cancellationToken)
    {
        string? currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        if (currentUserId is null)
        {
            return Result.NotFound("Current User Id Not Found");
        }

        Result<UserDto> result = await _userService.GetAsync("currentUserId", cancellationToken);
        if(!result.IsSuccess)
        {
            return Result.NotFound(result.Errors.ToArray<string>());
        }

        int? walletId = result.Value?.WalletId;
        if (walletId is null)
        {
            return Result.NotFound("Current User's Wallet Not Found");
        }

        return await _walletService.GetAsync((int) walletId, cancellationToken);
    }
    
    [HttpPatch("{id:int}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<WalletDto>> UpdateAsync(int id, decimal amount, CancellationToken cancellationToken)
    {
        return await _walletService.UpdateAsync(id, amount, cancellationToken);
    }
    
    [HttpPatch("Me")]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result<WalletDto>> UpdateMeAsync(decimal amount, CancellationToken cancellationToken)
    {
        string? currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        if (currentUserId is null)
        {
            return Result.NotFound("Current User Id Not Found");
        }

        Result<UserDto> result = await _userService.GetAsync(currentUserId, cancellationToken);
        if(!result.IsSuccess)
        {
            return Result.NotFound(result.Errors.ToArray<string>());
        }
        
        int? walletId = result.Value?.WalletId;
        if (walletId is null)
        {
            return Result.NotFound("Current User's Wallet Not Found");
        }

        return await _walletService.UpdateAsync((int) walletId, amount, cancellationToken);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result> UpdateAsync(int id, CancellationToken cancellationToken)
    {
        return await _walletService.DeleteAsync(id, cancellationToken);
    }
}