using System.Drawing;

namespace PumlNet.Options.Styles;

public static class StyleExtensions
{
    public static string ToHex(this Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}