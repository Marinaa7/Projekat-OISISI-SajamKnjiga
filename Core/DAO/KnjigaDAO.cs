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

    public Knjiga? GetByIsbn(string isbn) => _knjige.FirstOrDefault(k => k.Isbn == isbn);

    public Knjiga? Add(Knjiga knjiga)
    {
        if (GetByIsbn(knjiga.Isbn) is not null)
        {
            return null;
        }

        _knjige.Add(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    public Knjiga? Update(Knjiga knjiga)
    {
        Knjiga? existing = GetByIsbn(knjiga.Isbn);
        if (existing is null)
        {
            return null;
        }

        existing.Title = knjiga.Title;
        existing.Genre = knjiga.Genre;
        existing.PublicationYear = knjiga.PublicationYear;
        existing.Price = knjiga.Price;
        existing.PageCount = knjiga.PageCount;
        existing.PublisherCode = knjiga.PublisherCode;

        _storage.Save(_knjige);
        return existing;
    }

    public Knjiga? Remove(string isbn)
    {
        Knjiga? existing = GetByIsbn(isbn);
        if (existing is null)
        {
            return null;
        }

        _knjige.Remove(existing);
        _storage.Save(_knjige);
        return existing;
    }
}