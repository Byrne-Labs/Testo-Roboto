using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace ByrneLabs.TestoRoboto.HttpServices.Tests
{
    public class HarSerializerTest
    {
        [Fact]
        public void TestHar1Serialize()
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("TestData\\Sample1.har");

            Assert.NotEmpty(collection.Items);
            var requestMessage = (RequestMessage) collection.Items.First();
            Assert.Equal("https://www.ford.com/cmslibs/content/dam/vdm_ford/live/en_us/ford/nameplate/ranger/2019/collections/3-2/2019-Ranger-280x121.png/_jcr_content/renditions/cq5dam.web.768.768.png", requestMessage.Uri.ToString());
            Assert.Equal(HttpMethod.Get, requestMessage.HttpMethod);
            Assert.Equal(8, requestMessage.Headers.Count);
            Assert.Equal("Host", requestMessage.Headers.First().Key);
            Assert.Equal("www.ford.com", requestMessage.Headers.First().Value);
            Assert.Equal(19, requestMessage.Cookies.Count);
            Assert.Equal("s_fid", requestMessage.Cookies.First().Name);
            Assert.Equal("1E814B2A3EC563C3-1AF8BDDAA707DC58", requestMessage.Cookies.First().Value);
            Assert.Single(requestMessage.ResponseMessages);
            Assert.Equal(HttpStatusCode.OK, requestMessage.ResponseMessages.Single().StatusCode);
            Assert.Equal(12, requestMessage.ResponseMessages.Single().Headers.Count);
            Assert.Equal("X-Firefox-Spdy", requestMessage.ResponseMessages.Single().Headers[11].Key);
            Assert.Equal("h2", requestMessage.ResponseMessages.Single().Headers[11].Value);
            Assert.Empty(requestMessage.ResponseMessages.Single().Cookies);

            var requestMessage2 = (RequestMessage) collection.Items[8];
            Assert.Single(requestMessage2.ResponseMessages.Single().Cookies);
            Assert.Equal("userInfo", requestMessage2.ResponseMessages.Single().Cookies.Single().Name);
            Assert.Equal("country_code=US,region_code=KY,city=LOUISVILLE,county=JEFFERSON,zip=40201-40225+40228-40229+40231-40233+40241-40243+40245+40250-40253+40255-40259+40261+40266+40268-40270+40272+40280-40283+40285+40287+40289-40299", requestMessage2.ResponseMessages.Single().Cookies.Single().Value);

            Assert.Equal("(function(g,n,p){n.assign(FD.Brand.namespace(\"FD.Brand.Metrics\",g),{provider:d,getParameter:l,setParameter:v,check:b,updateVehicle:i,spcFlags:{set:a,reset:o,tabDisplayed:t}});\nvar j={US:{\"transit connect commercial\":\"van-comm\",strippedchassis:\"stripped chassis-comm\",\"transit chassis\":\"transit\",\"econoline cutaway\":\"e-series cutaway-comm\",\"econoline chassis\":\"e-series chassis-comm\",\"transit commercial\":\"van-comm\",\"chassis cab\":\"chassis cab-comm\",\"superduty commercial\":\"super duty-comm\",\"f-650-750\":\"f-650-750-comm\"},CA:{\"transit connect commercial\":\"transit connect-comm\",strippedchassis:\"stripped chassis-comm\",\"transit chassis\":\"transit-chassis\",\"econoline cutaway\":\"e-series cutaway-comm\",\"econoline chassis\":\"e-series chassis-comm\",\"transit commercial\":\"transit-comm\",\"chassis cab\":\"chassis cab-comm\",\"superduty commercial\":\"super duty-comm\",\"f-650-750\":\"f-650-750-comm\",\"f-150 commercial\":\"f-150\"}};\nvar c={finance:false,lease:false};\nvar q={brochures:\"brochures\",guides:\"guides\",manuals:\"manuals\"};\nvar u={page_radUIVersion:function(){g.radUIVersion=s();\nreturn g.radUIVersion\n},page_userLanguage:{en:\"eng\",es:\"esp\",fr:\"fr\"}[p.language||\"en\"],page_siteSearchString:\"\",page_client:{USFord:\"ford-us\",USLincoln:\"lincoln\",CAFord:\"ford-canada\",CALincoln:\"canada\"}[p.region+p.make],page_site:{USFord:\"fordvehicles.com\",USLincoln:\"lincoln.com\",CAFord:\"ford.ca\",CALincoln:\"lincolncanada.com\"}[p.region+p.make],vehicle_modelYear:p.nameplate&&p.nameplate.ngpYear,vehicle_nameplate:p.nameplate&&(p.make+\" \"+(p.nameplate.metricsName||p.nameplate.ngpModelName)).toLowerCase(),vehicle_bodyModelTrim:p.model&&p.model.ngpTrimName,\"$sitePrefix\":{USFord:\"fv\",USLincoln:\"ln\",CAFord:\"foc\",CALincoln:\"lnc\"}[p.region+p.make],\"$make\":p.make&&p.make.toLowerCase(),\"$nameplate\":p.nameplate&&(p.nameplate.metricsName||p.nameplate.ngpModelName).toLowerCase(),\"$modelYear\":p.nameplate&&p.nameplate.ngpYear,\"$$dealerPrefix\":\"dd\",\"$$brochuresTab\":\"brochures\"};\ne();\ng.radUIVersion=s();\nif(sessionStorage[\"fgx-lad-ownerCtx\"]){u.$$dealerContext=\"owner\";\nu.$$dealerPrefix=\"flmo\"\n}var k=f();\nif(k!==\"\"){u.$$brochuresTab=q[k.toLowerCase()]||\"brochures\"\n}r();\nfunction r(){n.ready(function(){var w=g.FD.Brand.User;\nw.authentication.subscribe(function(x){u.user_loginStatus=(x.authType===\"user\")?\"logged in\":\"logged out\"\n});\nw.location.subscribe(function(){w.location.detail().done(function(y){if(y){var x={};\nif(y.regions){n.assign(u,{user_fdafCode:(p.make===\"Lincoln\")?y.regions.LMDA:y.regions.FDAF,user_fordRegion:y.regions.Marketing});\nx.Marketing=y.regions.Marketing;\nx.FDAF=y.regions.FDAF;\nx.LMDA=y.regions.LMDA\n}if(!p.settings.syn){x.zip=(w.location.current()&&w.location.current().postalCode)?w.location.current().postalCode:\"\";\nx.PACode=(y.paCode)?y.paCode:\"\";\nh(x)\n}}})\n})\n})\n}function d(w){if(w in u){var x=u[w];\nreturn(typeof x===\"function\")?x():x\n}}function l(w){return u[w]\n}function v(w,x){u[w]=x\n}function b(){if(!(g.FD&&FD.Common&&FD.Common.Metrics&&FD.Brand.Metrics.handler)){var w=function(){};\nFD.Brand.Metrics.handler={page:w,direct:w,data:w}\n}}function m(w){var x=j[p.region][w];\nif(x){v(\"vehicle_nameplate\",(p.make+\" \"+x).toLowerCase());\nv(\"$nameplate\",x.toLowerCase())\n}}function e(){var w=false;\nswitch(p.region+\"-\"+u.$nameplate){case\"CA-f-150\":if(p.nameplate&&p.nameplate.ngpModelName&&p.nameplate.ngpModelName.indexOf(\"Commercial\")>-1){u.$segment=\"truck-commercial\"\n}else{u.$segment=\"truck\"\n}w=true;\nbreak;\ncase\"US-transit\":if(p.nameplate&&p.nameplate.metricsName&&p.nameplate.metricsName===u.$nameplate){u.$segment=\"truck-commercial\";\nw=true\n}break\n}if(!w){switch(u.$nameplate){case\"ecosport\":case\"flex\":case\"edge\":case\"mkt\":case\"mkc\":case\"mkx\":case\"nautilus\":case\"transit connect wagon\":u.$segment=\"crossover\";\nbreak;\ncase\"escape\":case\"explorer\":case\"expedition\":case\"navigator\":case\"aviator\":u.$segment=\"suv\";\nbreak;\ncase\"f-150\":case\"f-series\":case\"superduty\":case\"super duty\":case\"ranger\":case\"transit connect\":case\"transit vanwagon\":case\"van\":case\"transit\":u.$segment=\"truck\";\nbreak;\ncase\"strippedchassis\":case\"stripped chassis-comm\":case\"econoline chassis\":case\"econoline cutaway\":case\"e-series cutaway-comm\":case\"transit chassis\":case\"transit-chassis\":case\"transit commercial\":case\"transit-comm\":case\"van-comm\":case\"chassis cab\":case\"chassis cab-comm\":case\"f-650-750\":case\"f-650-750-comm\":case\"superduty commercial\":case\"super duty-comm\":case\"transit connect commercial\":case\"transit connect-comm\":case\"f-150 commercial\":u.$segment=\"truck-commercial\";\nif((p.nameplate&&(!p.nameplate.metricsName||(p.nameplate.metricsName!==u.$nameplate)))||!p.nameplate){m(u.$nameplate)\n}break;\ndefault:u.$segment=\"car\";\nbreak\n}}}function i(x,y,z){if(!x||!y||!z){return\n}var w=x.toLowerCase(),A=y.toLowerCase();\nv(\"vehicle_nameplate\",w+\" \"+A);\nv(\"vehicle_modelYear\",z);\nv(\"$make\",w);\nv(\"$nameplate\",A);\nv(\"$modelYear\",z);\ne()\n}function t(w){return c[w]||false\n}function o(){c.finance=false;\nc.lease=false\n}function a(x,w){c[x]=w\n}function h(A){var w=FD.Brand&&FD.Brand.Util;\nvar x=w.cookie.get(\"regions\");\nvar z=(x)?w.parameters.decode(x):{};\nvar y=p.settings[\"siteCookieDomain\"+p.make];\nz=n.assign(z,A);\nw.cookie.set(\"regions\",w.parameters.encode(z),{path:\"/\",domain:((y||\"\")==\"\")?null:y,expires:65})\n}function s(){var w=g.innerWidth;\nreturn\"ui:rad:\"+((w<768)?\"mobile\":((w<992)?\"tablet\":\"pc\"))\n}function f(){var w=g.location.hash,x=\"\";\nif(w){x=w.substr(1).trim()\n}return x\n}})(window,FD.Common,FD.Brand.Context);",
                requestMessage2.ResponseMessages.Single().Content);
        }

        [Fact]
        public void TestHar2Serialize()
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("TestData\\Sample2.har");

            Assert.NotEmpty(collection.Items);
            var requestMessage = (RequestMessage) collection.Items.First();
            Assert.Equal(HttpMethod.Post, requestMessage.HttpMethod);
            Assert.IsType<RawBody>(requestMessage.Body);
        }

        [Fact]
        public void TestHar3Serialize()
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("TestData\\Sample3.har");

            Assert.Equal(2, collection.Items.Count);
        }
    }
}
