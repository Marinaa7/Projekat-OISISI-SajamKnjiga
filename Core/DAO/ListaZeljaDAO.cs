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

    public List<int> GetBookIds(int visitorId)
    {
        return _listeZelja.Where(lz => lz.VisitorId == visitorId).Select(lz => lz.BookId).ToList();
    }

    public List<int> GetVisitorIds(int bookId)
    {
        return _listeZelja.Where(lz => lz.BookId == bookId).Select(lz => lz.VisitorId).ToList();
    }

    public bool Contains(int visitorId, int bookId)
    {
        return _listeZelja.Exists(lz => lz.VisitorId == visitorId && lz.BookId == bookId);
    }

    public ListaZelja? Add(int visitorId, int bookId)
    {
        if (Contains(visitorId, bookId))
        {
            return null;
        }

        ListaZelja listaZelja = new ListaZelja
        {
            VisitorId = visitorId,
            BookId = bookId
        };

        _listeZelja.Add(listaZelja);
        _storage.Save(_listeZelja);
        return listaZelja;
    }

    public ListaZelja? Remove(int visitorId, int bookId)
    {
        ListaZelja? listaZelja = _listeZelja.Find(lz => lz.VisitorId == visitorId && lz.BookId == bookId);
        if (listaZelja == null)
        {
            return null;
        }

        _listeZelja.Remove(listaZelja);
        _storage.Save(_listeZelja);
        return listaZelja;
    }

    public void RemoveByVisitor(int visitorId)
    {
        _listeZelja.RemoveAll(lz => lz.VisitorId == visitorId);
        _storage.Save(_listeZelja);
    }

    public void RemoveByBook(int bookId)
    {
        _listeZelja.RemoveAll(lz => lz.BookId == bookId);
        _storage.Save(_listeZelja);
    }
}