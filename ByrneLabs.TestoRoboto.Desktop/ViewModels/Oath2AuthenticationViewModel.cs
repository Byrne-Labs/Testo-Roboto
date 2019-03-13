﻿using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class Oath2AuthenticationViewModel
    {
        public string AccessToken { get; set; }

        public string TokenLocation { get; set; }

        public IEnumerable<string> TokenLocations { get; set; }
    }
}
