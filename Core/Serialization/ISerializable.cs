// Preuzeto sa vežbi: Vežbe 3 – SerializationExample, Serialization/Serializable.cs
namespace BookFair.Core.Serialization;

public interface ISerializable
{
    string[] ToCSV();

    void FromCSV(string[] values);
}
