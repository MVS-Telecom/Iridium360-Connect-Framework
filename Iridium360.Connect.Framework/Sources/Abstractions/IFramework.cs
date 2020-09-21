using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework
{
    public enum MessageStatus : int
    {
        Pending,
        ReceivedByDevice,
        QueuedForTransmission,
        Transmitting,
        Transmitted,
        Received,
        ErrorToolong,
        ErrorNoCredit,
        ErrorCapability,
        Error
    }

    public enum LockState : int
    {
        Unknown = 0,
        Unlocked = 1,
        Locked = 2,
    }


    public class PacketReceivedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public byte[] Payload { get; set; }
    }

    public class PacketStatusUpdatedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public MessageStatus Status { get; set; }
        public string Message { get; set; }
    }


    public class DeviceSearchResultsEventArgs : EventArgs
    {
        public List<IFoundDevice> Devices { get; set; }
    }


    public class MessageProgressEventArgs : EventArgs
    {
        public short MessageId { get; set; }
    }



    public interface IStorage
    {
        void PutString(string key, string value);
        string GetString(string key, string defaultValue);
        short GetShort(string key, short defaultValue);
        void PutShort(string key, short value);
        void Remove(string key);
    }


    public interface IBluetoothHelper
    {
        event EventHandler<ScanResultsEventArgs> ScanResults;
        bool IsOn { get; }
        Task<bool> TurnOn(bool force = false);
        void StartLeScan();
        void StopLeScan();
    }


    public interface IFramework : IDisposable
    {
        event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults;
        event EventHandler<EventArgs> SearchTimeout;
        event EventHandler<PacketStatusUpdatedEventArgs> PacketStatusUpdated;
        event EventHandler<PacketReceivedEventArgs> PacketReceived;

        /// <summary>
        /// 
        /// </summary>
        IDevice ConnectedDevice { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flags"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        Task<bool> Connect(
            Guid id,
            bool force = true,
            bool throwOnError = false,
            int attempts = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="flags"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        Task<bool> Connect(
            IBluetoothDevice device,
            bool force = true,
            bool throwOnError = false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        Task<bool> Reconnect(bool throwOnError = true);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ForgetDevice();


        /// <summary>
        /// Отправить текущие координаты
        /// </summary>
        Task SendManual();

        /// <summary>
        /// Отправить SOS
        /// </summary>
        Task RequestAlert();

        /// <summary>
        /// "БИИП"
        /// </summary>
        Task Beep();

        /// <summary>
        /// 
        /// </summary>
        Task GetReceivedMessages();


        Task StartDeviceSearch();
        void StopDeviceSearch();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="shortId"></param>
        /// <returns></returns>
        Task<ushort> SendData(byte[] data);
    }
}
