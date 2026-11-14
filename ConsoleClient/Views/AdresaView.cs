// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class AdresaView
{
    private readonly AdresaDAO _adresaDao;
    private readonly PosetilacDAO _posetilacDao;
    private readonly AutorDAO _autorDao;

    public AdresaView(AdresaDAO adresaDao, PosetilacDAO posetilacDao, AutorDAO autorDao)
    {
        _adresaDao = adresaDao;
        _posetilacDao = posetilacDao;
        _autorDao = autorDao;
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
        ConsoleOutput.ShowTitle("ADRESE");
        Console.WriteLine("1 - Prikaz svih adresa");
        Console.WriteLine("2 - Detalji adrese");
        Console.WriteLine("3 - Dodavanje adrese");
        Console.WriteLine("4 - Izmjena adrese");
        Console.WriteLine("5 - Brisanje adrese");
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
        List<Adresa> adrese = _adresaDao.GetAll();
        if (adrese.Count == 0)
        {
            ConsoleOutput.ShowInfo("U sistemu nema adresa.");
            return;
        }

        ConsoleOutput.ShowTableHeader($"{"ID",4} | {"Ulica",-28} | {"Broj",-6} | {"Grad",-18} | {"Država",-16}");
        foreach (Adresa adresa in adrese)
        {
            Console.WriteLine($"{adresa.Id,4} | " +
                              $"{ConsoleOutput.Fit(adresa.Street, 28)} | " +
                              $"{ConsoleOutput.Fit(adresa.Number, 6)} | " +
                              $"{ConsoleOutput.Fit(adresa.City, 18)} | " +
                              $"{ConsoleOutput.Fit(adresa.Country, 16)}");
        }

        Console.WriteLine($"Ukupno adresa: {adrese.Count}");
    }

    private void ShowDetails()
    {
        Adresa? adresa = FindById();
        if (adresa is null)
        {
            return;
        }

        List<Posetilac> posetioci = VisitorsAt(adresa.Id);
        List<Autor> autori = AuthorsAt(adresa.Id);

        Console.WriteLine($"ID:      {adresa.Id}");
        Console.WriteLine($"Ulica:   {adresa.Street}");
        Console.WriteLine($"Broj:    {adresa.Number}");
        Console.WriteLine($"Grad:    {adresa.City}");
        Console.WriteLine($"Država:  {adresa.Country}");

        Console.WriteLine($"Posjetioci na ovoj adresi ({posetioci.Count}):");
        posetioci.ForEach(p => Console.WriteLine($"  {p.CardNumber} - {p.FirstName} {p.LastName}"));

        Console.WriteLine($"Autori na ovoj adresi ({autori.Count}):");
        autori.ForEach(a => Console.WriteLine($"  {a.Id,3} - {a.FirstName} {a.LastName}"));
    }

    private void Add()
    {
        Adresa adresa = ReadAdresa(null);
        _adresaDao.Add(adresa);
        ConsoleOutput.ShowSuccess($"Adresa je uspješno dodata (ID {adresa.Id}).");
    }

    private void Update()
    {
        Adresa? existing = FindById();
        if (existing is null)
        {
            return;
        }

        ConsoleOutput.ShowInfo("Pritisnite Enter da zadržite trenutnu vrijednost (prikazana je u zagradi).");
        Adresa updated = ReadAdresa(existing);
        _adresaDao.Update(updated);
        ConsoleOutput.ShowSuccess("Adresa je uspješno izmijenjena.");
    }

    private void Remove()
    {
        Adresa? adresa = FindById();
        if (adresa is null)
        {
            return;
        }

        int visitorCount = VisitorsAt(adresa.Id).Count;
        int authorCount = AuthorsAt(adresa.Id).Count;
        if (visitorCount > 0 || authorCount > 0)
        {
            ConsoleOutput.ShowError($"Adresu koriste posjetioci: {visitorCount} i autori: {authorCount}. Prvo im promijenite adresu, pa je onda obrišite.");
            return;
        }

        if (!ConsoleInput.Confirm($"Da li ste sigurni da želite da obrišete adresu {ConsoleOutput.FormatAddress(adresa)}?"))
        {
            ConsoleOutput.ShowInfo("Brisanje je otkazano.");
            return;
        }

        _adresaDao.Remove(adresa.Id);
        ConsoleOutput.ShowSuccess("Adresa je uspješno obrisana.");
    }

    private static Adresa ReadAdresa(Adresa? current)
    {
        return new Adresa
        {
            Id = current?.Id ?? 0,
            Street = ConsoleInput.ReadText("Ulica", true, current?.Street),
            Number = ConsoleInput.ReadText("Broj (npr. 12a ili bb)", true, current?.Number),
            City = ConsoleInput.ReadText("Grad", true, current?.City),
            Country = ConsoleInput.ReadText("Država", true, current?.Country)
        };
    }

    private Adresa? FindById()
    {
        int id = ConsoleInput.ReadInt("ID adrese", 1, int.MaxValue);
        Adresa? adresa = _adresaDao.GetById(id);
        if (adresa is null)
        {
            ConsoleOutput.ShowError($"Adresa sa ID-jem {id} ne postoji.");
        }

        return adresa;
    }

    private List<Posetilac> VisitorsAt(int addressId) =>
        _posetilacDao.GetAll().Where(p => p.AddressId == addressId).ToList();

    private List<Autor> AuthorsAt(int addressId) =>
        _autorDao.GetAll().Where(a => a.AddressId == addressId).ToList();
}