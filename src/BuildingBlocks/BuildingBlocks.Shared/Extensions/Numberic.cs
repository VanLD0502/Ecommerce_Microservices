
namespace BuildingBlocks.Shared.Extensions;

public static class Numberic
{
    public static long ConvertToLong(this decimal price)
    {
        return (long)price * 100;
    }

    public static decimal ConvertToDecimal(this long price)
    {
        return (decimal)price / 100m;
    }

    public static string ToGrpcString(this decimal number)
    {
        return number.ToString("G29", System.Globalization.CultureInfo.InvariantCulture);
    }
    
    public static decimal FromGrpcString(this string number)
    {
        if (string.IsNullOrEmpty(number))
        {
            return 0;
        }
        
        if (decimal.TryParse(number, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return 0;
    }
}
