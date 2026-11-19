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
    public int VisitorId { get; set; }
    public int BookId { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public int Rating { get; set; } = MinRating;
    public string Comment { get; set; } = string.Empty;

    public Posetilac? Visitor { get; set; }
    public Knjiga? Book { get; set; }

    public Kupovina()
    {
    }

    public string[] ToCSV()
    {
        if (!IsValidRating(Rating))
        {
            throw new InvalidOperationException($"{nameof(Rating)} must be between {MinRating} and {MaxRating}, but it is {Rating}.");
        }

        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            VisitorId.ToString(CultureInfo.InvariantCulture),
            BookId.ToString(CultureInfo.InvariantCulture),
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
        int visitorId = CsvParser.ParseInt(values[1], nameof(VisitorId));
        int bookId = CsvParser.ParseInt(values[2], nameof(BookId));
        DateOnly purchaseDate = CsvParser.ParseDate(values[3], nameof(PurchaseDate));
        int rating = CsvParser.ParseInt(values[4], nameof(Rating));

        if (!IsValidRating(rating))
        {
            throw new FormatException($"{nameof(Rating)} must be between {MinRating} and {MaxRating}, but it is {rating}.");
        }

        Id = id;
        VisitorId = visitorId;
        BookId = bookId;
        PurchaseDate = purchaseDate;
        Rating = rating;
        Comment = values[5];
    }

    private static bool IsValidRating(int rating)
    {
        return rating >= MinRating && rating <= MaxRating;
    }
}