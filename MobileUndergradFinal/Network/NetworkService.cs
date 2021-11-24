using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Network
{
    public class NetworkService : INetworkService
    {
        private readonly Action<List<string>> _onError;
        private readonly string _address;

        public NetworkService([NotNull] string address, Action<List<string>> onError)
        {
            _address = address;
            _onError = onError;
        }

        public async Task GetAsync<T>([NotNull] string path, Action<T> onSuccess) where T : class, new()
        {
            await MakeRequestAsync<T>(HttpMethod.Get, path, onSuccess);
        }

        private async Task MakeRequestAsync<T>([NotNull] HttpMethod method, [NotNull] string path, Action<T> onSuccess, object body = null) where T : class, new()
        {
            HttpResponseMessage response = null;
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(method, _address + "/" + path);
                switch (body)
                {
                    case null:
                        request.Content = null;
                        break;
                    case Stream stream:
                    {
                        var cont = new MultipartFormDataContent();
                        var payload = new StreamContent(stream);
                        payload.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                        cont.Add(payload, "picture", "Picture");
                        request.Content = cont;
                        break;
                    }
                    default:
                    {
                        var cont = new StringContent(JsonConvert.SerializeObject(body));
                        cont.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        request.Content = cont;
                        break;
                    }
                }

                response = await client.SendAsync(request);

            }
            catch (Exception e)
            {
                _onError?.Invoke(new List<string>{e.Message});
                return;
            }

            if (response?.StatusCode < HttpStatusCode.BadRequest)
            {
                if (typeof(T) == typeof(Stream))
                {
                    onSuccess?.Invoke(await response.Content.ReadAsStreamAsync() as T);
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    onSuccess?.Invoke(JsonConvert.DeserializeObject<T>(res));
                }
            }
        }
    }
}
