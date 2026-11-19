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

    public List<int> GetAuthorIds(int bookId) =>
        _knjigeAutori.Where(ka => ka.BookId == bookId).Select(ka => ka.AuthorId).ToList();

    public List<int> GetBookIds(int authorId) =>
        _knjigeAutori.Where(ka => ka.AuthorId == authorId).Select(ka => ka.BookId).ToList();

    public void SetAuthors(int bookId, List<int> authorIds)
    {
        _knjigeAutori.RemoveAll(ka => ka.BookId == bookId);

        foreach (int authorId in authorIds.Distinct())
        {
            _knjigeAutori.Add(new KnjigaAutor { BookId = bookId, AuthorId = authorId });
        }

        _storage.Save(_knjigeAutori);
    }

    public void RemoveByBook(int bookId)
    {
        _knjigeAutori.RemoveAll(ka => ka.BookId == bookId);
        _storage.Save(_knjigeAutori);
    }
}