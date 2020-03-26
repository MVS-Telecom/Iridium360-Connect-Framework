using Newtonsoft.Json;
using Rock.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Iridium360
{
    public class Hardware
    {
        public string SerialNumber { get; set; }
        public string ActivationCode { get; set; }
        public string Imei { get; set; }
    }

    public class DeviceInfo
    {
        public int? DeviceType { get; set; }
        public string Imei { get; set; }
    }

    public class Prepaid
    {
        public DateTime? ActivationDate { get; set; }
        public string BillingBy360 { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? MonthlyBegin { get; set; }
        public DateTime? MonthlyNext { get; set; }
        public int? Balance { get; set; }
        public int? Units { get; set; }
        public int? Usages { get; set; }
        public string Rate { get; set; }
        public string Package { get; set; }
    }

    public class i360DeviceStatus
    {
        public DateTime? Time { get; set; }
        public Hardware Hardware { get; set; }
        public DeviceInfo Device { get; set; }
        public Prepaid Prepaid { get; set; }
    }



    /// <summary>
    /// HTTP REST клиент для iridium360.ru
    /// </summary>
    public class i360ApiClient : IDisposable
    {
        private readonly HttpClient client;
        private readonly string token;

        public i360ApiClient(string token, HttpClient client = null)
        {
            this.client = client ?? new HttpClient();
            this.token = token;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<i360DeviceStatus> GetDeviceStatus(string serial)
        {
            try
            {
                if (string.IsNullOrEmpty(serial))
                    throw new ArgumentNullException("Serial is null or empty");


                string auth = Md5.Get($"{serial}#{token}");
                string url = $"https://demo.iridium360.ru/connect/device-info?auth={auth}";


                var response = await client.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    ///Устройство не найдено или ошибка на сервере
                    if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        return new i360DeviceStatus();
                    }
                    else
                    {
                        throw;
                    }
                }


                string json = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<i360DeviceStatus>(json);
                return result;

            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
        }

    }
}
