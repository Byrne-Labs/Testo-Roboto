namespace ByrneLabs.TestoRoboto.HttpServices
{
    public interface ICollectionSerializer
    {
        bool BinaryOnly { get; }

        RequestMessageCollection Read(byte[] bytes);

        RequestMessageCollection ReadFromFile(string fileName);

        RequestMessageCollection ReadFromString(string collectionText);

        byte[] Write(RequestMessageCollection requestMessageCollection);

        void WriteToFile(RequestMessageCollection requestMessageCollection, string fileName);

        string WriteToString(RequestMessageCollection requestMessageCollection);
    }
}
