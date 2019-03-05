using ByrneLabs.Commons.Domain;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public class Item : Entity
    {
        public AuthenticationMethod AuthenticationMethod { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }
}
