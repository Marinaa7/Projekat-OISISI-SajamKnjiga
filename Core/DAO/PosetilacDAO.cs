// Po uzoru na vezbe 3
using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

public class PosetilacDAO
{
    private readonly List<Posetilac> _posetioci;
    private readonly Storage<Posetilac> _storage;

    public PosetilacDAO()
    {
        _storage = new Storage<Posetilac>("posetioci.txt");
        _posetioci = _storage.Load();
    }

    private int GenerateId()
    {
        if (_posetioci.Count == 0)
        {
            return 1;
        }

        return _posetioci.Max(p => p.Id) + 1;
    }

    public List<Posetilac> GetAll()
    {
        return _posetioci;
    }

    public Posetilac? GetById(int id)
    {
        return _posetioci.Find(p => p.Id == id);
    }

    public Posetilac? GetByCardNumber(string cardNumber)
    {
        return _posetioci.Find(p => p.CardNumber == cardNumber);
    }

    public Posetilac? Add(Posetilac posetilac)
    {
        if (GetByCardNumber(posetilac.CardNumber) != null)
        {
            return null;
        }

        posetilac.Id = GenerateId();
        _posetioci.Add(posetilac);
        _storage.Save(_posetioci);
        return posetilac;
    }

    public Posetilac? Update(Posetilac posetilac)
    {
        Posetilac? oldPosetilac = GetById(posetilac.Id);
        if (oldPosetilac == null)
        {
            return null;
        }

        oldPosetilac.FirstName = posetilac.FirstName;
        oldPosetilac.LastName = posetilac.LastName;
        oldPosetilac.DateOfBirth = posetilac.DateOfBirth;
        oldPosetilac.AddressId = posetilac.AddressId;
        oldPosetilac.Phone = posetilac.Phone;
        oldPosetilac.Email = posetilac.Email;
        oldPosetilac.MembershipYear = posetilac.MembershipYear;
        oldPosetilac.Status = posetilac.Status;
        oldPosetilac.AverageRating = posetilac.AverageRating;

        _storage.Save(_posetioci);
        return oldPosetilac;
    }

    public Posetilac? Remove(int id)
    {
        Posetilac? posetilac = GetById(id);
        if (posetilac == null)
        {
            return null;
        }

        _posetioci.Remove(posetilac);
        _storage.Save(_posetioci);
        return posetilac;
    }
}