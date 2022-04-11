using AutoMapper;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;

namespace BettingAgency.Application.Services;

public class GameService : IGameService
{
    private const int Factor = 9;
    private readonly IMapper _mapper;
    private readonly IGameRepository _repository;

    public GameService(IGameRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<string> PlaceBet(Request req, string email, CancellationToken ct)
    {
        var userEntity = await  _repository.GetUserDetailsByEmail(email, ct);
        var user = _mapper.Map<UserEntity, UserDto>(userEntity);
        var guessNumber = req.Number;
        var stake = req.Points;
        var prize = 0;
        var gewonnen = false;

        var random = new Random();
        var randomNumber = random.Next(0, Factor);

        if (guessNumber == randomNumber)
        {
            prize = stake * Factor;

            //add prize to the database
            user.Balance += prize;
            gewonnen = true;
        }
        else
        {
            prize = stake * Factor;

            //add prize to the database
            user.Balance -= prize;
            gewonnen = false;
        }


        var winningStatement = "Congratulations, You won the bet";
        if (!gewonnen)
        {
            winningStatement = "Unfortunately, you have lost the bet";
        }

        return $"{winningStatement}. The number was :{randomNumber}. You new Balance is: {user.Balance}";
    }


    public async Task<List<UserDto>> GetAllUsers(CancellationToken ct)
    {
        var users = await _repository.GetAllUsers(ct);
        return _mapper.Map<List<UserDto>>(users);
    }
}