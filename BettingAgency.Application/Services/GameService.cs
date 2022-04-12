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

    public async Task<List<UserDto>> GetAllUsers(CancellationToken ct)
    {
        var users = await _repository.GetAllUsers(ct);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<Response> PlaceBet(Request req, string email, CancellationToken ct)
    {
        var user = await _repository.GetUserDetailsByEmail(email, ct);

        var guessNumber = req.Number;
        var stake = req.Points;

        // use must not be allowed to play anymore if the balance have hit zero
        if (user.Balance == 0) throw new Exception("Sorry, you do not have enough balance to play");

        //generate randon number
        var randomNumber = new Random().Next(0, Factor);
        var hasWon = guessNumber == randomNumber;

        if (hasWon)
            WonTheBet(stake, user);
        else
            lostTheBet(stake, user);

        await _repository.UpdateUser(user, ct);

        convertToString(Status.Won);

        return new Response
        {
            Points = stake,
            AccountBalance = user.Balance,
            Status = hasWon ? convertToString(Status.Won) : convertToString(Status.Lost)
        };
    }

    private void lostTheBet(int stake, UserEntity user)
    {
        var prize = stake * Factor;
        if (user.Balance - prize <= 0)
        {
            user.Balance = 0;
            return;
        }

        user.Balance -= prize;
    }

    private void WonTheBet(int stake, UserEntity user)
    {
        var prize = stake * Factor;

        user.Balance += prize;
    }

    public static string convertToString(Enum eff)
    {
        return Enum.GetName(eff.GetType(), eff);
    }
}