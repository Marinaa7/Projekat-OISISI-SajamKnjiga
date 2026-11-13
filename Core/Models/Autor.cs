using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Autor : ISerializable
{
    private const int FieldCount = 9;

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public int AddressId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdCardNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }

    public Adresa? Address { get; set; }
    public List<Knjiga> Books { get; set; } = new List<Knjiga>();

    public Autor()
    {
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            FirstName,
            LastName,
            DateOfBirth.ToString(CsvParser.DateFormat, CultureInfo.InvariantCulture),
            AddressId.ToString(CultureInfo.InvariantCulture),
            Phone,
            Email,
            IdCardNumber,
            YearsOfExperience.ToString(CultureInfo.InvariantCulture)
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"An author record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "author");

        int id = CsvParser.ParseInt(values[0], nameof(Id));
        DateOnly dateOfBirth = CsvParser.ParseDate(values[3], nameof(DateOfBirth));
        int addressId = CsvParser.ParseInt(values[4], nameof(AddressId));
        int yearsOfExperience = CsvParser.ParseInt(values[8], nameof(YearsOfExperience));

        Id = id;
        FirstName = values[1];
        LastName = values[2];
        DateOfBirth = dateOfBirth;
        AddressId = addressId;
        Phone = values[5];
        Email = values[6];
        IdCardNumber = values[7];
        YearsOfExperience = yearsOfExperience;
    }
}