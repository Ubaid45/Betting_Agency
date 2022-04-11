using AutoMapper;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Persistence.Abstraction.Entities;

namespace BettingAgency.Application.Common;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, UserEntity>().ReverseMap();
    }
}