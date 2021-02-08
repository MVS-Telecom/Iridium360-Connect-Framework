using Iridium360.Connect.Framework.Models;
using Iridium360.Connect.Framework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework
{
    public class Snapshot
    {
        [JsonProperty("Zip")]
        public string Zip { get; set; }

        [JsonProperty("Text")]
        public string Text { get; set; }

        [JsonProperty("Date")]
        public DateTime? Date { get; set; }
    }

    public class DeviceInfo
    {
        [JsonProperty("MacAddress")]
        public string MacAddress { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Serial")]
        public string Serial { get; set; }

        [JsonProperty("Hardware")]
        public string Hardware { get; set; }

        [JsonProperty("Firmware")]
        public string Firmware { get; set; }
    }

    public class Feedback
    {
        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }

        [JsonProperty("Os")]
        public string Os { get; set; }

        [JsonProperty("Device")]
        public string Device { get; set; }

        [JsonProperty("ConnectedDevice")]
        public DeviceInfo ConnectedDevice { get; set; }

        [JsonProperty("VersionHistory")]
        public List<string> VersionHistory { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Snapshots")]
        public List<Snapshot> Snapshots { get; set; }
    }


    internal class Result<T>
    {
        internal T ApiResult { get; set; }
        internal Exception Exception { get; set; }
        internal HttpResponseMessage HttpResponse { get; set; }

        internal void ThrowIfError()
        {
            if (Exception != null)
                throw Exception;
        }
    }

    /// <summary>
    /// HTTP REST клиент для iridium360.ru
    /// </summary>
    public class i360ApiClient : IDisposable
    {
        private readonly string endpoint;
        private readonly HttpClient client;
        private readonly string token;
        private readonly bool shouldDisposeClient = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="serial"></param>
        /// <param name="client"></param>
        /// <param name="endpoint"></param>
        public i360ApiClient(string token, HttpClient client = null, string endpoint = "https://demo.iridium360.ru")
        {
            if (client == null)
            {
                shouldDisposeClient = true;
                client = new HttpClient();
            }

            this.client = client;
            this.endpoint = endpoint;
            this.token = token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        private string GetAuth(string serial) => Md5.Get($"{serial}#{token}");

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        private async Task<Result<T>> MakeApiRequest<T>(string actionName, string serial, Dictionary<string, string> @params = null)
        {
            try
            {
                if (@params == null)
                    @params = new Dictionary<string, string>();

                @params.Add("auth", GetAuth(serial));


                string _params = string.Join("&", @params.Select(x => $"{x.Key}={x.Value}"));
                string url = $"{endpoint}/connect/{actionName}?{_params}";


                var response = await client.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    return new Result<T>()
                    {
                        Exception = e,
                        HttpResponse = response
                    };
                }


                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);

                return new Result<T>()
                {
                    ApiResult = result,
                };
            }
            catch (Exception ex)
            {
                return new Result<T>()
                {
                    Exception = ex
                };
            }
        }

        private async Task<Result<T>> MakePostApiRequest<T>(string actionName, string serial, Dictionary<string, HttpContent> @params = null)
        {
            try
            {
                if (@params == null)
                    @params = new Dictionary<string, HttpContent>();

                string url = $"{endpoint}/connect/{actionName}";

                HttpResponseMessage response = null;

                using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StringContent(GetAuth(serial)), "auth");

                    foreach (var param in @params)
                    {
                        if (param.Value is ByteArrayContent)
                            content.Add(param.Value, param.Key, param.Key);
                        else
                            content.Add(param.Value, param.Key);
                    }

                    response = await client.PostAsync(url, content);
                }

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    return new Result<T>()
                    {
                        Exception = ex
                    };
                }

                string jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(jsonResult);

                return new Result<T>()
                {
                    ApiResult = result,
                };
            }
            catch (Exception ex)
            {
                return new Result<T>()
                {
                    Exception = ex
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public async Task<bool> SendFeedback(string serial, Feedback feedback, byte[] bytes)
        {
            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            var json = JsonConvert.SerializeObject(feedback, Formatting.Indented);

            var result = await MakePostApiRequest<bool>("feedback", serial, new Dictionary<string, HttpContent>
            {
                { "json", new StringContent(json, Encoding.UTF8,  "application/json") },
                { "feedback.zip", byteContent },
            });

            result.ThrowIfError();

            return result.ApiResult;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<i360WeatherForecast> GetWeatherForecast(string serial, double lat, double lon)
        {
            var result = await MakeApiRequest<i360WeatherForecast>("weather", serial, new Dictionary<string, string>()
            {
                {"lat", lat.ToString(CultureInfo.InvariantCulture) },
                {"lon", lon.ToString(CultureInfo.InvariantCulture) }
            });
            result.ThrowIfError();

            return result.ApiResult;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<i360DeviceStatus> GetDeviceStatus(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                throw new ArgumentNullException("Serial is null or empty");


            var result = await MakeApiRequest<i360DeviceStatus>("device-info", serial);

            try
            {
                result.ThrowIfError();
            }
            catch (HttpRequestException)
            {
                ///Устройство не найдено 
                if (result?.HttpResponse?.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Device not found on iridium360.ru");
                    return new i360DeviceStatus();
                }

                ///Ошибка на сервере
                if (result?.HttpResponse?.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Console.WriteLine($"Server error `{result?.HttpResponse?.StatusCode}`");
                    Debugger.Break();
                    throw;
                }

                throw;
            }

            return result.ApiResult;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (shouldDisposeClient)
                client?.Dispose();
        }

    }
}
