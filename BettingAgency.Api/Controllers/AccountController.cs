using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BettingAgency.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IGameService _gameService;

    public AccountController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<IActionResult> GetToken(UserLogins userLogins, CancellationToken ct)
    {
        var token = await _gameService.GetToken(userLogins, ct);
        return token == null ? BadRequest("wrong password") : Ok(token);
    }

    /// <summary>
    ///     Get List of UserAccounts
    /// </summary>
    /// <returns>List Of UserAccounts</returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        var userList = await _gameService.GetAllUsers(ct);
        return Ok(userList);
    }
}