using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Izdavac : ISerializable
{
    private const int FieldCount = 3;

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int HeadAuthorId { get; set; }

    public Autor? HeadAuthor { get; set; }
    public List<Autor> Authors { get; set; } = new List<Autor>();
    public List<Knjiga> Books { get; set; } = new List<Knjiga>();

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
            HeadAuthorId.ToString(CultureInfo.InvariantCulture)
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

        Code = code;
        Name = values[1];
        HeadAuthorId = headAuthorId;
    }
}