using BettingAgency.Application.Abstraction.Models;
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

    public async Task<UserEntity?> GetUserByCredentials(UserLogins user, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) &&
                       x.Password.Equals(user.Password), cancellationToken: cancellationToken);
 
    }
    public async Task<UserEntity> UpdateUser(UserEntity user, CancellationToken cancellationToken)
    {
        user.Timestamp = DateTime.UtcNow;
        _context.Entry(await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken))
            .CurrentValues.SetValues(user);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUserDetailsByEmail(user.Email, cancellationToken);
    }

    public async Task<List<UserEntity?>> GetAllUsers(CancellationToken cancellationToken)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<UserEntity> GetUserDetailsByEmail(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(m => m.Email == email, cancellationToken);
    }
}