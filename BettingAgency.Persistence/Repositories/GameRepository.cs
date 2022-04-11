using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;

namespace BettingAgency.Persistence.Repositories;

public class GameRepository: IGameRepository
{
    private readonly ApiContext _context;

    public GameRepository(ApiContext context)
    {
        _context = context;
    }

    public List<UserEntity> GetUsers()
    {
        return _context.Users.ToList();
    }

    public UserEntity GetUserDetails(int id)
    {
        return _context.Users.FirstOrDefault(m => m.Id == id);
    }
}