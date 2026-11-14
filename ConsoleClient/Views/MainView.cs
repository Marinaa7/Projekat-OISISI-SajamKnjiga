// Po uzoru na vezbe 3 (CRUDExample, VehicleConsoleView.cs)
namespace BookFair.ConsoleClient.Views;

public class MainView
{
    private readonly PosetilacView _posetilacView;
    private readonly KupovinaView _kupovinaView;
    private readonly IzdavacView _izdavacView;

    public MainView(PosetilacView posetilacView, KupovinaView kupovinaView, IzdavacView izdavacView)
    {
        _posetilacView = posetilacView;
        _kupovinaView = kupovinaView;
        _izdavacView = izdavacView;
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
        Console.WriteLine("0 - Izlaz");
    }
}