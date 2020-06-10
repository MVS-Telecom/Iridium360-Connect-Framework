using Iridium360.Models;
using Newtonsoft.Json;
using Rock.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360
{
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
                if (result.HttpResponse.StatusCode == HttpStatusCode.NotFound || result.HttpResponse.StatusCode == HttpStatusCode.InternalServerError)
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
