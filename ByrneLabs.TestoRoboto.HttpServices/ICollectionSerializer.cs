namespace ByrneLabs.TestoRoboto.HttpServices
{
    public interface ICollectionSerializer
    {
        bool BinaryOnly { get; }

        Collection Read(byte[] bytes);

        Collection ReadFromFile(string fileName);

        Collection ReadFromString(string collectionText);

        byte[] Write(Collection collection);

        void WriteToFile(Collection collection, string fileName);

        string WriteToString(Collection collection);
    }
}
