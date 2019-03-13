using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class AwsSignatureAuthenticationViewModel
    {
        public string AccessKey { get; set; }

        public string Region { get; set; }

        public string SecretKey { get; set; }

        public string ServiceName { get; set; }

        public string SessionToken { get; set; }

    }
}
