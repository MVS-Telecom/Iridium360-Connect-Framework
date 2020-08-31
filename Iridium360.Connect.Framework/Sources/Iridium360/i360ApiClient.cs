using Iridium360.Connect.Framework.Models;
using Iridium360.Connect.Framework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

    public class SavedDeviceInfo
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
        public SavedDeviceInfo ConnectedDevice { get; set; }

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
        private readonly HttpClient client;
        private readonly string token;
        private readonly string serial;
        private readonly string auth;
        private bool shouldDisposeClient = false;

        public i360ApiClient(string token, string serial, HttpClient client = null)
        {
            if (client == null)
            {
                shouldDisposeClient = true;
                client = new HttpClient();
            }

            this.client = client;
            this.token = token;
            this.serial = serial;
            this.auth = Md5.Get($"{serial}#{token}");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="params"></param>
        /// <returns></returns>
        private async Task<Result<T>> MakeApiRequest<T>(string actionName, Dictionary<string, string> @params = null)
        {
            try
            {
                if (@params == null)
                    @params = new Dictionary<string, string>();

                @params.Add("auth", auth);


                string _params = string.Join("&", @params.Select(x => $"{x.Key}={x.Value}"));
                string url = $"https://demo.iridium360.ru/connect/{actionName}?{_params}";


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

        private async Task<Result<T>> MakePostApiRequest<T>(string actionName, Dictionary<string, HttpContent> @params = null)
        {
            try
            {
                if (@params == null)
                    @params = new Dictionary<string, HttpContent>();

                //string url = $"http://192.168.88.36:45455/connect/{actionName}";
                string url = $"https://demo.iridium360.ru/connect/{actionName}";

                HttpResponseMessage response = null;

                using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StringContent(auth), "auth");

                    foreach (var param in @params)
                        content.Add(param.Value, param.Key);

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
        public async Task<bool> SendFeedback(Feedback feedback, MemoryStream zip)
        {
            var json = JsonConvert.SerializeObject(feedback, Formatting.Indented);

            var result = await MakePostApiRequest<bool>("feedback", new Dictionary<string, HttpContent>
            {
                { "json", new StringContent(json, Encoding.UTF8,  "application/json") },
                { "feedback.zip", new ByteArrayContent(zip.ToArray(), 0, (int)zip.Length) },
            });

            result.ThrowIfError();

            return result.ApiResult;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<i360WeatherForecast> GetWeatherForecast(double lat, double lon)
        {
            var result = await MakeApiRequest<i360WeatherForecast>("weather", new Dictionary<string, string>()
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
        public async Task<i360DeviceStatus> GetDeviceStatus()
        {
            if (string.IsNullOrEmpty(serial))
                throw new ArgumentNullException("Serial is null or empty");


            var result = await MakeApiRequest<i360DeviceStatus>("device-info");

            try
            {
                result.ThrowIfError();
            }
            catch (HttpRequestException)
            {
                ///Устройство не найдено или ошибка на сервере
                if (result?.HttpResponse?.StatusCode == HttpStatusCode.NotFound || result?.HttpResponse?.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return new i360DeviceStatus();
                }
                else
                {
                    throw;
                }
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
