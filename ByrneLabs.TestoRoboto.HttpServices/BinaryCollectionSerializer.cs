using System;
using System.IO;
using MessagePack;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class BinaryCollectionSerializer : ICollectionSerializer
    {
        public bool BinaryOnly => true;

        public Collection Read(byte[] bytes) => MessagePackSerializer.Deserialize<Collection>(bytes);

        public Collection ReadFromFile(string fileName) => Read(File.ReadAllBytes(fileName));

        public Collection ReadFromString(string collectionText) => throw new NotSupportedException();

        public byte[] Write(Collection collection) => MessagePackSerializer.Serialize(collection);

        public void WriteToFile(Collection collection, string fileName)
        {
            File.WriteAllBytes(fileName, Write(collection));
        }

        public string WriteToString(Collection collection) => throw new NotSupportedException();
    }
}
