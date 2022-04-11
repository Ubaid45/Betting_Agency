using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Persistence.Abstraction.Entities;

namespace BettingAgency.Persistence.Abstraction.Interfaces;

public interface IGameRepository
{
    Task<UserEntity?> GetUserByCredentials(UserLogins user, CancellationToken cancellationToken);
    Task<List<UserEntity?>> GetAllUsers(CancellationToken cancellationToken);
    Task<UserEntity> GetUserDetailsByEmail(string email, CancellationToken cancellationToken);
    Task<UserEntity> UpdateUser(UserEntity user, CancellationToken cancellationToken);
}