// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class KupovinaDAO
{
    private readonly List<Kupovina> _kupovine;
    private readonly Storage<Kupovina> _storage;

    public KupovinaDAO()
    {
        _storage = new Storage<Kupovina>("kupovine.txt");
        _kupovine = _storage.Load();
    }

    private int GenerateId()
    {
        if (_kupovine.Count == 0)
        {
            return 1;
        }

        return _kupovine.Max(k => k.Id) + 1;
    }

    public List<Kupovina> GetAll()
    {
        return _kupovine;
    }

    public Kupovina? GetById(int id)
    {
        return _kupovine.Find(k => k.Id == id);
    }

    public Kupovina Add(Kupovina kupovina)
    {
        kupovina.Id = GenerateId();
        _kupovine.Add(kupovina);
        _storage.Save(_kupovine);
        return kupovina;
    }

    public Kupovina? Update(Kupovina kupovina)
    {
        Kupovina? oldKupovina = GetById(kupovina.Id);
        if (oldKupovina == null)
        {
            return null;
        }

        oldKupovina.VisitorCardNumber = kupovina.VisitorCardNumber;
        oldKupovina.BookIsbn = kupovina.BookIsbn;
        oldKupovina.PurchaseDate = kupovina.PurchaseDate;
        oldKupovina.Rating = kupovina.Rating;
        oldKupovina.Comment = kupovina.Comment;

        _storage.Save(_kupovine);
        return oldKupovina;
    }

    public Kupovina? Remove(int id)
    {
        Kupovina? kupovina = GetById(id);
        if (kupovina == null)
        {
            return null;
        }

        _kupovine.Remove(kupovina);
        _storage.Save(_kupovine);
        return kupovina;
    }
}