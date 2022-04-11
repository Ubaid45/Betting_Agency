using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using Microsoft.AspNetCore.Mvc;

namespace BettingAgency.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceBet(Request request, CancellationToken ct)
    {
        var res = await _gameService.PlaceBet(request, ct);
        return Ok(res);
    }
}