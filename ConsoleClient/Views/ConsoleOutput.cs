using System.Globalization;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public static class ConsoleOutput
{
    public static void ShowTitle(string title)
    {
        Console.WriteLine();
        WriteColored($"=== {title} ===", ConsoleColor.Cyan);
    }

    public static void ShowSuccess(string message)
    {
        WriteColored(message, ConsoleColor.Green);
    }

    public static void ShowError(string message)
    {
        WriteColored(message, ConsoleColor.Red);
    }

    public static void ShowInfo(string message)
    {
        WriteColored(message, ConsoleColor.Yellow);
    }

    public static void ShowTableHeader(string header)
    {
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));
    }

    public static string Fit(string? value, int width)
    {
        string text = string.IsNullOrWhiteSpace(value) ? "-" : value;
        return text.Length <= width ? text.PadRight(width) : text.Substring(0, width - 1) + "…";
    }

    public static string OrDash(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }

    public static string FormatDate(DateOnly date)
    {
        return date.ToString("dd.MM.yyyy.", CultureInfo.InvariantCulture);
    }

    public static string FormatAddress(Adresa? adresa)
    {
        return adresa == null ? "-" : $"{adresa.Street} {adresa.Number}, {adresa.City}, {adresa.Country}";
    }

    private static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}