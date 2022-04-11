using BettingAgency.Persistence.Abstraction.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BettingAgency.Persistence.Abstraction.Interfaces;

public interface IApiContext
{
    DatabaseFacade Database { get; }

    DbSet<UserEntity> Users { get; set; }

    int SaveChanges();

}