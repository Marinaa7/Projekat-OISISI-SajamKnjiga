using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Posetilac : ISerializable
{
    private const int FieldCount = 10;
    private const double MinAverageRating = 0;
    private const double MaxAverageRating = 5;

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

        string cardNumber = CsvParser.ParseIdentifier(values[0], nameof(CardNumber));
        DateOnly dateOfBirth = CsvParser.ParseDate(values[3], nameof(DateOfBirth));
        int addressId = CsvParser.ParseInt(values[4], nameof(AddressId));
        int membershipYear = CsvParser.ParseInt(values[7], nameof(MembershipYear));
        MembershipStatus status = ParseStatus(values[8]);
        double averageRating = CsvParser.ParseDouble(values[9], nameof(AverageRating));

        if (!IsValidAverageRating(averageRating))
        {
            throw new FormatException($"{nameof(AverageRating)} must be between {MinAverageRating} and {MaxAverageRating}, but it is '{values[9]}'.");
        }

        CardNumber = cardNumber;
        FirstName = values[1];
        LastName = values[2];
        DateOfBirth = dateOfBirth;
        AddressId = addressId;
        Phone = values[5];
        Email = values[6];
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