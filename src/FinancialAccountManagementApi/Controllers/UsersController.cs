using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Ardalis.Result.FluentValidation;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Models.Authentication.Request;
using FinancialAccountManagementApi.Models.Authentication.Response;
using FinancialAccountManagementApi.Services.Authentication;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccountManagementApi.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IValidator<UpdateRolesRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsersController(IUserService userService, IRoleService roleService, IValidator<UpdateRolesRequest> validator, 
        IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _roleService = roleService;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<IList<UserDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        return await _userService.GetListAsync(cancellationToken);
    }
    
    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request)
    {
        return await _userService.CreateAsync(request.Adapt<UserDto>(), request.Password);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<UserDto>> GetAsync(string id, CancellationToken cancellationToken)
    {
        return await _userService.GetAsync(id, cancellationToken);
    }
    
    [HttpGet("Me")]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result<UserDto>> GetMeAsync(CancellationToken cancellationToken)
    {
        string? currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        if (currentUserId is null)
        {
            return Result.NotFound("Current User Id Not Found");
        }
               
        return await _userService.GetAsync(currentUserId, cancellationToken);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result> DeleteAsync(string id)
    {
        return await _userService.DeleteAsync(id);
    }
    
    [HttpGet("{id}/Roles")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result<IList<string>>> GetRolesAsync(string id)
    {
        return await _roleService.GetAsync(id);
    }
    
    [HttpGet("Me/Roles")]
    [Authorize]
    [TranslateResultToActionResult]
    public async Task<Result<IList<string>>> GetMeRolesAsync()
    {
        string? currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        if (currentUserId is null)
        {
            return Result.NotFound("Current User Id Not Found");
        }
                
        return await _roleService.GetAsync(currentUserId);
    }
    
    [HttpPut("{id}/Roles")]
    [Authorize(Roles = nameof(Role.Admin))]
    [TranslateResultToActionResult]
    public async Task<Result> UpdateAsync(string id, [CustomizeValidator(Skip=true)] UpdateRolesRequest request, 
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }
        
        return await _roleService.UpdateAsync(id, request.Roles, cancellationToken);
    }
}