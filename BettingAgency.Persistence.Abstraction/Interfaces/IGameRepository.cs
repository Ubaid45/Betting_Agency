using BettingAgency.Persistence.Abstraction.Entities;

namespace BettingAgency.Persistence.Abstraction.Interfaces;

public interface IGameRepository
{
    List<UserEntity> GetUsers();
    UserEntity GetUserDetails(int id);
}