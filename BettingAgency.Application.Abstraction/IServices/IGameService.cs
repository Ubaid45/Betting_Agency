using BettingAgency.Application.Abstraction.Models;

namespace BettingAgency.Application.Abstraction.IServices;

public interface IGameService
{
    public string PlaceBet(Request req);
}