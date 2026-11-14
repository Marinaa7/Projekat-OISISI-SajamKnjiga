// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
namespace BookFair.ConsoleClient.Views;

public class MainView
{
    private readonly PosetilacView _posetilacView;
    private readonly KupovinaView _kupovinaView;
    private readonly IzdavacView _izdavacView;
    private readonly AutorView _autorView;
    private readonly KnjigaView _knjigaView;
    private readonly AdresaView _adresaView;

    public MainView(PosetilacView posetilacView, KupovinaView kupovinaView, IzdavacView izdavacView,
                    AutorView autorView, KnjigaView knjigaView, AdresaView adresaView)
    {
        _posetilacView = posetilacView;
        _kupovinaView = kupovinaView;
        _izdavacView = izdavacView;
        _autorView = autorView;
        _knjigaView = knjigaView;
        _adresaView = adresaView;
    }

    public void Run()
    {
        while (true)
        {
            ShowMenu();
            string choice = ConsoleInput.ReadChoice();

            switch (choice)
            {
                case "1":
                    _posetilacView.Run();
                    break;
                case "2":
                    _kupovinaView.Run();
                    break;
                case "3":
                    _izdavacView.Run();
                    break;
                case "4":
                    _autorView.Run();
                    break;
                case "5":
                    _knjigaView.Run();
                    break;
                case "6":
                    _adresaView.Run();
                    break;
                case "0":
                    Console.WriteLine("Doviđenja!");
                    return;
                default:
                    ConsoleOutput.ShowError("Nepostojeća opcija.");
                    break;
            }
        }
    }

    private static void ShowMenu()
    {
        ConsoleOutput.ShowTitle("SAJAM KNJIGA - GLAVNI MENI");
        Console.WriteLine("1 - Posjetioci");
        Console.WriteLine("2 - Kupovine");
        Console.WriteLine("3 - Izdavači");
        Console.WriteLine("4 - Autori");
        Console.WriteLine("5 - Knjige");
        Console.WriteLine("6 - Adrese");
        Console.WriteLine("0 - Izlaz");
    }
}