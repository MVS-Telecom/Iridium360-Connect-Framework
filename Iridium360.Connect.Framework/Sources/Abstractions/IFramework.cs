﻿using System;
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


    public class MessageReceivedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public byte[] Payload { get; set; }
        public bool Handled { get; set; } = false;
    }

    public class MessageStatusUpdatedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public MessageStatus Status { get; set; }
        public bool Handled { get; set; } = false;
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
        bool IsOn { get; }
        Task<bool> TurnOn(bool force = false);
    }


    public interface IFramework : IDisposable
    {
        event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults;
        event EventHandler<EventArgs> SearchTimeout;
        event EventHandler<MessageStatusUpdatedEventArgs> _MessageStatusUpdated;
        event EventHandler<MessageReceivedEventArgs> _MessageReceived;

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


        void StartDeviceSearch();
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