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

    public List<Posetilac> GetAll()
    {
        return _posetioci;
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

        _posetioci.Add(posetilac);
        _storage.Save(_posetioci);
        return posetilac;
    }

    public Posetilac? Update(Posetilac posetilac)
    {
        Posetilac? oldPosetilac = GetByCardNumber(posetilac.CardNumber);
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

    public Posetilac? Remove(string cardNumber)
    {
        Posetilac? posetilac = GetByCardNumber(cardNumber);
        if (posetilac == null)
        {
            return null;
        }

        _posetioci.Remove(posetilac);
        _storage.Save(_posetioci);
        return posetilac;
    }
}