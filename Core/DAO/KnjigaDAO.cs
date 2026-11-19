// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class KnjigaDAO
{
    private readonly Storage<Knjiga> _storage;
    private readonly List<Knjiga> _knjige;

    public KnjigaDAO()
    {
        _storage = new Storage<Knjiga>("knjige.txt");
        _knjige = _storage.Load();
    }

    public List<Knjiga> GetAll() => _knjige;

    public Knjiga? GetById(int id) => _knjige.FirstOrDefault(k => k.Id == id);

    public Knjiga? GetByIsbn(string isbn) => _knjige.FirstOrDefault(k => k.Isbn == isbn);

    public Knjiga? Add(Knjiga knjiga)
    {
        if (GetByIsbn(knjiga.Isbn) is not null)
        {
            return null;
        }

        knjiga.Id = NextId();
        _knjige.Add(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    public Knjiga? Update(Knjiga knjiga)
    {
        Knjiga? existing = GetById(knjiga.Id);
        if (existing is null)
        {
            return null;
        }

        existing.Title = knjiga.Title;
        existing.Genre = knjiga.Genre;
        existing.PublicationYear = knjiga.PublicationYear;
        existing.Price = knjiga.Price;
        existing.PageCount = knjiga.PageCount;
        existing.PublisherId = knjiga.PublisherId;

        _storage.Save(_knjige);
        return existing;
    }

    public Knjiga? Remove(int id)
    {
        Knjiga? existing = GetById(id);
        if (existing is null)
        {
            return null;
        }

        _knjige.Remove(existing);
        _storage.Save(_knjige);
        return existing;
    }

    private int NextId() => _knjige.Count == 0 ? 1 : _knjige.Max(k => k.Id) + 1;
}