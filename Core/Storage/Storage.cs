// Preuzeto sa vežbi: Vežbe 3 – SerializationExample, Storage/Storage.cs (izmenjena putanja do Data direktorijuma)
using BookFair.Core.Serialization;

namespace BookFair.Core.Storage;

public class Storage<T> where T : ISerializable, new()
{
    private readonly string _filePath;
    private readonly Serializer<T> _serializer = new();

    public Storage(string fileName)
    {
        string relativeDataPath = string.Join(Path.DirectorySeparatorChar, "..", "..", "..", "..", "Data");
        string dataDirectory = Path.GetFullPath(AppContext.BaseDirectory + relativeDataPath);
        Directory.CreateDirectory(dataDirectory);
        _filePath = dataDirectory + Path.DirectorySeparatorChar + fileName;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            FileStream fs = File.Create(_filePath);
            fs.Close();
        }

        IEnumerable<string> lines = File.ReadLines(_filePath);
        List<T> objects = _serializer.FromCSV(lines);

        return objects;
    }

    public void Save(List<T> objects)
    {
        string serializedObjects = _serializer.ToCSV(objects);
        using (StreamWriter streamWriter = new StreamWriter(_filePath))
        {
            streamWriter.Write(serializedObjects);
        }
    }
}
