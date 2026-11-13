using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Kupovina : ISerializable
{
    private const int FieldCount = 6;
    private const int MinRating = 1;
    private const int MaxRating = 5;

    public int Id { get; set; }
    public string VisitorCardNumber { get; set; } = string.Empty;
    public string BookIsbn { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public int Rating { get; set; } = MinRating;
    public string Comment { get; set; } = string.Empty;

    public Kupovina()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(VisitorCardNumber, nameof(VisitorCardNumber));
        CsvParser.EnsureValidIdentifier(BookIsbn, nameof(BookIsbn));

        if (!IsValidRating(Rating))
        {
            throw new InvalidOperationException($"{nameof(Rating)} must be between {MinRating} and {MaxRating}, but it is {Rating}.");
        }

        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            VisitorCardNumber,
            BookIsbn,
            PurchaseDate.ToString(CsvParser.DateFormat, CultureInfo.InvariantCulture),
            Rating.ToString(CultureInfo.InvariantCulture),
            Comment
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A purchase record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "purchase");

        int id = CsvParser.ParseInt(values[0], nameof(Id));
        string visitorCardNumber = CsvParser.ParseIdentifier(values[1], nameof(VisitorCardNumber));
        string bookIsbn = CsvParser.ParseIdentifier(values[2], nameof(BookIsbn));
        DateOnly purchaseDate = CsvParser.ParseDate(values[3], nameof(PurchaseDate));
        int rating = CsvParser.ParseInt(values[4], nameof(Rating));

        if (!IsValidRating(rating))
        {
            throw new FormatException($"{nameof(Rating)} must be between {MinRating} and {MaxRating}, but it is {rating}.");
        }

        Id = id;
        VisitorCardNumber = visitorCardNumber;
        BookIsbn = bookIsbn;
        PurchaseDate = purchaseDate;
        Rating = rating;
        Comment = values[5];
    }

    private static bool IsValidRating(int rating)
    {
        return rating >= MinRating && rating <= MaxRating;
    }
}
