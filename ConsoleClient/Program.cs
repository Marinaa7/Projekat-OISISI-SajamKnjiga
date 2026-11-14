using BookFair.ConsoleClient.Views;
using BookFair.Core.DAO;

namespace BookFair.ConsoleClient;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        MainView mainView;
        try
        {
            PosetilacDAO posetilacDao = new PosetilacDAO();
            KupovinaDAO kupovinaDao = new KupovinaDAO();
            IzdavacDAO izdavacDao = new IzdavacDAO();
            AutorDAO autorDao = new AutorDAO();
            KnjigaDAO knjigaDao = new KnjigaDAO();
            AdresaDAO adresaDao = new AdresaDAO();

            mainView = new MainView(
                new PosetilacView(posetilacDao, kupovinaDao, adresaDao),
                new KupovinaView(kupovinaDao, posetilacDao, knjigaDao),
                new IzdavacView(izdavacDao, autorDao, knjigaDao));
        }
        catch (Exception ex)
        {
            ConsoleOutput.ShowError("Podaci nisu mogli biti učitani: " + ex.Message);
            ConsoleOutput.ShowError("Provjerite fajlove u folderu Data i pokrenite aplikaciju ponovo.");
            return;
        }

        mainView.Run();
    }
}