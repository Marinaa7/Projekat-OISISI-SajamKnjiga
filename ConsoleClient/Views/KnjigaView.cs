// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class KnjigaView
{
    private const string AllowedIsbnCharacters = "0123456789-Xx";
    private const double MinPrice = 0.01;
    private const double MaxPrice = 1000000;
    private const int MaxPageCount = 100000;

    private readonly KnjigaDAO _knjigaDao;
    private readonly KnjigaAutorDAO _knjigaAutorDao;
    private readonly AutorDAO _autorDao;
    private readonly IzdavacDAO _izdavacDao;
    private readonly KupovinaDAO _kupovinaDao;
    private readonly PosetilacDAO _posetilacDao;
    private readonly ListaZeljaDAO _listaZeljaDao;

    public KnjigaView(KnjigaDAO knjigaDao, KnjigaAutorDAO knjigaAutorDao, AutorDAO autorDao,
                      IzdavacDAO izdavacDao, KupovinaDAO kupovinaDao, PosetilacDAO posetilacDao,
                      ListaZeljaDAO listaZeljaDao)
    {
        _knjigaDao = knjigaDao;
        _knjigaAutorDao = knjigaAutorDao;
        _autorDao = autorDao;
        _izdavacDao = izdavacDao;
        _kupovinaDao = kupovinaDao;
        _posetilacDao = posetilacDao;
        _listaZeljaDao = listaZeljaDao;
    }

    public void Run()
    {
        string choice;
        do
        {
            ShowMenu();
            choice = ConsoleInput.ReadChoice();

            if (choice != "0")
            {
                ExecuteSafely(choice);
            }
        }
        while (choice != "0");
    }

    private void ExecuteSafely(string choice)
    {
        try
        {
            HandleChoice(choice);
        }
        catch (Exception ex)
        {
            ConsoleOutput.ShowError("Operacija nije izvršena: " + ex.Message);
        }
    }

    private static void ShowMenu()
    {
        ConsoleOutput.ShowTitle("KNJIGE");
        Console.WriteLine("1 - Prikaz svih knjiga");
        Console.WriteLine("2 - Detalji knjige");
        Console.WriteLine("3 - Dodavanje knjige");
        Console.WriteLine("4 - Izmjena knjige");
        Console.WriteLine("5 - Brisanje knjige");
        Console.WriteLine("0 - Nazad");
    }

    private void HandleChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                ShowAll();
                break;
            case "2":
                ShowDetails();
                break;
            case "3":
                Add();
                break;
            case "4":
                Update();
                break;
            case "5":
                Remove();
                break;
            default:
                ConsoleOutput.ShowError("Nepostojeća opcija.");
                break;
        }
    }

    private void ShowAll()
    {
        List<Knjiga> knjige = _knjigaDao.GetAll();
        if (knjige.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema knjiga.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"ISBN",-17} | {"Naziv",-26} | {"Cijena",10} | {"Godina",6} | {"Žanr",-14} | {"Izdavač",-18}");
        foreach (Knjiga knjiga in knjige)
        {
            Console.WriteLine($"{ConsoleOutput.Fit(knjiga.Isbn, 17)} | " +
                              $"{ConsoleOutput.Fit(knjiga.Title, 26)} | " +
                              $"{knjiga.Price,10:0.00} | " +
                              $"{knjiga.PublicationYear,6} | " +
                              $"{ConsoleOutput.Fit(knjiga.Genre, 14)} | " +
                              $"{ConsoleOutput.Fit(PublisherName(knjiga.PublisherCode), 18)}");
        }

        Console.WriteLine($"Ukupno knjiga: {knjige.Count}");
    }

    private void ShowDetails()
    {
        Knjiga? knjiga = FindByIsbn();
        if (knjiga is null)
        {
            return;
        }

        List<Autor> autori = AuthorsOf(knjiga.Isbn);
        List<Kupovina> kupovine = PurchasesOf(knjiga.Isbn);
        List<string> wishedBy = _listaZeljaDao.GetVisitorCardNumbers(knjiga.Isbn);

        Console.WriteLine($"ISBN:            {knjiga.Isbn}");
        Console.WriteLine($"Naziv:           {knjiga.Title}");
        Console.WriteLine($"Žanr:            {knjiga.Genre}");
        Console.WriteLine($"Godina izdanja:  {knjiga.PublicationYear}");
        Console.WriteLine($"Cijena:          {knjiga.Price:0.00}");
        Console.WriteLine($"Broj strana:     {knjiga.PageCount}");
        Console.WriteLine($"Izdavač:         {knjiga.PublisherCode} ({PublisherName(knjiga.PublisherCode)})");

        Console.WriteLine($"Autori ({autori.Count}):");
        autori.ForEach(a => Console.WriteLine($"  {a.Id,3} - {a.FirstName} {a.LastName}"));

        Console.WriteLine($"Kupili ({kupovine.Count}):");
        kupovine.ForEach(k => Console.WriteLine($"  {k.VisitorCardNumber} - {VisitorName(k.VisitorCardNumber)}"));

        Console.WriteLine($"Na listi želja ({wishedBy.Count}):");
        wishedBy.ForEach(cardNumber => Console.WriteLine($"  {cardNumber} - {VisitorName(cardNumber)}"));
    }

    private void Add()
    {
        if (!CanEditBooks())
        {
            return;
        }

        string isbn = ReadIsbn();
        if (_knjigaDao.GetByIsbn(isbn) is not null)
        {
            ConsoleOutput.ShowError($"Knjiga sa ISBN-om '{isbn}' već postoji.");
            return;
        }

        Knjiga knjiga = ReadKnjiga(isbn, null);
        List<int> authorIds = ReadAuthorIds(null);

        _knjigaDao.Add(knjiga);
        _knjigaAutorDao.SetAuthors(isbn, authorIds);
        ConsoleOutput.ShowSuccess("Knjiga je uspješno dodata.");
    }

    private void Update()
    {
        if (!CanEditBooks())
        {
            return;
        }

        Knjiga? existing = FindByIsbn();
        if (existing is null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Pritisnite Enter da zadržite trenutnu vrijednost (prikazana je u zagradi).");
        Knjiga updated = ReadKnjiga(existing.Isbn, existing);
        List<int> authorIds = ReadAuthorIds(_knjigaAutorDao.GetAuthorIds(existing.Isbn));

        _knjigaDao.Update(updated);
        _knjigaAutorDao.SetAuthors(existing.Isbn, authorIds);
        ConsoleOutput.ShowSuccess("Podaci o knjizi su uspješno izmijenjeni.");
    }

    private void Remove()
    {
        Knjiga? knjiga = FindByIsbn();
        if (knjiga is null)
        {
            return;
        }

        int purchaseCount = PurchasesOf(knjiga.Isbn).Count;
        if (purchaseCount > 0)
        {
            ConsoleOutput.ShowError($"Knjiga '{knjiga.Title}' ima kupovina: {purchaseCount}. Ne može se obrisati dok se te kupovine ne obrišu u meniju Kupovine.");
            return;
        }

        int wishlistCount = _listaZeljaDao.GetVisitorCardNumbers(knjiga.Isbn).Count;
        string question = wishlistCount == 0
            ? $"Da li ste sigurni da želite da obrišete knjigu '{knjiga.Title}'?"
            : $"Knjiga '{knjiga.Title}' je na listi želja kod posjetilaca: {wishlistCount} i biće uklonjena sa tih lista. Da li ste sigurni?";

        if (!ConsoleInput.Confirm(question))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        _listaZeljaDao.RemoveByBook(knjiga.Isbn);
        _knjigaAutorDao.RemoveByBook(knjiga.Isbn);
        _knjigaDao.Remove(knjiga.Isbn);
        ConsoleOutput.ShowSuccess("Knjiga je uspješno obrisana.");
    }

    private bool CanEditBooks()
    {
        if (_izdavacDao.GetAll().Count == 0)
        {
            ConsoleOutput.ShowError("U sistemu nema izdavača. Prvo dodajte izdavača u meniju Izdavači.");
            return false;
        }

        if (_autorDao.GetAll().Count == 0)
        {
            ConsoleOutput.ShowError("U sistemu nema autora. Prvo dodajte autora u meniju Autori.");
            return false;
        }

        return true;
    }

    private Knjiga ReadKnjiga(string isbn, Knjiga? current)
    {
        return new Knjiga
        {
            Isbn = isbn,
            Title = ConsoleInput.ReadText("Naziv", true, current?.Title),
            Genre = ConsoleInput.ReadText("Žanr", true, current?.Genre),
            PublicationYear = ConsoleInput.ReadInt("Godina izdanja", 1, DateTime.Today.Year, current?.PublicationYear),
            Price = ConsoleInput.ReadDouble("Cijena", MinPrice, MaxPrice, current?.Price),
            PageCount = ConsoleInput.ReadInt("Broj strana", 1, MaxPageCount, current?.PageCount),
            PublisherCode = ChoosePublisher(current?.PublisherCode)
        };
    }

    private static string ReadIsbn()
    {
        while (true)
        {
            string isbn = ConsoleInput.ReadText("ISBN (npr. 978-86-521-1234-5)");
            if (isbn.All(c => AllowedIsbnCharacters.Contains(c)) && isbn.Any(char.IsDigit))
            {
                return isbn;
            }

            ConsoleOutput.ShowError("ISBN može sadržati samo cifre, crtice i slovo X.");
        }
    }

    private string ChoosePublisher(string? current)
    {
        Console.WriteLine("Izdavači:");
        _izdavacDao.GetAll().ForEach(i => Console.WriteLine($"  {i.Code} - {i.Name}"));

        while (true)
        {
            string code = ConsoleInput.ReadText("Šifra izdavača", true, current);
            if (_izdavacDao.GetByCode(code) is not null)
            {
                return code;
            }

            ConsoleOutput.ShowError($"Izdavač sa šifrom '{code}' ne postoji.");
        }
    }

    private List<int> ReadAuthorIds(List<int>? current)
    {
        Console.WriteLine("Autori:");
        _autorDao.GetAll().ForEach(a => Console.WriteLine($"  {a.Id,3} - {a.FirstName} {a.LastName}"));

        string? currentText = current is null || current.Count == 0 ? null : string.Join(", ", current);
        while (true)
        {
            string input = ConsoleInput.ReadText("ID-jevi autora, odvojeni zarezom (npr. 1, 3)", true, currentText);
            List<int>? authorIds = ParseAuthorIds(input);
            if (authorIds is not null)
            {
                return authorIds;
            }
        }
    }

    private List<int>? ParseAuthorIds(string input)
    {
        List<int> authorIds = new List<int>();

        foreach (string part in input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(part, out int id) || _autorDao.GetById(id) is null)
            {
                ConsoleOutput.ShowError($"Autor sa ID-jem '{part}' ne postoji.");
                return null;
            }

            if (!authorIds.Contains(id))
            {
                authorIds.Add(id);
            }
        }

        if (authorIds.Count == 0)
        {
            ConsoleOutput.ShowError("Knjiga mora imati bar jednog autora.");
            return null;
        }

        return authorIds;
    }

    private Knjiga? FindByIsbn()
    {
        string isbn = ConsoleInput.ReadText("ISBN knjige");
        Knjiga? knjiga = _knjigaDao.GetByIsbn(isbn);
        if (knjiga is null)
        {
            ConsoleOutput.ShowError($"Knjiga sa ISBN-om '{isbn}' ne postoji.");
        }

        return knjiga;
    }

    private List<Autor> AuthorsOf(string isbn)
    {
        List<int> authorIds = _knjigaAutorDao.GetAuthorIds(isbn);
        return _autorDao.GetAll().Where(a => authorIds.Contains(a.Id)).ToList();
    }

    private List<Kupovina> PurchasesOf(string isbn) =>
        _kupovinaDao.GetAll().Where(k => k.BookIsbn == isbn).ToList();

    private string PublisherName(string code) =>
        _izdavacDao.GetByCode(code)?.Name ?? "-";

    private string VisitorName(string cardNumber)
    {
        Posetilac? posetilac = _posetilacDao.GetByCardNumber(cardNumber);
        return posetilac is null ? "-" : $"{posetilac.FirstName} {posetilac.LastName}";
    }
}