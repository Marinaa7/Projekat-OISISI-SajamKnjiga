// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class AutorDAO
{
    private readonly Storage<Autor> _storage;
    private readonly List<Autor> _autori;

    public AutorDAO()
    {
        _storage = new Storage<Autor>("autori.txt");
        _autori = _storage.Load();
    }

    public List<Autor> GetAll() => _autori;

    public Autor? GetById(int id) => _autori.FirstOrDefault(a => a.Id == id);

    public Autor Add(Autor autor)
    {
        autor.Id = NextId();
        _autori.Add(autor);
        _storage.Save(_autori);
        return autor;
    }

    public Autor? Update(Autor autor)
    {
        Autor? existing = GetById(autor.Id);
        if (existing is null)
        {
            return null;
        }

        existing.FirstName = autor.FirstName;
        existing.LastName = autor.LastName;
        existing.DateOfBirth = autor.DateOfBirth;
        existing.AddressId = autor.AddressId;
        existing.Phone = autor.Phone;
        existing.Email = autor.Email;
        existing.IdCardNumber = autor.IdCardNumber;
        existing.YearsOfExperience = autor.YearsOfExperience;

        _storage.Save(_autori);
        return existing;
    }

    public Autor? Remove(int id)
    {
        Autor? existing = GetById(id);
        if (existing is null)
        {
            return null;
        }

        _autori.Remove(existing);
        _storage.Save(_autori);
        return existing;
    }

    private int NextId() => _autori.Count == 0 ? 1 : _autori.Max(a => a.Id) + 1;
}