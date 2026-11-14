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
            ListaZeljaDAO listaZeljaDao = new ListaZeljaDAO();
            KnjigaAutorDAO knjigaAutorDao = new KnjigaAutorDAO();

            mainView = new MainView(
                new PosetilacView(posetilacDao, kupovinaDao, adresaDao, knjigaDao, listaZeljaDao),
                new KupovinaView(kupovinaDao, posetilacDao, knjigaDao, listaZeljaDao),
                new IzdavacView(izdavacDao, autorDao, knjigaDao),
                new AutorView(autorDao, adresaDao, izdavacDao, knjigaDao, knjigaAutorDao),
                new KnjigaView(knjigaDao, knjigaAutorDao, autorDao, izdavacDao, kupovinaDao, posetilacDao, listaZeljaDao),
                new AdresaView(adresaDao, posetilacDao, autorDao));
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