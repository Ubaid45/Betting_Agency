using System.ComponentModel;

namespace BettingAgency.Application.Abstraction.Models;

public enum Status
{
    [Description("Won")] Won,
    [Description("Lost")] Lost
}