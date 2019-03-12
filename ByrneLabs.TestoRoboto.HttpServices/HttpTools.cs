using System.Net.Http;

namespace ByrneLabs.TestoRoboto.HttpServices
{
    public static class HttpTools
    {
        public static HttpMethod HttpMethodFromString(string httpMethodString)
        {
            HttpMethod httpMethod;
            switch (httpMethodString)
            {
                case "GET":
                    httpMethod = HttpMethod.Get;
                    break;
                case "POST":
                    httpMethod = HttpMethod.Post;
                    break;
                case "PUT":
                    httpMethod = HttpMethod.Put;
                    break;
                case "DELETE":
                    httpMethod = HttpMethod.Delete;
                    break;
                case "HEAD":
                    httpMethod = HttpMethod.Head;
                    break;
                case "OPTIONS":
                    httpMethod = HttpMethod.Options;
                    break;
                case "TRACE":
                    httpMethod = HttpMethod.Trace;
                    break;
                default:
                    httpMethod = new HttpMethod(httpMethodString);
                    break;
            }

            return httpMethod;
        }
    }
}
