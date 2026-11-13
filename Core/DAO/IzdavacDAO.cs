// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class IzdavacDAO
{
    private readonly List<Izdavac> _izdavaci;
    private readonly Storage<Izdavac> _storage;

    public IzdavacDAO()
    {
        _storage = new Storage<Izdavac>("izdavaci.txt");
        _izdavaci = _storage.Load();
    }

    public List<Izdavac> GetAll()
    {
        return _izdavaci;
    }

    public Izdavac? GetByCode(string code)
    {
        return _izdavaci.Find(i => i.Code == code);
    }

    public Izdavac? Add(Izdavac izdavac)
    {
        if (GetByCode(izdavac.Code) != null)
        {
            return null;
        }

        _izdavaci.Add(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }

    public Izdavac? Update(Izdavac izdavac)
    {
        Izdavac? oldIzdavac = GetByCode(izdavac.Code);
        if (oldIzdavac == null)
        {
            return null;
        }

        oldIzdavac.Name = izdavac.Name;
        oldIzdavac.HeadAuthorId = izdavac.HeadAuthorId;

        _storage.Save(_izdavaci);
        return oldIzdavac;
    }

    public Izdavac? Remove(string code)
    {
        Izdavac? izdavac = GetByCode(code);
        if (izdavac == null)
        {
            return null;
        }

        _izdavaci.Remove(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }
}