using System.ComponentModel.DataAnnotations;

namespace BettingAgency.Application.Abstraction.Models;

public class Request
{
    [Range(1, 1000)] public int Points { get; set; }

    [Range(0, 9)] public int Number { get; set; }
}