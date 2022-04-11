using AutoMapper;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;
using BettingAgency.Persistence.Abstraction.Interfaces;

namespace BettingAgency.Application.Services;

public class GameService : IGameService
{
    private const int Factor = 9;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    private readonly IGameRepository _repository;

    private readonly ITokenService _tokenService;
    private int _accountBalance = 10000;

    public GameService(IGameRepository repository, JwtSettings jwtSettings,
        IMapper mapper, ITokenService tokenService)
    {
        _repository = repository;
        _jwtSettings = jwtSettings;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<string> PlaceBet(Request req, CancellationToken ct)
    {
        var users = await _repository.GetUsers(ct);
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
            _accountBalance += prize;
            gewonnen = true;
        }
        else
        {
            prize = stake * Factor;

            //add prize to the database
            _accountBalance -= prize;
            gewonnen = false;
        }


        var winningStatement = "Congratulations, You won the bet";
        if (!gewonnen)
        {
            winningStatement = "Unfortunately, you have lost the bet";
        }

        return $"{winningStatement}. The number was :{randomNumber}. You new Balance is: {_accountBalance}";
    }
    

    public async Task<List<UserDto>> GetAllUsers(CancellationToken ct)
    {
        var users = await _repository.GetUsers(ct);
        return _mapper.Map<List<UserDto>>(users);
    }
}