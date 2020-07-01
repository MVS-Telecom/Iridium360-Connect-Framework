using Iridium360.Connect.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Exceptions
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class RockException : Exception
    {
        public RockException(string message, Exception innerException) : base(message, innerException) { }
        public RockException(string message) : base(message) { }
        public RockException() : base() { }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MessageSendingException : RockException
    {
        public MessageSendingException(string message, Exception innerException) : base(message, innerException) { }
        public MessageSendingException(string message) : base(message) { }
        public MessageSendingException() : base() { }
    }


    /// <summary>
    /// Параметр не доступен (не поддерживается устройством или фреймворк еще не понял о его существовании - нужно повторить через пару секунд)
    /// </summary>
    public class ParameterUnavailableException : RockException
    {
        public ParameterUnavailableException(Parameter parameter, Exception innerException = null) : base($"Parameter `{parameter}` unavailable (unsupported or not ready)", innerException) { }
    }

    /// <summary>
    /// Не удалось подключиться к устройству
    /// </summary>
    public class DeviceConnectionException : RockException
    {
        public DeviceConnectionException(string message = null, Exception innerException = null) : base(message, innerException) { }
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
    public class DeviceIsLockedException : RockException
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public class BluetoothTurnedOffException : RockException
    {

    }
}
