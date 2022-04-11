using BettingAgency.Persistence.Abstraction.Entities;

namespace BettingAgency.Persistence.Abstraction.Interfaces;

public interface IGameRepository
{
    Task<List<UserEntity>> GetUsers(CancellationToken cancellationToken);
    Task<UserEntity> GetUserDetails(int id, CancellationToken cancellationToken);
    Task<UserEntity> UpdateUser(UserEntity user, CancellationToken cancellationToken);
}