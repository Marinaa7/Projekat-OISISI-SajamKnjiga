// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class ListaZeljaDAO
{
    private readonly List<ListaZelja> _listeZelja;
    private readonly Storage<ListaZelja> _storage;

    public ListaZeljaDAO()
    {
        _storage = new Storage<ListaZelja>("listeZelja.txt");
        _listeZelja = _storage.Load();
    }

    public List<ListaZelja> GetAll()
    {
        return _listeZelja;
    }

    public List<string> GetBookIsbns(string cardNumber)
    {
        return _listeZelja.Where(lz => lz.VisitorCardNumber == cardNumber).Select(lz => lz.BookIsbn).ToList();
    }

    public List<string> GetVisitorCardNumbers(string isbn)
    {
        return _listeZelja.Where(lz => lz.BookIsbn == isbn).Select(lz => lz.VisitorCardNumber).ToList();
    }

    public bool Contains(string cardNumber, string isbn)
    {
        return _listeZelja.Exists(lz => lz.VisitorCardNumber == cardNumber && lz.BookIsbn == isbn);
    }

    public ListaZelja? Add(string cardNumber, string isbn)
    {
        if (Contains(cardNumber, isbn))
        {
            return null;
        }

        ListaZelja listaZelja = new ListaZelja
        {
            VisitorCardNumber = cardNumber,
            BookIsbn = isbn
        };

        _listeZelja.Add(listaZelja);
        _storage.Save(_listeZelja);
        return listaZelja;
    }

    public ListaZelja? Remove(string cardNumber, string isbn)
    {
        ListaZelja? listaZelja = _listeZelja.Find(lz => lz.VisitorCardNumber == cardNumber && lz.BookIsbn == isbn);
        if (listaZelja == null)
        {
            return null;
        }

        _listeZelja.Remove(listaZelja);
        _storage.Save(_listeZelja);
        return listaZelja;
    }

    public void RemoveByVisitor(string cardNumber)
    {
        _listeZelja.RemoveAll(lz => lz.VisitorCardNumber == cardNumber);
        _storage.Save(_listeZelja);
    }

    public void RemoveByBook(string isbn)
    {
        _listeZelja.RemoveAll(lz => lz.BookIsbn == isbn);
        _storage.Save(_listeZelja);
    }
}