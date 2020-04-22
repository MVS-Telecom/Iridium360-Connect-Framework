using Rock.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Helpers;
using System.Text;
using System.Linq;
using Rock.Core;
using System;
namespace Rock.Tests
{
    [TestClass]
    public class CommandsTest
    {
        [TestMethod]
        public void ByteBuffer_AutoResize()
        {
            var buffer = new ByteBuffer();

            if (buffer.Bytes.Length != 0)
                Assert.Fail();

            buffer.WriteByte(200);
            buffer.WriteInt16(140);
            buffer.WriteInt32(7587383);
            buffer.WriteBytes(new byte[] { 73, 92, 6, 201, 88 });

            if (buffer.Bytes.Length != 1 + 2 + 4 + 5)
                Assert.Fail();
        }


        [TestMethod]
        public void PinCommand()
        {
            var command = new PinCommand("AQIDBAU=", 255, 1234, 1234);
            var package = command.GetPackets()[0] as byte[];
            string hex = package.ToHexString();
            if (hex != "000D070102030405FF04D204D263750000000000")
                Assert.Fail();
        }


        [TestMethod]
        public void BeepCommand()
        {
            var command = new ActionCommand("AQIDBAU=", 255, ActionRequestType.Beep);
            var package = command.GetPackets()[0] as byte[];
            string hex = package.ToHexString();
            if (hex != "000A080102030405FF0AAF050000000000000000")
                Assert.Fail();
        }


        [TestMethod]
        public void SendMessageCommand()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var command = new SendMessageCommand("AQIDBAU=", 255, 3, bytes);
            var package = command.GetPackets()[0] as byte[];
            string hex = package.ToHexString();
            if (hex != "0010010102030405FF000301020304050CC60000")
                Assert.Fail();
        }


        /// <summary>
        /// Каждый передаваемый по блютузу на устройство пакет не должен превышать 20 байтов
        /// </summary>
        [TestMethod]
        public void MessageChunks()
        {
            var bytes = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var command = new SendMessageCommand("AQIDBAU=", 255, 3, bytes);
            var packages = command.GetPackets().ToArray().Cast<byte[]>().ToList();
            if (packages.Any(x => x.Length > 20))
                Assert.Fail();
        }


        [TestMethod]
        public void ByteBufferCheck()
        {
            var _byte = 203;
            var _bytes = new byte[] { 109, 22, 34, 9, 57 };
            var _int16 = 1045;
            var _int32 = 57329;
            var buffer = ByteBuffer.Allocate(30);
            buffer.WriteByte((byte)_byte);
            buffer.WriteBytes(_bytes);
            buffer.WriteInt16((short)_int16);
            buffer.WriteInt32(_int32);
            var buffer2 = new ByteBuffer(buffer.Bytes);
            var _byte2 = buffer2.ReadByte();
            var _bytes2 = buffer2.ReadBytes(5);
            var _int16_2 = buffer2.ReadInt16();
            var _int32_2 = buffer2.ReadInt32();

            if (_byte != _byte2)
                Assert.Fail();
            if (!_bytes.SequenceEqual(_bytes2))
                Assert.Fail();
            if (_int16 != _int16_2)
                Assert.Fail();
            if (_int32 != _int32_2)
                Assert.Fail();
        }

        [TestMethod]
        public void IncomingCommandParse_Battery()
        {
            //var bytes = ArrayHelper.ToByteArray("000B0202F8BB2D640000008C9300");
            var bytes = ArrayHelper.ToByteArray("000A0871156899F91407784D0000000000000000");
            var command = DeserializedCommand.Parse(bytes);
            if (command.ActionRequestType != ActionRequestType.BatteryUpdate || command.Key != 20 || command.AppId != "cRVomfk=")
                Assert.Fail();
        }

        [TestMethod]
        public void ParseCommand()
        {
            var bytes = ArrayHelper.ToByteArray("782E72753A68656C6C6F20776F726C64CA410000");
            var command = DeserializedCommand.Parse(bytes);
        }

        [TestMethod]
        public void ParseCommand2()
        {
            var bytes = ArrayHelper.ToByteArray("000B01E8DFB81EB400000293BD00000000000000");
            var command = DeserializedCommand.Parse(bytes);
        }

        [TestMethod]
        public void ParseCommand3()
        {
            var bytes = ArrayHelper.ToByteArray("03328373B600000D00");
            var command = DeserializedStatus.Parse(bytes);

            if (command.AppId != "AzKDc7Y=" || command.Key != 0 || command.MessageId != 13)
                Assert.Fail();
        }

        [TestMethod]
        public void ParseCommand4()
        {
            var bytes = ArrayHelper.ToByteArray("B5ABFCA541FF000900");
            var command = DeserializedStatus.Parse(bytes);

            if (command.AppId != "AzKDc7Y=" || command.Key != 0 || command.MessageId != 13)
                Assert.Fail();
        }

        [TestMethod]
        public void FromDevice_DeleteMessageCommand()
        {
            var bytes = ArrayHelper.ToByteArray("000B03044FA7812A140000EF0600000000000000");
            var command = DeserializedCommand.Parse(bytes);
            if (command.CommandType != CommandType.DeleteMessage)
                Assert.Fail();
        }

        [TestMethod]
        public void BigEndianReadWrite()
        {
            var buffer1 = new ByteBuffer();
            buffer1.WriteInt16(3);

            var buffer = new ByteBuffer(buffer1.Bytes);
            short value = buffer.ReadInt16();

            if (value != 3)
                Assert.Fail();
        }

        [TestMethod]
        public void BigEndianRead()
        {
            var bytes = new byte[] { 3 };

            var buffer = new ByteBuffer(bytes);
            short value = buffer.ReadInt16();

            if (value != 3)
                Assert.Fail();
        }
    }
}