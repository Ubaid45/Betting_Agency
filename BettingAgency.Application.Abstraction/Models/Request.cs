using System.ComponentModel.DataAnnotations;

namespace BettingAgency.Application.Abstraction.Models;

public class Request
{
    [Range(1, 1000)] public int Stake { get; set; }

    [Range(0, 9)] public int GuessNumber { get; set; }
}