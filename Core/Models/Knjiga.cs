using System.Globalization;
using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Models;

public class Knjiga : ISerializable
{
    private const int FieldCount = 7;

    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public double Price { get; set; }
    public int PageCount { get; set; }
    public string PublisherCode { get; set; } = string.Empty;

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
        CsvParser.EnsureValidIdentifier(PublisherCode, nameof(PublisherCode));

        string[] csvValues =
        {
            Isbn,
            Title,
            Genre,
            PublicationYear.ToString(CultureInfo.InvariantCulture),
            Price.ToString(CultureInfo.InvariantCulture),
            PageCount.ToString(CultureInfo.InvariantCulture),
            PublisherCode
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

        string isbn = CsvParser.ParseIdentifier(values[0], nameof(Isbn));
        int publicationYear = CsvParser.ParseInt(values[3], nameof(PublicationYear));
        double price = CsvParser.ParseDouble(values[4], nameof(Price));
        int pageCount = CsvParser.ParseInt(values[5], nameof(PageCount));
        string publisherCode = CsvParser.ParseIdentifier(values[6], nameof(PublisherCode));

        Isbn = isbn;
        Title = values[1];
        Genre = values[2];
        PublicationYear = publicationYear;
        Price = price;
        PageCount = pageCount;
        PublisherCode = publisherCode;
    }
}