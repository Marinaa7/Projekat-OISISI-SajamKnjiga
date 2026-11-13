using System.Globalization;

namespace BookFair.Core.Utils;

public static class CsvParser
{
    public const char ListSeparator = ';';
    public const string DateFormat = "yyyy-MM-dd";
    private static readonly char[] ForbiddenIdentifierCharacters = { ListSeparator, '|', '\r', '\n' };

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

    public static List<int> ParseIntList(string value, string fieldName)
    {
        List<int> result = new List<int>();

        if (value.Length == 0)
        {
            return result;
        }

        foreach (string element in value.Split(ListSeparator))
        {
            result.Add(ParseInt(element, fieldName));
        }

        return result;
    }

    public static List<string> ParseIdentifierList(string value, string fieldName)
    {
        List<string> result = new List<string>();

        if (value.Length == 0)
        {
            return result;
        }

        foreach (string element in value.Split(ListSeparator))
        {
            result.Add(ParseIdentifier(element, fieldName));
        }

        return result;
    }

    public static string FormatIdentifierList(List<string> identifiers, string fieldName)
    {
        foreach (string identifier in identifiers)
        {
            EnsureValidIdentifier(identifier, fieldName);
        }

        return string.Join(ListSeparator, identifiers);
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
