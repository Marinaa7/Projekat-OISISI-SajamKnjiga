// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class AdresaDAO
{
    private readonly Storage<Adresa> _storage;
    private readonly List<Adresa> _adrese;

    public AdresaDAO()
    {
        _storage = new Storage<Adresa>("adrese.txt");
        _adrese = _storage.Load();
    }

    public List<Adresa> GetAll() => _adrese;

    public Adresa? GetById(int id) => _adrese.FirstOrDefault(a => a.Id == id);

    public Adresa Add(Adresa adresa)
    {
        adresa.Id = NextId();
        _adrese.Add(adresa);
        _storage.Save(_adrese);
        return adresa;
    }

    public Adresa? Update(Adresa adresa)
    {
        Adresa? existing = GetById(adresa.Id);
        if (existing is null)
        {
            return null;
        }

        existing.Street = adresa.Street;
        existing.Number = adresa.Number;
        existing.City = adresa.City;
        existing.Country = adresa.Country;

        _storage.Save(_adrese);
        return existing;
    }

    public Adresa? Remove(int id)
    {
        Adresa? existing = GetById(id);
        if (existing is null)
        {
            return null;
        }

        _adrese.Remove(existing);
        _storage.Save(_adrese);
        return existing;
    }

    private int NextId() => _adrese.Count == 0 ? 1 : _adrese.Max(a => a.Id) + 1;
}