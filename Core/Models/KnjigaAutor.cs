using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class KnjigaAutor : ISerializable
{
    private const int FieldCount = 2;

    public int BookId { get; set; }
    public int AuthorId { get; set; }

    public KnjigaAutor()
    {
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            BookId.ToString(CultureInfo.InvariantCulture),
            AuthorId.ToString(CultureInfo.InvariantCulture)
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A book-author record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "book-author");

        int bookId = CsvParser.ParseInt(values[0], nameof(BookId));
        int authorId = CsvParser.ParseInt(values[1], nameof(AuthorId));

        BookId = bookId;
        AuthorId = authorId;
    }
}