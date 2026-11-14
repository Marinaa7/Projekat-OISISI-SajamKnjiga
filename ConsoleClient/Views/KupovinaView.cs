// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class KupovinaView
{
    private readonly KupovinaDAO _kupovinaDao;
    private readonly PosetilacDAO _posetilacDao;
    private readonly KnjigaDAO _knjigaDao;

    public KupovinaView(KupovinaDAO kupovinaDao, PosetilacDAO posetilacDao, KnjigaDAO knjigaDao)
    {
        _kupovinaDao = kupovinaDao;
        _posetilacDao = posetilacDao;
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
        ConsoleOutput.ShowTitle("KUPOVINE");
        Console.WriteLine("1 - Prikaz svih kupovina");
        Console.WriteLine("2 - Detalji kupovine");
        Console.WriteLine("3 - Dodavanje kupovine");
        Console.WriteLine("4 - Izmjena kupovine");
        Console.WriteLine("5 - Brisanje kupovine");
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
        List<Kupovina> kupovine = _kupovinaDao.GetAll();
        if (kupovine.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema kupovina.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"ID",4} | {"Br. karte",-12} | {"Posjetilac",-20} | {"ISBN",-17} | {"Knjiga",-22} | {"Datum",-11} | {"Ocjena",6}");
        foreach (Kupovina kupovina in kupovine)
        {
            Console.WriteLine($"{kupovina.Id,4} | " +
                              $"{ConsoleOutput.Fit(kupovina.VisitorCardNumber, 12)} | " +
                              $"{ConsoleOutput.Fit(VisitorName(kupovina.VisitorCardNumber), 20)} | " +
                              $"{ConsoleOutput.Fit(kupovina.BookIsbn, 17)} | " +
                              $"{ConsoleOutput.Fit(BookTitle(kupovina.BookIsbn), 22)} | " +
                              $"{ConsoleOutput.FormatDate(kupovina.PurchaseDate),-11} | " +
                              $"{kupovina.Rating,6}");
        }

        Console.WriteLine($"Ukupno kupovina: {kupovine.Count}");
    }

    private void ShowDetails()
    {
        Kupovina? kupovina = FindById();
        if (kupovina == null)
        {
            return;
        }

        Console.WriteLine($"ID kupovine:     {kupovina.Id}");
        Console.WriteLine($"Posjetilac:      {kupovina.VisitorCardNumber} ({VisitorName(kupovina.VisitorCardNumber)})");
        Console.WriteLine($"Knjiga:          {kupovina.BookIsbn} ({BookTitle(kupovina.BookIsbn)})");
        Console.WriteLine($"Datum kupovine:  {ConsoleOutput.FormatDate(kupovina.PurchaseDate)}");
        Console.WriteLine($"Ocjena:          {kupovina.Rating}");
        Console.WriteLine($"Komentar:        {ConsoleOutput.OrDash(kupovina.Comment)}");
    }

    private void Add()
    {
        Posetilac? posetilac = ChooseVisitor();
        if (posetilac == null)
        {
            return;
        }

        Knjiga? knjiga = ChooseBook();
        if (knjiga == null)
        {
            return;
        }

        bool alreadyBought = _kupovinaDao.GetAll().Any(k => k.VisitorCardNumber == posetilac.CardNumber && k.BookIsbn == knjiga.Isbn);
        if (alreadyBought)
        {
            ConsoleOutput.ShowError("Ovaj posjetilac je već kupio ovu knjigu.");
            return;
        }

        Kupovina kupovina = new Kupovina
        {
            VisitorCardNumber = posetilac.CardNumber,
            BookIsbn = knjiga.Isbn,
            PurchaseDate = ConsoleInput.ReadDate("Datum kupovine", null, false),
            Rating = ConsoleInput.ReadInt("Ocjena (1-5)", 1, 5),
            Comment = ConsoleInput.ReadText("Komentar", false)
        };

        _kupovinaDao.Add(kupovina);
        UpdateAverageRating(posetilac.CardNumber);
        ConsoleOutput.ShowSuccess($"Kupovina je uspješno dodata (ID {kupovina.Id}).");
    }

    private void Update()
    {
        Kupovina? existing = FindById();
        if (existing == null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Posjetilac i knjiga se ne mijenjaju. Pritisnite Enter da zadržite trenutnu vrijednost.");
        Kupovina updated = new Kupovina
        {
            Id = existing.Id,
            VisitorCardNumber = existing.VisitorCardNumber,
            BookIsbn = existing.BookIsbn,
            PurchaseDate = ConsoleInput.ReadDate("Datum kupovine", existing.PurchaseDate, false),
            Rating = ConsoleInput.ReadInt("Ocjena (1-5)", 1, 5, existing.Rating),
            Comment = ConsoleInput.ReadText("Komentar", false, existing.Comment)
        };

        _kupovinaDao.Update(updated);
        UpdateAverageRating(updated.VisitorCardNumber);
        ConsoleOutput.ShowSuccess("Kupovina je uspješno izmijenjena.");
    }

    private void Remove()
    {
        Kupovina? kupovina = FindById();
        if (kupovina == null)
        {
            return;
        }

        string question = $"Da li ste sigurni da želite da obrišete kupovinu {kupovina.Id} ({VisitorName(kupovina.VisitorCardNumber)} - {BookTitle(kupovina.BookIsbn)})?";
        if (!ConsoleInput.Confirm(question))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        _kupovinaDao.Remove(kupovina.Id);
        UpdateAverageRating(kupovina.VisitorCardNumber);
        ConsoleOutput.ShowSuccess("Kupovina je uspješno obrisana.");
    }

    private Posetilac? ChooseVisitor()
    {
        List<Posetilac> posetioci = _posetilacDao.GetAll();
        if (posetioci.Count == 0)
        {
            ConsoleOutput.ShowError("U sistemu nema posjetilaca. Prvo dodajte posjetioca.");
            return null;
        }

        Console.WriteLine("Posjetioci:");
        foreach (Posetilac posetilac in posetioci)
        {
            Console.WriteLine($"  {posetilac.CardNumber} - {posetilac.FirstName} {posetilac.LastName}");
        }

        string cardNumber = ConsoleInput.ReadText("Broj članske karte");
        Posetilac? chosen = _posetilacDao.GetByCardNumber(cardNumber);
        if (chosen == null)
        {
            ConsoleOutput.ShowError($"Posjetilac sa brojem karte '{cardNumber}' ne postoji.");
        }

        return chosen;
    }

    private Knjiga? ChooseBook()
    {
        List<Knjiga> knjige = _knjigaDao.GetAll();
        if (knjige.Count == 0)
        {
            ConsoleOutput.ShowError("U sistemu nema knjiga. Prvo dodajte knjigu u meniju Knjige.");
            return null;
        }

        Console.WriteLine("Knjige:");
        foreach (Knjiga knjiga in knjige)
        {
            Console.WriteLine($"  {knjiga.Isbn} - {knjiga.Title}");
        }

        string isbn = ConsoleInput.ReadText("ISBN knjige");
        Knjiga? chosen = _knjigaDao.GetByIsbn(isbn);
        if (chosen == null)
        {
            ConsoleOutput.ShowError($"Knjiga sa ISBN-om '{isbn}' ne postoji.");
        }

        return chosen;
    }

    private Kupovina? FindById()
    {
        int id = ConsoleInput.ReadInt("ID kupovine", 1, int.MaxValue);
        Kupovina? kupovina = _kupovinaDao.GetById(id);
        if (kupovina == null)
        {
            ConsoleOutput.ShowError($"Kupovina sa ID-jem {id} ne postoji.");
        }

        return kupovina;
    }

    private void UpdateAverageRating(string cardNumber)
    {
        Posetilac? posetilac = _posetilacDao.GetByCardNumber(cardNumber);
        if (posetilac == null)
        {
            return;
        }

        List<Kupovina> kupovine = _kupovinaDao.GetAll().Where(k => k.VisitorCardNumber == cardNumber).ToList();
        posetilac.AverageRating = kupovine.Count == 0 ? 0 : kupovine.Average(k => k.Rating);
        _posetilacDao.Update(posetilac);
    }

    private string VisitorName(string cardNumber)
    {
        Posetilac? posetilac = _posetilacDao.GetByCardNumber(cardNumber);
        return posetilac == null ? "-" : $"{posetilac.FirstName} {posetilac.LastName}";
    }

    private string BookTitle(string isbn)
    {
        return _knjigaDao.GetByIsbn(isbn)?.Title ?? "-";
    }
}