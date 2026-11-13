using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class KnjigaAutor : ISerializable
{
    private const int FieldCount = 2;

    public string BookIsbn { get; set; } = string.Empty;
    public int AuthorId { get; set; }

    public KnjigaAutor()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(BookIsbn, nameof(BookIsbn));

        string[] csvValues =
        {
            BookIsbn,
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

        string bookIsbn = CsvParser.ParseIdentifier(values[0], nameof(BookIsbn));
        int authorId = CsvParser.ParseInt(values[1], nameof(AuthorId));

        BookIsbn = bookIsbn;
        AuthorId = authorId;
    }
}