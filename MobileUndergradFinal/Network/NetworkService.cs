using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public string BearerToken { get; set; }

        private readonly string _address;

        public NetworkService([NotNull] string address)
        {
            _address = address;
        }

        public async Task GetAsync<T>([NotNull] string path, Action<T> onSuccess, Action<List<string>> onError) where T : class
        {
            await MakeRequestAsync(HttpMethod.Get, path, onSuccess, onError);
        }

        public async Task PostAsync<T>([NotNull] string path, object body, Action<T> onSuccess, Action<List<string>> onError) where T : class
        {
            await MakeRequestAsync(HttpMethod.Post, path, onSuccess, onError, body);
        }

        private async Task MakeRequestAsync<T>([NotNull] HttpMethod method, [NotNull] string path, Action<T> onSuccess, Action<List<string>> onError, object body = null) where T : class
        {
            HttpResponseMessage response;
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(method, _address + "/" + path);
                if(!string.IsNullOrWhiteSpace(BearerToken))
                    request.Headers.Add("Authorization", "Bearer " + BearerToken);
                Debug.WriteLine(_address + "/" + path);
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
                onError?.Invoke(new List<string>{e.Message});
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
                    if (typeof(T) == typeof(string))
                        onSuccess?.Invoke(res as T);
                    else
                        onSuccess?.Invoke(JsonConvert.DeserializeObject<T>(res));
                }
            }
            else
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        onError?.Invoke(new List<string>{"401 Unauthorized"});
                        break;
                    case HttpStatusCode.InternalServerError:
                        onError?.Invoke(new List<string> { "500 Internal server error" });
                        break;
                    case HttpStatusCode.BadRequest:
                    {
                        var res = await response.Content.ReadAsStringAsync();
                        onError?.Invoke(JsonConvert.DeserializeObject<List<string>>(res));
                        break;
                    }
                    default:
                    {
                        var res = await response.Content.ReadAsStringAsync();
                        onError?.Invoke(new List<string>{res});
                        break;
                    }

                }
            }
        }
    }
}
