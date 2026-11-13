using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Izdavac : ISerializable
{
    private const int FieldCount = 5;

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int HeadAuthorId { get; set; }
    public List<int> AuthorIds { get; set; } = new List<int>();
    public List<string> BookIsbns { get; set; } = new List<string>();

    public Izdavac()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(Code, nameof(Code));

        string[] csvValues =
        {
            Code,
            Name,
            HeadAuthorId.ToString(CultureInfo.InvariantCulture),
            string.Join(CsvParser.ListSeparator, AuthorIds.Select(id => id.ToString(CultureInfo.InvariantCulture))),
            CsvParser.FormatIdentifierList(BookIsbns, nameof(BookIsbns))
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A publisher record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "publisher");

        string code = CsvParser.ParseIdentifier(values[0], nameof(Code));
        int headAuthorId = CsvParser.ParseInt(values[2], nameof(HeadAuthorId));
        List<int> authorIds = CsvParser.ParseIntList(values[3], nameof(AuthorIds));
        List<string> bookIsbns = CsvParser.ParseIdentifierList(values[4], nameof(BookIsbns));

        Code = code;
        Name = values[1];
        HeadAuthorId = headAuthorId;
        AuthorIds = authorIds;
        BookIsbns = bookIsbns;
    }
}
