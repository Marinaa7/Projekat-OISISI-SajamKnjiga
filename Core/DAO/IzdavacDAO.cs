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

    private int GenerateId()
    {
        if (_izdavaci.Count == 0)
        {
            return 1;
        }

        return _izdavaci.Max(i => i.Id) + 1;
    }

    public List<Izdavac> GetAll()
    {
        return _izdavaci;
    }

    public Izdavac? GetById(int id)
    {
        return _izdavaci.Find(i => i.Id == id);
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

        izdavac.Id = GenerateId();
        _izdavaci.Add(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }

    public Izdavac? Update(Izdavac izdavac)
    {
        Izdavac? oldIzdavac = GetById(izdavac.Id);
        if (oldIzdavac == null)
        {
            return null;
        }

        oldIzdavac.Name = izdavac.Name;
        oldIzdavac.HeadAuthorId = izdavac.HeadAuthorId;

        _storage.Save(_izdavaci);
        return oldIzdavac;
    }

    public Izdavac? Remove(int id)
    {
        Izdavac? izdavac = GetById(id);
        if (izdavac == null)
        {
            return null;
        }

        _izdavaci.Remove(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }
}