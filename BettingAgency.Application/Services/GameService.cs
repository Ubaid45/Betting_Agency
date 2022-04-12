using AutoMapper;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Common;
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

    public async Task<List<UserDto>> GetAllUsers(CancellationToken ct)
    {
        var users = await _repository.GetAllUsers(ct);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<Response> PlaceBet(Request req, string email, CancellationToken ct)
    {
        var user = await _repository.GetUserDetailsByEmail(email, ct);

        // use must not be allowed to play anymore if the balance have hit zero
        if (user.Balance == 0) throw new Exception("Sorry, you do not have enough balance to play");

        //generate random number
        var randomNumber = new Random().Next(0, Factor);

        // if the guessed number is the random number, the bet is won
        var hasWon = req.GuessNumber == randomNumber;
        
        user.Balance = hasWon ?
            WonTheBet(req.Stake, user.Balance) : LostTheBet(req.Stake, user.Balance);

        await _repository.UpdateUser(user, ct);

        return new Response
        {
            Points = req.Stake,
            AccountBalance = user.Balance,
            Status = hasWon ? StringUtilities.ConvertToString(Status.Won) : StringUtilities.ConvertToString(Status.Lost)
        };
    }

    private int LostTheBet(int stake, int currentBalance)
    {
        var prize = stake * Factor;
        if (currentBalance - prize <= 0)
        {
            return 0;
        }

        currentBalance -= prize;
        
        return currentBalance;
    }

    private int WonTheBet(int stake, int currentBalance)
    {
        var prize = stake * Factor;

         currentBalance += prize;
         
         return currentBalance;
    }

    
}