// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class PosetilacView
{
    private readonly PosetilacDAO _posetilacDao;
    private readonly KupovinaDAO _kupovinaDao;
    private readonly AdresaDAO _adresaDao;
    private readonly KnjigaDAO _knjigaDao;
    private readonly ListaZeljaDAO _listaZeljaDao;

    public PosetilacView(PosetilacDAO posetilacDao, KupovinaDAO kupovinaDao, AdresaDAO adresaDao, KnjigaDAO knjigaDao, ListaZeljaDAO listaZeljaDao)
    {
        _posetilacDao = posetilacDao;
        _kupovinaDao = kupovinaDao;
        _adresaDao = adresaDao;
        _knjigaDao = knjigaDao;
        _listaZeljaDao = listaZeljaDao;
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
        ConsoleOutput.ShowTitle("POSJETIOCI");
        Console.WriteLine("1 - Prikaz svih posjetilaca");
        Console.WriteLine("2 - Detalji posjetioca");
        Console.WriteLine("3 - Dodavanje posjetioca");
        Console.WriteLine("4 - Izmjena posjetioca");
        Console.WriteLine("5 - Brisanje posjetioca");
        Console.WriteLine("6 - Dodavanje knjige na listu želja");
        Console.WriteLine("7 - Uklanjanje knjige sa liste želja");
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
            case "6":
                AddToWishlist();
                break;
            case "7":
                RemoveFromWishlist();
                break;
            default:
                ConsoleOutput.ShowError("Nepostojeća opcija.");
                break;
        }
    }

    private void ShowAll()
    {
        List<Posetilac> posetioci = _posetilacDao.GetAll();
        if (posetioci.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema posjetilaca.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"Br. karte",-12} | {"Ime",-12} | {"Prezime",-14} | {"Adresa",-30} | {"Status",-7} | {"Ocjena",6}");
        foreach (Posetilac posetilac in posetioci)
        {
            Console.WriteLine($"{ConsoleOutput.Fit(posetilac.CardNumber, 12)} | " +
                              $"{ConsoleOutput.Fit(posetilac.FirstName, 12)} | " +
                              $"{ConsoleOutput.Fit(posetilac.LastName, 14)} | " +
                              $"{ConsoleOutput.Fit(ConsoleOutput.FormatAddress(_adresaDao.GetById(posetilac.AddressId)), 30)} | " +
                              $"{ConsoleOutput.Fit(FormatStatus(posetilac.Status), 7)} | " +
                              $"{posetilac.AverageRating,6:0.00}");
        }

        Console.WriteLine($"Ukupno posjetilaca: {posetioci.Count}");
    }

    private void ShowDetails()
    {
        Posetilac? posetilac = FindByCardNumber();
        if (posetilac == null)
        {
            return;
        }

        List<Kupovina> kupovine = PurchasesOf(posetilac.Id);
        List<Knjiga> wishlist = WishlistOf(posetilac.Id);

        Console.WriteLine($"ID:                  {posetilac.Id}");
        Console.WriteLine($"Broj članske karte:  {posetilac.CardNumber}");
        Console.WriteLine($"Ime i prezime:       {posetilac.FirstName} {posetilac.LastName}");
        Console.WriteLine($"Datum rođenja:       {ConsoleOutput.FormatDate(posetilac.DateOfBirth)}");
        Console.WriteLine($"Adresa:              {ConsoleOutput.FormatAddress(_adresaDao.GetById(posetilac.AddressId))}");
        Console.WriteLine($"Kontakt telefon:     {ConsoleOutput.OrDash(posetilac.Phone)}");
        Console.WriteLine($"E-mejl:              {ConsoleOutput.OrDash(posetilac.Email)}");
        Console.WriteLine($"Godina članstva:     {posetilac.MembershipYear}");
        Console.WriteLine($"Status:              {FormatStatus(posetilac.Status)}");
        Console.WriteLine($"Prosječna ocjena:    {posetilac.AverageRating:0.00}");

        Console.WriteLine($"Kupljene knjige ({kupovine.Count}):");
        foreach (Kupovina kupovina in kupovine)
        {
            Console.WriteLine($"  {BookText(kupovina.BookId)} (ocjena {kupovina.Rating})");
        }

        Console.WriteLine($"Lista želja ({wishlist.Count}):");
        foreach (Knjiga knjiga in wishlist)
        {
            Console.WriteLine($"  {knjiga.Isbn} - {knjiga.Title}");
        }
    }

    private void Add()
    {
        string cardNumber = ConsoleInput.ReadText("Broj članske karte (npr. SK-1-2023)");
        if (_posetilacDao.GetByCardNumber(cardNumber) != null)
        {
            ConsoleOutput.ShowError($"Posjetilac sa brojem karte '{cardNumber}' već postoji.");
            return;
        }

        Posetilac posetilac = ReadPosetilac(cardNumber, null);
        _posetilacDao.Add(posetilac);
        ConsoleOutput.ShowSuccess("Posjetilac je uspješno dodat.");
    }

    private void Update()
    {
        Posetilac? existing = FindByCardNumber();
        if (existing == null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Pritisnite Enter da zadržite trenutnu vrijednost (prikazana je u zagradi).");
        Posetilac updated = ReadPosetilac(existing.CardNumber, existing);
        _posetilacDao.Update(updated);
        ConsoleOutput.ShowSuccess("Podaci o posjetiocu su uspješno izmijenjeni.");
    }

    private void Remove()
    {
        Posetilac? posetilac = FindByCardNumber();
        if (posetilac == null)
        {
            return;
        }

        List<Kupovina> kupovine = PurchasesOf(posetilac.Id);
        int wishlistCount = _listaZeljaDao.GetBookIds(posetilac.Id).Count;
        string question = kupovine.Count == 0 && wishlistCount == 0
            ? $"Da li ste sigurni da želite da obrišete posjetioca {posetilac.FirstName} {posetilac.LastName}?"
            : $"Posjetilac {posetilac.FirstName} {posetilac.LastName} ima kupovina: {kupovine.Count} i knjiga na listi želja: {wishlistCount}. I one će biti obrisane. Da li ste sigurni?";

        if (!ConsoleInput.Confirm(question))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        foreach (Kupovina kupovina in kupovine)
        {
            _kupovinaDao.Remove(kupovina.Id);
        }

        _listaZeljaDao.RemoveByVisitor(posetilac.Id);
        _posetilacDao.Remove(posetilac.Id);
        ConsoleOutput.ShowSuccess("Posjetilac je uspješno obrisan.");
    }

    private void AddToWishlist()
    {
        Posetilac? posetilac = FindByCardNumber();
        if (posetilac == null)
        {
            return;
        }

        if (_knjigaDao.GetAll().Count == 0)
        {
            ConsoleOutput.ShowError("U sistemu nema knjiga. Prvo dodajte knjigu u meniju Knjige.");
            return;
        }

        List<Knjiga> available = _knjigaDao.GetAll().Where(k => CanAddToWishlist(posetilac.Id, k.Id)).ToList();
        if (available.Count == 0)
        {
            ConsoleOutput.ShowInfo("Nema knjiga koje se mogu dodati: posjetilac je sve knjige već kupio ili ih ima na listi želja.");
            return;
        }

        Console.WriteLine("Knjige koje se mogu dodati (nisu kupljene i nisu već na listi želja):");
        foreach (Knjiga knjiga in available)
        {
            Console.WriteLine($"  {knjiga.Isbn} - {knjiga.Title}");
        }

        string isbn = ConsoleInput.ReadText("ISBN knjige");
        Knjiga? chosen = available.Find(k => k.Isbn == isbn);
        if (chosen == null)
        {
            ConsoleOutput.ShowError($"Knjiga sa ISBN-om '{isbn}' nije na spisku knjiga koje se mogu dodati.");
            return;
        }

        _listaZeljaDao.Add(posetilac.Id, chosen.Id);
        ConsoleOutput.ShowSuccess($"Knjiga '{chosen.Title}' je dodata na listu želja.");
    }

    private void RemoveFromWishlist()
    {
        Posetilac? posetilac = FindByCardNumber();
        if (posetilac == null)
        {
            return;
        }

        List<Knjiga> wishlist = WishlistOf(posetilac.Id);
        if (wishlist.Count == 0)
        {
            ConsoleOutput.ShowInfo("Lista želja ovog posjetioca je prazna.");
            return;
        }

        Console.WriteLine("Lista želja:");
        foreach (Knjiga knjiga in wishlist)
        {
            Console.WriteLine($"  {knjiga.Isbn} - {knjiga.Title}");
        }

        string isbn = ConsoleInput.ReadText("ISBN knjige");
        Knjiga? chosen = wishlist.Find(k => k.Isbn == isbn);
        if (chosen == null)
        {
            ConsoleOutput.ShowError($"Knjiga sa ISBN-om '{isbn}' nije na listi želja ovog posjetioca.");
            return;
        }

        if (!ConsoleInput.Confirm($"Da li ste sigurni da želite da uklonite knjigu '{chosen.Title}' sa liste želja?"))
        {
            ConsoleOutput.ShowInfo("Uklanjanje je otkazano.");
            return;
        }

        _listaZeljaDao.Remove(posetilac.Id, chosen.Id);
        ConsoleOutput.ShowSuccess("Knjiga je uklonjena sa liste želja.");
    }

    private Posetilac ReadPosetilac(string cardNumber, Posetilac? current)
    {
        return new Posetilac
        {
            Id = current?.Id ?? 0,
            CardNumber = cardNumber,
            FirstName = ConsoleInput.ReadText("Ime", true, current?.FirstName),
            LastName = ConsoleInput.ReadText("Prezime", true, current?.LastName),
            DateOfBirth = ConsoleInput.ReadDate("Datum rođenja", current?.DateOfBirth, false),
            AddressId = ReadAddressId(current?.AddressId),
            Phone = ConsoleInput.ReadPhone("Kontakt telefon", current?.Phone),
            Email = ConsoleInput.ReadEmail("E-mejl", current?.Email),
            MembershipYear = ConsoleInput.ReadInt("Trenutna godina članstva", 1, 100, current?.MembershipYear),
            Status = ConsoleInput.ReadStatus("Status", current?.Status),
            AverageRating = current?.AverageRating ?? 0
        };
    }

    private int ReadAddressId(int? current)
    {
        List<Adresa> adrese = _adresaDao.GetAll();
        if (adrese.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema adresa (dodaju se u meniju Adrese), pa se posjetilac upisuje bez adrese.");
            return 0;
        }

        Console.WriteLine("Dostupne adrese:");
        foreach (Adresa adresa in adrese)
        {
            Console.WriteLine($"  {adresa.Id,3} - {ConsoleOutput.FormatAddress(adresa)}");
        }

        while (true)
        {
            int id = ConsoleInput.ReadInt("ID adrese (0 - bez adrese)", 0, int.MaxValue, current);
            if (id == 0 || _adresaDao.GetById(id) != null)
            {
                return id;
            }

            ConsoleOutput.ShowError($"Adresa sa ID-jem {id} ne postoji.");
        }
    }

    private Posetilac? FindByCardNumber()
    {
        string cardNumber = ConsoleInput.ReadText("Broj članske karte");
        Posetilac? posetilac = _posetilacDao.GetByCardNumber(cardNumber);
        if (posetilac == null)
        {
            ConsoleOutput.ShowError($"Posjetilac sa brojem karte '{cardNumber}' ne postoji.");
        }

        return posetilac;
    }

    private bool CanAddToWishlist(int visitorId, int bookId)
    {
        bool purchased = _kupovinaDao.GetAll().Exists(k => k.VisitorId == visitorId && k.BookId == bookId);
        return !purchased && !_listaZeljaDao.Contains(visitorId, bookId);
    }

    private List<Kupovina> PurchasesOf(int visitorId)
    {
        return _kupovinaDao.GetAll().Where(k => k.VisitorId == visitorId).ToList();
    }

    private List<Knjiga> WishlistOf(int visitorId)
    {
        List<int> bookIds = _listaZeljaDao.GetBookIds(visitorId);
        return _knjigaDao.GetAll().Where(k => bookIds.Contains(k.Id)).ToList();
    }

    private string BookText(int bookId)
    {
        Knjiga? knjiga = _knjigaDao.GetById(bookId);
        return knjiga == null ? "-" : $"{knjiga.Isbn} - {knjiga.Title}";
    }

    private static string FormatStatus(MembershipStatus status)
    {
        return status == MembershipStatus.V ? "VIP" : "Redovan";
    }
}