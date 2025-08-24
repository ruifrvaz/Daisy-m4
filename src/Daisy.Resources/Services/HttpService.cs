using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Daisy.Resources.Services
{
    public class HttpService : IDisposable
    {
        public class UriParameters
        {
            public string Path { get; set; }
            public string Host { get; set; }
            public string Fragments { get; set; }
            public Dictionary<string, string> HeaderData { get; set; }
            public Dictionary<string, string> QueryParameters { get; set; }
            public string QueryWord { get; set; }

        }

        private bool _alreadyDisposed = false;

        public static HttpClient HttpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        public UriParameters GenerateParameters(string hostname, string path, string queryWord = "", Dictionary<string, string> headerValues = null, Dictionary<string, string> queryParameters = null)
        {
            return new UriParameters()
            {
                HeaderData = headerValues,
                Path = path,
                Host = hostname,
                QueryWord = queryWord,
                QueryParameters = queryParameters,
            };
        }

        public Uri BuildUri(UriParameters uriParameters)
        {

            var uriBuilder = new UriBuilder()
            {
                Host = uriParameters.Host,
                Path = uriParameters.Path,
                Scheme = "https"
            };

            if (uriParameters.QueryParameters != null)
            {
                var queryParameters = HttpUtility.ParseQueryString(string.Empty);

                foreach (var queryParameter in uriParameters.QueryParameters)
                {
                    queryParameters[queryParameter.Key] = queryParameter.Value;
                }
                uriBuilder.Query = queryParameters.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(uriParameters.QueryWord))
                {
                    uriBuilder.Path = string.Join("", uriBuilder.Path, uriParameters.QueryWord);
                }
            }

            uriBuilder.Fragment = (!string.IsNullOrEmpty(uriParameters.Fragments) ? uriParameters.Fragments : "");

            return uriBuilder.Uri;
        }

        public async Task<T> GetJson<T>(Uri url, Dictionary<string, string> headers)
        {
            T model = default(T);

            if (headers != null)
            {
                HttpClient.DefaultRequestHeaders.Clear();

                foreach (var header in headers)
                {
                    HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                HttpClient.DefaultRequestHeaders.Add("Accept", new MediaTypeWithQualityHeaderValue("application/json").ToString());
            }

            using (var response = await HttpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    using HttpContent content = response.Content;

                    // ... Read the content.
                    var result = await content.ReadAsStringAsync();

                    // ... Store the result.
                    if (result != null)
                    {
                        model = JsonSerializer.Deserialize<T>(result);
                    }
                }
            }

            return model;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // prevents a redundant dispose that is always performed by gc.
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
            {
                return;
            }
            if (isDisposing)
            {
                HttpClient?.Dispose();
            }
            _alreadyDisposed = true;
        }
    }

}