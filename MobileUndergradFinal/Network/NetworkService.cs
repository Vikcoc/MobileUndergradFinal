using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Network.Environment;
using Network.Response;
using Newtonsoft.Json;

namespace Network
{
    public class NetworkService
    {
        public string BearerToken { get; set; }

        private readonly string _address;

        public NetworkService()
        {
            _address = EnvironmentSettings.Instance.Information.BackendAddress;
        }

        public async Task<NetworkResponse<T>> GetAsync<T>([NotNull] string path) where T : class
        {
            return await MakeRequestAsync<T>(HttpMethod.Get, path);
        }

        public async Task<NetworkResponse<T>> PostAsync<T>([NotNull] string path, object body) where T : class
        {
            return await MakeRequestAsync<T>(HttpMethod.Post, path, body);
        }

        private async Task<NetworkResponse<T>> MakeRequestAsync<T>([NotNull] HttpMethod method, [NotNull] string path, object body = null) where T : class
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
                return new NetworkResponse<T>
                {
                    ErrorType = ErrorType.NonActionable,
                    Error = e.Message
                };
            }

            if (response?.StatusCode < HttpStatusCode.BadRequest)
            {
                if (typeof(T) == typeof(Stream))
                {
                    return new NetworkResponse<T>
                    {
                        ErrorType = ErrorType.None,
                        Data = await response.Content.ReadAsStreamAsync() as T
                    };
                }

                var res = await response.Content.ReadAsStringAsync();
                if (typeof(T) == typeof(string))
                    return new NetworkResponse<T>
                    {
                        ErrorType = ErrorType.None,
                        Data = res as T
                    };
                return new NetworkResponse<T>
                {
                    ErrorType = ErrorType.None,
                    Data = JsonConvert.DeserializeObject<T>(res)
                };
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new NetworkResponse<T>
                    {
                        ErrorType = ErrorType.Actionable,
                        Error = "401 Unauthorized"
                    };
                case HttpStatusCode.BadRequest:
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return new NetworkResponse<T>
                    {
                        ErrorType = ErrorType.Actionable,
                        Error = res
                    };
                }
                default:
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return new NetworkResponse<T>
                    {
                        ErrorType = ErrorType.NonActionable,
                        Error = res
                    };
                    }

            }
        }
    }
}
