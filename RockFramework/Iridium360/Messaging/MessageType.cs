namespace Rock.Iridium360.Messaging
{
    using System;

    public enum MessageType : byte
    {
        Empty = 0,
        Ping = 1,
        Payload = 2,
        FreeText = 3,
        ChatMessageMO = 4,
        WeatherMO = 5,
    }
}

