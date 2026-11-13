using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class ListaZelja : ISerializable
{
    private const int FieldCount = 2;

    public string VisitorCardNumber { get; set; } = string.Empty;
    public string BookIsbn { get; set; } = string.Empty;

    public ListaZelja()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(VisitorCardNumber, nameof(VisitorCardNumber));
        CsvParser.EnsureValidIdentifier(BookIsbn, nameof(BookIsbn));

        string[] csvValues =
        {
            VisitorCardNumber,
            BookIsbn
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A wishlist record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "wishlist");

        string visitorCardNumber = CsvParser.ParseIdentifier(values[0], nameof(VisitorCardNumber));
        string bookIsbn = CsvParser.ParseIdentifier(values[1], nameof(BookIsbn));

        VisitorCardNumber = visitorCardNumber;
        BookIsbn = bookIsbn;
    }
}