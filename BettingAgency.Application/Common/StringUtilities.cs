namespace BettingAgency.Application.Common;

public class StringUtilities
{
    public static string ConvertToString(Enum eff)
    {
        return Enum.GetName(eff.GetType(), eff);
    }
}