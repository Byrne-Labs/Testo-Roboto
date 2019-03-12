using System;
using System.IO;
using ByrneLabs.Serializer;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class BinaryCollectionSerializer : ICollectionSerializer
    {
        public bool BinaryOnly => true;

        public Collection Read(byte[] bytes) => ByrneLabsSerializer.Deserialize<Collection>(bytes);

        public Collection ReadFromFile(string fileName) => Read(File.ReadAllBytes(fileName));

        public Collection ReadFromString(string collectionText) => throw new NotSupportedException();

        public byte[] Write(Collection collection) => ByrneLabsSerializer.Serialize(collection);

        public void WriteToFile(Collection collection, string fileName)
        {
            File.WriteAllBytes(fileName, Write(collection));
        }

        public string WriteToString(Collection collection) => throw new NotSupportedException();
    }
}
