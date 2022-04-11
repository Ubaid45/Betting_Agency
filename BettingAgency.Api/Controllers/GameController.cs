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
    public IActionResult PlaceBet(Request request)
    {
       var res = _gameService.PlaceBet(request);
        return Ok(res);
    }
}