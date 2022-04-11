using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BettingAgency.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ITokenService _tokenService;

    public GameController(IGameService gameService, ITokenService tokenService)
    {
        _gameService = gameService;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> GetToken(UserLogins userLogins, CancellationToken ct)
    {
        var token = _tokenService.GetToken(userLogins, await _gameService.GetAllUsers(ct));
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

    [HttpPost]
    public async Task<IActionResult> PlaceBet(Request request, CancellationToken ct)
    {
        var res = await _gameService.PlaceBet(request, ct);
        return Ok(res);
    }
}