using System.Globalization;

namespace BookFair.Core.Utils;

public static class CsvParser
{
    public const string DateFormat = "yyyy-MM-dd";
    private static readonly char[] ForbiddenIdentifierCharacters = { '|', '\r', '\n' };

    public static bool IsValidIdentifier(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.IndexOfAny(ForbiddenIdentifierCharacters) < 0;
    }

    public static void EnsureValidIdentifier(string? value, string fieldName)
    {
        if (!IsValidIdentifier(value))
        {
            throw new InvalidOperationException($"Invalid identifier for {fieldName}: '{value}'.");
        }
    }

    public static string ParseIdentifier(string value, string fieldName)
    {
        if (!IsValidIdentifier(value))
        {
            throw new FormatException($"Invalid identifier for {fieldName}: '{value}'.");
        }

        return value;
    }

    public static int ParseInt(string value, string fieldName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
        {
            throw new FormatException($"Invalid integer value for {fieldName}: '{value}'.");
        }

        return result;
    }

    public static double ParseDouble(string value, string fieldName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
            || !double.IsFinite(result))
        {
            throw new FormatException($"Invalid decimal value for {fieldName}: '{value}'.");
        }

        return result;
    }

    public static DateOnly ParseDate(string value, string fieldName)
    {
        if (!DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result))
        {
            throw new FormatException($"Invalid date for {fieldName}: '{value}'. Expected format is {DateFormat}.");
        }

        return result;
    }

    public static void EnsureNoNullValues(string[] values, string recordName)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == null)
            {
                throw new FormatException($"Field {i} of a {recordName} record is null.");
            }
        }
    }
}