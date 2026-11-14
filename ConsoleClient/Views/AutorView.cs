// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class AutorView
{
    private const int MinHeadExperience = 5;
    private const int MaxExperience = 80;

    private readonly AutorDAO _autorDao;
    private readonly AdresaDAO _adresaDao;
    private readonly IzdavacDAO _izdavacDao;
    private readonly KnjigaDAO _knjigaDao;
    private readonly KnjigaAutorDAO _knjigaAutorDao;

    public AutorView(AutorDAO autorDao, AdresaDAO adresaDao, IzdavacDAO izdavacDao, KnjigaDAO knjigaDao, KnjigaAutorDAO knjigaAutorDao)
    {
        _autorDao = autorDao;
        _adresaDao = adresaDao;
        _izdavacDao = izdavacDao;
        _knjigaDao = knjigaDao;
        _knjigaAutorDao = knjigaAutorDao;
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
        ConsoleOutput.ShowTitle("AUTORI");
        Console.WriteLine("1 - Prikaz svih autora");
        Console.WriteLine("2 - Detalji autora");
        Console.WriteLine("3 - Dodavanje autora");
        Console.WriteLine("4 - Izmjena autora");
        Console.WriteLine("5 - Brisanje autora");
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
        List<Autor> autori = _autorDao.GetAll();
        if (autori.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema autora.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"ID",4} | {"Ime",-12} | {"Prezime",-14} | {"Broj LK",-12} | {"Datum rođenja",-13} | {"E-mejl",-26}");
        foreach (Autor autor in autori)
        {
            Console.WriteLine($"{autor.Id,4} | " +
                              $"{ConsoleOutput.Fit(autor.FirstName, 12)} | " +
                              $"{ConsoleOutput.Fit(autor.LastName, 14)} | " +
                              $"{ConsoleOutput.Fit(autor.IdCardNumber, 12)} | " +
                              $"{ConsoleOutput.FormatDate(autor.DateOfBirth),-13} | " +
                              $"{ConsoleOutput.Fit(autor.Email, 26)}");
        }

        Console.WriteLine($"Ukupno autora: {autori.Count}");
    }

    private void ShowDetails()
    {
        Autor? autor = FindById();
        if (autor is null)
        {
            return;
        }

        List<Izdavac> headOf = PublishersHeadedBy(autor.Id);
        List<Knjiga> knjige = BooksOf(autor.Id);

        Console.WriteLine($"ID:                {autor.Id}");
        Console.WriteLine($"Ime i prezime:     {autor.FirstName} {autor.LastName}");
        Console.WriteLine($"Datum rođenja:     {ConsoleOutput.FormatDate(autor.DateOfBirth)}");
        Console.WriteLine($"Adresa:            {ConsoleOutput.FormatAddress(_adresaDao.GetById(autor.AddressId))}");
        Console.WriteLine($"Telefon:           {ConsoleOutput.OrDash(autor.Phone)}");
        Console.WriteLine($"E-mejl:            {ConsoleOutput.OrDash(autor.Email)}");
        Console.WriteLine($"Broj lične karte:  {autor.IdCardNumber}");
        Console.WriteLine($"Godine iskustva:   {autor.YearsOfExperience}");
        Console.WriteLine($"Šef izdavača:      {(headOf.Count == 0 ? "-" : PublisherNames(headOf))}");

        Console.WriteLine($"Knjige ({knjige.Count}):");
        knjige.ForEach(k => Console.WriteLine($"  {k.Isbn} - {k.Title}"));
    }

    private void Add()
    {
        Autor autor = ReadAutor(null);
        _autorDao.Add(autor);
        ConsoleOutput.ShowSuccess($"Autor je uspješno dodat (ID {autor.Id}).");
    }

    private void Update()
    {
        Autor? existing = FindById();
        if (existing is null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Pritisnite Enter da zadržite trenutnu vrijednost (prikazana je u zagradi).");
        Autor updated = ReadAutor(existing);
        _autorDao.Update(updated);
        ConsoleOutput.ShowSuccess("Podaci o autoru su uspješno izmijenjeni.");
    }

    private void Remove()
    {
        Autor? autor = FindById();
        if (autor is null)
        {
            return;
        }

        List<Izdavac> headOf = PublishersHeadedBy(autor.Id);
        if (headOf.Count > 0)
        {
            ConsoleOutput.ShowError($"Autor {autor.FirstName} {autor.LastName} je šef izdavača ({PublisherNames(headOf)}). Prvo postavite drugog šefa u meniju Izdavači.");
            return;
        }

        int bookCount = BooksOf(autor.Id).Count;
        if (bookCount > 0)
        {
            ConsoleOutput.ShowError($"Autor {autor.FirstName} {autor.LastName} ima knjiga: {bookCount}. Prvo ga uklonite sa tih knjiga (izmjena knjige) ili obrišite knjige.");
            return;
        }

        if (!ConsoleInput.Confirm($"Da li ste sigurni da želite da obrišete autora {autor.FirstName} {autor.LastName}?"))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        _autorDao.Remove(autor.Id);
        ConsoleOutput.ShowSuccess("Autor je uspješno obrisan.");
    }

    private Autor ReadAutor(Autor? current)
    {
        return new Autor
        {
            Id = current?.Id ?? 0,
            FirstName = ConsoleInput.ReadText("Ime", true, current?.FirstName),
            LastName = ConsoleInput.ReadText("Prezime", true, current?.LastName),
            DateOfBirth = ConsoleInput.ReadDate("Datum rođenja", current?.DateOfBirth, false),
            AddressId = ReadAddressId(current?.AddressId),
            Phone = ConsoleInput.ReadPhone("Telefon", current?.Phone),
            Email = ConsoleInput.ReadEmail("E-mejl", current?.Email),
            IdCardNumber = ReadIdCardNumber(current),
            YearsOfExperience = ReadYearsOfExperience(current)
        };
    }

    private int ReadAddressId(int? current)
    {
        List<Adresa> adrese = _adresaDao.GetAll();
        if (adrese.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema adresa (dodaju se u meniju Adrese), pa se autor upisuje bez adrese.");
            return 0;
        }

        Console.WriteLine("Dostupne adrese:");
        adrese.ForEach(a => Console.WriteLine($"  {a.Id,3} - {ConsoleOutput.FormatAddress(a)}"));

        while (true)
        {
            int id = ConsoleInput.ReadInt("ID adrese (0 - bez adrese)", 0, int.MaxValue, current);
            if (id == 0 || _adresaDao.GetById(id) is not null)
            {
                return id;
            }

            ConsoleOutput.ShowError($"Adresa sa ID-jem {id} ne postoji.");
        }
    }

    private string ReadIdCardNumber(Autor? current)
    {
        while (true)
        {
            string idCardNumber = ConsoleInput.ReadText("Broj lične karte", true, current?.IdCardNumber);
            Autor? owner = _autorDao.GetAll().FirstOrDefault(a => a.IdCardNumber == idCardNumber);

            if (owner is null || owner.Id == current?.Id)
            {
                return idCardNumber;
            }

            ConsoleOutput.ShowError($"Broj lične karte '{idCardNumber}' već ima autor {owner.FirstName} {owner.LastName}.");
        }
    }

    private int ReadYearsOfExperience(Autor? current)
    {
        List<Izdavac> headOf = current is null ? new List<Izdavac>() : PublishersHeadedBy(current.Id);

        while (true)
        {
            int years = ConsoleInput.ReadInt("Godine iskustva", 0, MaxExperience, current?.YearsOfExperience);
            if (headOf.Count == 0 || years >= MinHeadExperience)
            {
                return years;
            }

            ConsoleOutput.ShowError($"Autor je šef izdavača ({PublisherNames(headOf)}), pa mora imati najmanje {MinHeadExperience} godina iskustva.");
        }
    }

    private Autor? FindById()
    {
        int id = ConsoleInput.ReadInt("ID autora", 1, int.MaxValue);
        Autor? autor = _autorDao.GetById(id);
        if (autor is null)
        {
            ConsoleOutput.ShowError($"Autor sa ID-jem {id} ne postoji.");
        }

        return autor;
    }

    private List<Izdavac> PublishersHeadedBy(int authorId) =>
        _izdavacDao.GetAll().Where(i => i.HeadAuthorId == authorId).ToList();

    private List<Knjiga> BooksOf(int authorId)
    {
        List<string> isbns = _knjigaAutorDao.GetBookIsbns(authorId);
        return _knjigaDao.GetAll().Where(k => isbns.Contains(k.Isbn)).ToList();
    }

    private static string PublisherNames(List<Izdavac> izdavaci) =>
        string.Join(", ", izdavaci.Select(i => i.Name));
}