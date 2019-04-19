using System;
using System.IO;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class BinaryCollectionSerializer : ICollectionSerializer
    {
        public bool BinaryOnly => true;

        public RequestMessageCollection Read(byte[] bytes) => MessagePackSerializer.Deserialize<RequestMessageCollection>(bytes);

        public RequestMessageCollection ReadFromFile(string fileName) => Read(File.ReadAllBytes(fileName));

        public RequestMessageCollection ReadFromString(string collectionText) => throw new NotSupportedException();

        public byte[] Write(RequestMessageCollection requestMessageCollection) => MessagePackSerializer.Serialize(requestMessageCollection);

        public void WriteToFile(RequestMessageCollection requestMessageCollection, string fileName)
        {
            File.WriteAllBytes(fileName, Write(requestMessageCollection));
        }

        public string WriteToString(RequestMessageCollection requestMessageCollection) => throw new NotSupportedException();
    }
}
