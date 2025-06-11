using System.Numerics;

namespace fishingShopProject.Tools.Extensions;

public static class StringExtensions
{
    public static Boolean IsNullOrWhiteSpace(this String? str)
    {
        return string.IsNullOrWhiteSpace(str); 
    }
}
