using System;
using System.Globalization;

namespace WallyMapSpinzor2;

public static class Utils
{
    // parse functions. if invalid format - error. if null - null.
    public static double? ParseDoubleOrNull(string? s) => (s is null) ? null : double.Parse(s, CultureInfo.InvariantCulture);
    public static float? ParseFloatOrNull(string? s) => (s is null) ? null : float.Parse(s, CultureInfo.InvariantCulture);
    public static bool? ParseBoolOrNull(string? s) => s?.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
    public static int? ParseIntOrNull(string? s) => (s is null) ? null : int.Parse(s, CultureInfo.InvariantCulture);
    public static uint? ParseUIntOrNull(string? s) => (s is null) ? null : (s.StartsWith("0x") ? Convert.ToUInt32(s, 16) : uint.Parse(s, CultureInfo.InvariantCulture));
    public static E? ParseEnumOrNull<E>(string? s) where E : struct, Enum => s is null ? null : Enum.TryParse(s, out E e) ? e : null;
    // give default if null or invalid
    public static E ParseEnumOrDefault<E>(string? s, E @default = default) where E : struct, Enum => Enum.TryParse(s, out E e) ? e : @default;

    public static Color? FromHexOrNull(uint? hex) => (hex is null) ? null : Color.FromHex(hex.Value);

    public static void DrawArrow(this ICanvas canvas, double x1, double y1, double x2, double y2, double arrowSide, double arrowBack, Color color, Transform trans, DrawPriorityEnum priority, object? caller)
    {
        canvas.DrawLine(x1, y1, x2, y2, color, trans, priority, caller);
        // draw arrow parts
        // we start with an arrow pointing right
        // and we rotate it to match
        double length = BrawlhallaMath.Length(x2 - x1, y2 - y1); // arrow length
        (double dirX, double dirY) = BrawlhallaMath.Normalize(x2 - x1, y2 - y1); // arrow direction
        double angle = Math.Atan2(dirY, dirX); // arrow angle
        // calculate end points by applying the rotation to the arrow
        (double arrowEndX1, double arrowEndY1) = BrawlhallaMath.Rotated(length - arrowBack, arrowSide, angle);
        (double arrowEndX2, double arrowEndY2) = BrawlhallaMath.Rotated(length - arrowBack, -arrowSide, angle);
        // draw the lines
        canvas.DrawLine(x2, y2, x1 + arrowEndX1, y1 + arrowEndY1, color, trans, priority, caller);
        canvas.DrawLine(x2, y2, x1 + arrowEndX2, y1 + arrowEndY2, color, trans, priority, caller);
    }

    public static int AS3ParseInt(ReadOnlySpan<char> str)
    {
        if (str.IsEmpty)
            return 0; // NaN

        int i = 0;

        // Skip leading whitespace
        while (i < str.Length && char.IsWhiteSpace(str[i]))
            i++;

        if (i >= str.Length)
            return 0; // NaN

        // Handle sign
        int sign = 1;
        if (str[i] == '+' || str[i] == '-')
        {
            if (str[i] == '-') sign = -1;
            i++;
        }

        if (i >= str.Length)
            return 0; // NaN

        // Detect hex (0x / 0X)
        int radix = 10;
        if (i + 1 < str.Length && str[i] == '0' && (str[i + 1] == 'x' || str[i + 1] == 'X'))
        {
            radix = 16;
            i += 2;
        }

        int result = 0;
        bool foundDigit = false;

        while (i < str.Length)
        {
            char c = str[i];
            int digit;

            if (c >= '0' && c <= '9')
                digit = c - '0';
            else if (c >= 'a' && c <= 'z')
                digit = c - 'a' + 10;
            else if (c >= 'A' && c <= 'Z')
                digit = c - 'A' + 10;
            else
                break;

            if (digit >= radix)
                break;

            foundDigit = true;
            result = result * radix + digit;
            i++;
        }

        if (!foundDigit)
            return 0; // NaN

        return sign * result;
    }
}