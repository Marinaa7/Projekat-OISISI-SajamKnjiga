// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class IzdavacView
{
    private const int MinHeadExperience = 5;

    private readonly IzdavacDAO _izdavacDao;
    private readonly AutorDAO _autorDao;
    private readonly KnjigaDAO _knjigaDao;

    public IzdavacView(IzdavacDAO izdavacDao, AutorDAO autorDao, KnjigaDAO knjigaDao)
    {
        _izdavacDao = izdavacDao;
        _autorDao = autorDao;
        _knjigaDao = knjigaDao;
    }

    public void Run()
    {
        while (true)
        {
            ShowMenu();
            string choice = ConsoleInput.ReadChoice();

            if (choice == "0")
            {
                return;
            }

            try
            {
                HandleChoice(choice);
            }
            catch (Exception ex)
            {
                ConsoleOutput.ShowError("Operacija nije izvršena: " + ex.Message);
            }
        }
    }

    private static void ShowMenu()
    {
        ConsoleOutput.ShowTitle("IZDAVAČI");
        Console.WriteLine("1 - Prikaz svih izdavača");
        Console.WriteLine("2 - Detalji izdavača");
        Console.WriteLine("3 - Dodavanje izdavača");
        Console.WriteLine("4 - Izmjena izdavača");
        Console.WriteLine("5 - Brisanje izdavača");
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
        List<Izdavac> izdavaci = _izdavacDao.GetAll();
        if (izdavaci.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema izdavača.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"Šifra",-8} | {"Naziv",-28} | {"Šef izdavača",-26} | {"Br. knjiga",10}");
        foreach (Izdavac izdavac in izdavaci)
        {
            Console.WriteLine($"{ConsoleOutput.Fit(izdavac.Code, 8)} | " +
                              $"{ConsoleOutput.Fit(izdavac.Name, 28)} | " +
                              $"{ConsoleOutput.Fit(AuthorName(izdavac.HeadAuthorId), 26)} | " +
                              $"{BooksOf(izdavac.Code).Count,10}");
        }

        Console.WriteLine($"Ukupno izdavača: {izdavaci.Count}");
    }

    private void ShowDetails()
    {
        Izdavac? izdavac = FindByCode();
        if (izdavac == null)
        {
            return;
        }

        Autor? sef = _autorDao.GetById(izdavac.HeadAuthorId);
        string headText = sef == null ? "-" : $"{sef.FirstName} {sef.LastName} (godine iskustva: {sef.YearsOfExperience})";

        Console.WriteLine($"Šifra:          {izdavac.Code}");
        Console.WriteLine($"Naziv:          {izdavac.Name}");
        Console.WriteLine($"Šef izdavača:   {headText}");

        List<Knjiga> knjige = BooksOf(izdavac.Code);
        Console.WriteLine($"Knjige ({knjige.Count}):");
        foreach (Knjiga knjiga in knjige)
        {
            Console.WriteLine($"  {knjiga.Isbn} - {knjiga.Title}");
        }
    }

    private void Add()
    {
        string code = ConsoleInput.ReadText("Šifra izdavača");
        if (_izdavacDao.GetByCode(code) != null)
        {
            ConsoleOutput.ShowError($"Izdavač sa šifrom '{code}' već postoji.");
            return;
        }

        Izdavac izdavac = new Izdavac
        {
            Code = code,
            Name = ConsoleInput.ReadText("Naziv"),
            HeadAuthorId = ReadHeadAuthorId(null)
        };

        _izdavacDao.Add(izdavac);
        ConsoleOutput.ShowSuccess("Izdavač je uspješno dodat.");
    }

    private void Update()
    {
        Izdavac? existing = FindByCode();
        if (existing == null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Pritisnite Enter da zadržite trenutnu vrijednost (prikazana je u zagradi).");
        Izdavac updated = new Izdavac
        {
            Code = existing.Code,
            Name = ConsoleInput.ReadText("Naziv", true, existing.Name),
            HeadAuthorId = ReadHeadAuthorId(existing.HeadAuthorId)
        };

        _izdavacDao.Update(updated);
        ConsoleOutput.ShowSuccess("Podaci o izdavaču su uspješno izmijenjeni.");
    }

    private void Remove()
    {
        Izdavac? izdavac = FindByCode();
        if (izdavac == null)
        {
            return;
        }

        int bookCount = BooksOf(izdavac.Code).Count;
        if (bookCount > 0)
        {
            ConsoleOutput.ShowError($"Izdavač '{izdavac.Name}' ima knjiga: {bookCount}. Ne može se obrisati dok se te knjige ne obrišu ili ne prebace drugom izdavaču.");
            return;
        }

        if (!ConsoleInput.Confirm($"Da li ste sigurni da želite da obrišete izdavača '{izdavac.Name}'?"))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        _izdavacDao.Remove(izdavac.Code);
        ConsoleOutput.ShowSuccess("Izdavač je uspješno obrisan.");
    }

    private int ReadHeadAuthorId(int? current)
    {
        List<Autor> candidates = _autorDao.GetAll().Where(a => a.YearsOfExperience >= MinHeadExperience).ToList();
        if (candidates.Count == 0)
        {
            ConsoleOutput.ShowInfo($"Nema autora sa najmanje {MinHeadExperience} godina iskustva, pa se izdavač upisuje bez šefa.");
            return 0;
        }

        Console.WriteLine($"Autori koji mogu biti šef (najmanje {MinHeadExperience} godina iskustva):");
        foreach (Autor autor in candidates)
        {
            Console.WriteLine($"  {autor.Id,3} - {autor.FirstName} {autor.LastName} ({autor.YearsOfExperience} god.)");
        }

        while (true)
        {
            int id = ConsoleInput.ReadInt("ID šefa izdavača (0 - bez šefa)", 0, int.MaxValue, current);
            if (id == 0)
            {
                return 0;
            }

            Autor? autor = _autorDao.GetById(id);
            if (autor == null)
            {
                ConsoleOutput.ShowError($"Autor sa ID-jem {id} ne postoji.");
            }
            else if (autor.YearsOfExperience < MinHeadExperience)
            {
                ConsoleOutput.ShowError($"Autor {autor.FirstName} {autor.LastName} ima {autor.YearsOfExperience} god. iskustva, a šef mora imati najmanje {MinHeadExperience}.");
            }
            else
            {
                return id;
            }
        }
    }

    private Izdavac? FindByCode()
    {
        string code = ConsoleInput.ReadText("Šifra izdavača");
        Izdavac? izdavac = _izdavacDao.GetByCode(code);
        if (izdavac == null)
        {
            ConsoleOutput.ShowError($"Izdavač sa šifrom '{code}' ne postoji.");
        }

        return izdavac;
    }

    private List<Knjiga> BooksOf(string code)
    {
        return _knjigaDao.GetAll().Where(k => k.PublisherCode == code).ToList();
    }

    private string AuthorName(int authorId)
    {
        Autor? autor = _autorDao.GetById(authorId);
        return autor == null ? "-" : $"{autor.FirstName} {autor.LastName}";
    }
}