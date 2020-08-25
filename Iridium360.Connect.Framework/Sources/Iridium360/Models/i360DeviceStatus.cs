using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Models
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


    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum AppFeatures
    {
        None = 0,
        Developer = 1
    }


    public class i360DeviceStatus
    {
        public AppFeatures? AppFeatures { get; set; }
        public DateTime? Time { get; set; }
        public Hardware Hardware { get; set; }
        public DeviceInfo Device { get; set; }
        public Prepaid Prepaid { get; set; }
    }

}
