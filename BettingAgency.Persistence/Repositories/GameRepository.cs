using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BettingAgency.Persistence.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApiContext _context;

    public GameRepository(ApiContext context)
    {
        _context = context;
    }

    public async Task<UserEntity> UpdateUser(UserEntity user, CancellationToken cancellationToken)
    {
        user.Timestamp = DateTime.UtcNow;
        _context.Entry(await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken))
            .CurrentValues.SetValues(user);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUserDetails(user.Id, cancellationToken);
    }

    public async Task<List<UserEntity>> GetUsers(CancellationToken cancellationToken)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<UserEntity> GetUserDetails(int id, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
}