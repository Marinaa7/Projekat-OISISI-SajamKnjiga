// Po uzoru na vezbe 3 (CRUDExample, ConsoleViewUtils.cs)
using System.Globalization;
using System.Net.Mail;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public static class ConsoleInput
{
    private const char ForbiddenCharacter = '|';
    private const string AllowedPhoneCharacters = "0123456789+-/() ";
    private static readonly string[] DateFormats = { "d.M.yyyy", "dd.MM.yyyy" };

    public static string ReadLine()
    {
        string? line = Console.ReadLine();
        if (line == null)
        {
            Console.WriteLine();
            Console.WriteLine("Unos je završen. Program se zatvara.");
            Environment.Exit(0);
        }

        return line.Trim();
    }

    public static string ReadChoice()
    {
        Console.Write("Izaberite opciju: ");
        return ReadLine();
    }

    public static string ReadText(string label, bool required = true, string? current = null)
    {
        while (true)
        {
            Console.Write(FormatLabel(label, current));
            string input = ReadLine();

            if (input.Length == 0 && current != null)
            {
                return current;
            }

            if (input.Length == 0 && required)
            {
                ConsoleOutput.ShowError("Ovo polje je obavezno.");
                continue;
            }

            if (input.Contains(ForbiddenCharacter))
            {
                ConsoleOutput.ShowError($"Znak '{ForbiddenCharacter}' nije dozvoljen.");
                continue;
            }

            return input;
        }
    }

    public static string ReadPhone(string label, string? current = null)
    {
        while (true)
        {
            string input = ReadText(label, false, current);

            if (input.All(c => AllowedPhoneCharacters.Contains(c)))
            {
                return input;
            }

            ConsoleOutput.ShowError("Telefon može sadržati samo cifre, razmak i znakove + - / ( ).");
        }
    }

    public static string ReadEmail(string label, string? current = null)
    {
        while (true)
        {
            string input = ReadText(label, false, current);

            if (input.Length == 0 || (MailAddress.TryCreate(input, out MailAddress? address) && address.Address == input))
            {
                return input;
            }

            ConsoleOutput.ShowError("Unesite ispravnu e-mejl adresu, npr. ime@primer.com.");
        }
    }

    public static int ReadInt(string label, int min, int max, int? current = null)
    {
        while (true)
        {
            Console.Write(FormatLabel(label, current?.ToString(CultureInfo.InvariantCulture)));
            string input = ReadLine();

            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) && value >= min && value <= max)
            {
                return value;
            }

            ConsoleOutput.ShowError($"Unesite cijeli broj od {min} do {max}.");
        }
    }

    public static double ReadDouble(string label, double min, double max, double? current = null)
    {
        while (true)
        {
            Console.Write(FormatLabel(label, current?.ToString(CultureInfo.InvariantCulture)));
            string input = ReadLine();

            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }

            string normalized = input.Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && double.IsFinite(value) && value >= min && value <= max)
            {
                return value;
            }

            ConsoleOutput.ShowError($"Unesite broj od {min.ToString(CultureInfo.InvariantCulture)} do {max.ToString(CultureInfo.InvariantCulture)}.");
        }
    }

    public static DateOnly ReadDate(string label, DateOnly? current = null, bool allowFuture = true)
    {
        while (true)
        {
            Console.Write(FormatLabel(label + " (dd.MM.gggg.)", current == null ? null : ConsoleOutput.FormatDate(current.Value)));
            string input = ReadLine().TrimEnd('.');

            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }

            if (!DateOnly.TryParseExact(input, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly value))
            {
                ConsoleOutput.ShowError("Unesite ispravan datum, npr. 17.05.2000.");
                continue;
            }

            if (!allowFuture && value > DateOnly.FromDateTime(DateTime.Today))
            {
                ConsoleOutput.ShowError("Datum ne može biti u budućnosti.");
                continue;
            }

            return value;
        }
    }

    public static MembershipStatus ReadStatus(string label, MembershipStatus? current = null)
    {
        while (true)
        {
            Console.Write(FormatLabel(label + " (R - redovan, V - VIP)", current?.ToString()));
            string input = ReadLine().ToUpperInvariant();

            if (input.Length == 0 && current != null)
            {
                return current.Value;
            }

            if (input == "R")
            {
                return MembershipStatus.R;
            }

            if (input == "V")
            {
                return MembershipStatus.V;
            }

            ConsoleOutput.ShowError("Unesite R ili V.");
        }
    }

    public static bool Confirm(string question)
    {
        while (true)
        {
            Console.Write(question + " (d/n): ");
            string input = ReadLine().ToLowerInvariant();

            if (input == "d")
            {
                return true;
            }

            if (input == "n")
            {
                return false;
            }

            ConsoleOutput.ShowError("Unesite d (da) ili n (ne).");
        }
    }

    private static string FormatLabel(string label, string? current)
    {
        return string.IsNullOrEmpty(current) ? $"{label}: " : $"{label} [{current}]: ";
    }
}