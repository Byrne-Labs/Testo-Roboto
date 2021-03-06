﻿using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class Oauth2AuthenticationViewModel : AuthenticationViewModel
    {
        public string AccessToken { get; set; }

        public override string Name => "OAuth V2";

        public string TokenLocation { get; set; } = "Headers";

        public IEnumerable<string> TokenLocations { get; } = new[] { "Headers", "Body/URL" };
    }
}
