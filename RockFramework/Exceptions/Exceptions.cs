using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Exceptions
{
    public class RockException : Exception
    {
        public RockException(string message, Exception innerException) : base(message, innerException) { }
        public RockException(string message) : base(message) { }
        public RockException() : base() { }
    }


    public class MessageSendingException : RockException
    {
        public MessageSendingException(string message, Exception innerException) : base(message, innerException) { }
        public MessageSendingException(string message) : base(message) { }
        public MessageSendingException() : base() { }
    }


    /// <summary>
    /// Не удалось подключиться к устройству
    /// </summary>
    public class DeviceConnectionException : RockException
    {
        public DeviceConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Устройство не подключено
    /// </summary>
    public class NotConnectedException : RockException
    {

    }

    /// <summary>
    /// Не удалось разблокировать устройство - некорректный PIN
    /// </summary>
    public class IncorrectPinException : RockException
    {
    }


    /// <summary>
    /// Устройство не разблокировано
    /// </summary>
    public class DeviceIsLockedException: RockException
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public class BluetoothTurnedOffException : RockException
    {

    }
}
