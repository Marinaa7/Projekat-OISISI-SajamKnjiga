using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Posetilac : ISerializable
{
    private const int FieldCount = 11;
    private const double MinAverageRating = 0;
    private const double MaxAverageRating = 5;

    public int Id { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public int AddressId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MembershipYear { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.R;
    public double AverageRating { get; set; }

    public Adresa? Address { get; set; }
    public List<Kupovina> Purchases { get; set; } = new List<Kupovina>();
    public List<Knjiga> Wishlist { get; set; } = new List<Knjiga>();

    public Posetilac()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(CardNumber, nameof(CardNumber));

        if (!IsValidAverageRating(AverageRating))
        {
            throw new InvalidOperationException($"{nameof(AverageRating)} must be between {MinAverageRating} and {MaxAverageRating}, but it is {AverageRating.ToString(CultureInfo.InvariantCulture)}.");
        }

        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            CardNumber,
            FirstName,
            LastName,
            DateOfBirth.ToString(CsvParser.DateFormat, CultureInfo.InvariantCulture),
            AddressId.ToString(CultureInfo.InvariantCulture),
            Phone,
            Email,
            MembershipYear.ToString(CultureInfo.InvariantCulture),
            FormatStatus(Status),
            AverageRating.ToString(CultureInfo.InvariantCulture)
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A visitor record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "visitor");

        int id = CsvParser.ParseInt(values[0], nameof(Id));
        string cardNumber = CsvParser.ParseIdentifier(values[1], nameof(CardNumber));
        DateOnly dateOfBirth = CsvParser.ParseDate(values[4], nameof(DateOfBirth));
        int addressId = CsvParser.ParseInt(values[5], nameof(AddressId));
        int membershipYear = CsvParser.ParseInt(values[8], nameof(MembershipYear));
        MembershipStatus status = ParseStatus(values[9]);
        double averageRating = CsvParser.ParseDouble(values[10], nameof(AverageRating));

        if (!IsValidAverageRating(averageRating))
        {
            throw new FormatException($"{nameof(AverageRating)} must be between {MinAverageRating} and {MaxAverageRating}, but it is '{values[10]}'.");
        }

        Id = id;
        CardNumber = cardNumber;
        FirstName = values[2];
        LastName = values[3];
        DateOfBirth = dateOfBirth;
        AddressId = addressId;
        Phone = values[6];
        Email = values[7];
        MembershipYear = membershipYear;
        Status = status;
        AverageRating = averageRating;
    }

    private static bool IsValidAverageRating(double averageRating)
    {
        return averageRating >= MinAverageRating && averageRating <= MaxAverageRating;
    }

    private static string FormatStatus(MembershipStatus status)
    {
        return status switch
        {
            MembershipStatus.R => nameof(MembershipStatus.R),
            MembershipStatus.V => nameof(MembershipStatus.V),
            _ => throw new InvalidOperationException($"Invalid value for {nameof(Status)}: {status}.")
        };
    }

    private static MembershipStatus ParseStatus(string value)
    {
        return value switch
        {
            nameof(MembershipStatus.R) => MembershipStatus.R,
            nameof(MembershipStatus.V) => MembershipStatus.V,
            _ => throw new FormatException($"Invalid value for {nameof(Status)}: '{value}'. Expected {nameof(MembershipStatus.R)} or {nameof(MembershipStatus.V)}.")
        };
    }
}