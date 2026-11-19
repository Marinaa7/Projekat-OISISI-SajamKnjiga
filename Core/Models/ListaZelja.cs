using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class ListaZelja : ISerializable
{
    private const int FieldCount = 2;

    public int VisitorId { get; set; }
    public int BookId { get; set; }

    public ListaZelja()
    {
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            VisitorId.ToString(CultureInfo.InvariantCulture),
            BookId.ToString(CultureInfo.InvariantCulture)
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

        int visitorId = CsvParser.ParseInt(values[0], nameof(VisitorId));
        int bookId = CsvParser.ParseInt(values[1], nameof(BookId));

        VisitorId = visitorId;
        BookId = bookId;
    }
}