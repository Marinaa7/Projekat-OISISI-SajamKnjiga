// Preuzeto sa vežbi: Vežbe 3 – SerializationExample, Serialization/Serializer.cs (dodato preskakanje praznih linija i odbijanje vrednosti sa | ili prelomom reda)
using System.Text;

namespace BookFair.Core.Serialization;

class Serializer<T> where T : ISerializable, new()
{
    private const char Delimiter = '|';
    private static readonly char[] UnsupportedCharacters = { Delimiter, '\r', '\n' };

    public string ToCSV(List<T> objects)
    {
        StringBuilder sb = new StringBuilder();

        foreach (ISerializable obj in objects)
        {
            string[] csvValues = obj.ToCSV();
            EnsureValuesCanBeSaved(csvValues);
            string line = string.Join(Delimiter.ToString(), csvValues);
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    public List<T> FromCSV(IEnumerable<string> lines)
    {
        List<T> objects = new List<T>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] csvValues = line.Split(Delimiter);
            T obj = new T();
            obj.FromCSV(csvValues);
            objects.Add(obj);
        }

        return objects;
    }

    private static void EnsureValuesCanBeSaved(string[] csvValues)
    {
        for (int i = 0; i < csvValues.Length; i++)
        {
            if (csvValues[i] != null && csvValues[i].IndexOfAny(UnsupportedCharacters) >= 0)
            {
                throw new InvalidOperationException($"Field {i} of {typeof(T).Name} contains '{Delimiter}' or a line break and cannot be saved: '{csvValues[i]}'.");
            }
        }
    }
}
