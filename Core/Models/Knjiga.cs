using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Knjiga : ISerializable
{
    private const int FieldCount = 8;

    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public double Price { get; set; }
    public int PageCount { get; set; }
    public int PublisherId { get; set; }

    public Izdavac? Publisher { get; set; }
    public List<Autor> Authors { get; set; } = new List<Autor>();
    public List<Posetilac> PurchasedBy { get; set; } = new List<Posetilac>();
    public List<Posetilac> WishlistedBy { get; set; } = new List<Posetilac>();

    public Knjiga()
    {
    }

    public string[] ToCSV()
    {
        CsvParser.EnsureValidIdentifier(Isbn, nameof(Isbn));

        if (!double.IsFinite(Price))
        {
            throw new InvalidOperationException($"{nameof(Price)} must be a finite number, but it is {Price.ToString(CultureInfo.InvariantCulture)}.");
        }

        string[] csvValues =
        {
            Id.ToString(CultureInfo.InvariantCulture),
            Isbn,
            Title,
            Genre,
            PublicationYear.ToString(CultureInfo.InvariantCulture),
            Price.ToString(CultureInfo.InvariantCulture),
            PageCount.ToString(CultureInfo.InvariantCulture),
            PublisherId.ToString(CultureInfo.InvariantCulture)
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != FieldCount)
        {
            throw new FormatException($"A book record must have {FieldCount} fields, but it has {values.Length}.");
        }

        CsvParser.EnsureNoNullValues(values, "book");

        int id = CsvParser.ParseInt(values[0], nameof(Id));
        string isbn = CsvParser.ParseIdentifier(values[1], nameof(Isbn));
        int publicationYear = CsvParser.ParseInt(values[4], nameof(PublicationYear));
        double price = CsvParser.ParseDouble(values[5], nameof(Price));
        int pageCount = CsvParser.ParseInt(values[6], nameof(PageCount));
        int publisherId = CsvParser.ParseInt(values[7], nameof(PublisherId));

        Id = id;
        Isbn = isbn;
        Title = values[2];
        Genre = values[3];
        PublicationYear = publicationYear;
        Price = price;
        PageCount = pageCount;
        PublisherId = publisherId;
    }
}