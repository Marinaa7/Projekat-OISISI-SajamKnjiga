using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Adresa : ISerializable
{
    private const int FieldCount = 5;

    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public Adresa()
    {
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            Street,
            Number,
            City,
            Country
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"An address record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "address");

        int id = CsvParser.ParseInt(values[0], nameof(Id));

        Id = id;
        Street = values[1];
        Number = values[2];
        City = values[3];
        Country = values[4];
    }
}