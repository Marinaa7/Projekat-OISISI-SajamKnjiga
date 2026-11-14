// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class KnjigaAutorDAO
{
    private readonly Storage<KnjigaAutor> _storage;
    private readonly List<KnjigaAutor> _knjigeAutori;

    public KnjigaAutorDAO()
    {
        _storage = new Storage<KnjigaAutor>("knjigeAutori.txt");
        _knjigeAutori = _storage.Load();
    }

    public List<KnjigaAutor> GetAll() => _knjigeAutori;

    public List<int> GetAuthorIds(string isbn) =>
        _knjigeAutori.Where(ka => ka.BookIsbn == isbn).Select(ka => ka.AuthorId).ToList();

    public List<string> GetBookIsbns(int authorId) =>
        _knjigeAutori.Where(ka => ka.AuthorId == authorId).Select(ka => ka.BookIsbn).ToList();

    public void SetAuthors(string isbn, List<int> authorIds)
    {
        _knjigeAutori.RemoveAll(ka => ka.BookIsbn == isbn);

        foreach (int authorId in authorIds.Distinct())
        {
            _knjigeAutori.Add(new KnjigaAutor { BookIsbn = isbn, AuthorId = authorId });
        }

        _storage.Save(_knjigeAutori);
    }

    public void RemoveByBook(string isbn)
    {
        _knjigeAutori.RemoveAll(ka => ka.BookIsbn == isbn);
        _storage.Save(_knjigeAutori);
    }
}