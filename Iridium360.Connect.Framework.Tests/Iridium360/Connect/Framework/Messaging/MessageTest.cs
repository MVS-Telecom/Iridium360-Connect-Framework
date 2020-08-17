using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Iridium360.Connect.Framework.Messaging.Legacy;
using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Models;
using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Util;
using ByskySubscriber = Bysky.Subscriber;
using Bysky.SDK;
using Iridium360.Connect.Framework.Messaging.Storage;

namespace Iridium360.Connect.Framework.Tests.Messaging
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void Location__()
        {
            try
            {
                var buffer = new InMemoryBuffer();
                string text = @"Shovel works with all types of exchanges. 
In this example, I've set up exchanges as direct exchanges which use routing keys. 
However, if you have fanout exchanges, those will work as well—but you won't indicate a routing key when setting up the shovel for the exchange.
To demonstarate how you can send messages, let's look at the shovels we created for the first exchange that we set up—compose-to-messages-rmq. 
This shovel connects to our Compose for RabbitMQ exchange compose-exchange with our messages-rmq exchange in our Messages for RabbitMQ instance. 
Ive also created two queues to send messages to: compose - queue in the Compose instance and messages - queue in the Messages for RabbitMQ instance. 
Since the shovel is sending messages to the two exchanges, the queues have been bound to the exchanges so that the messages are received and queued up.";

                var packets = ChatMessageMO.Create(Subscriber.Create("79999740562", SubscriberNetwork.Mobile), null, null, text, -27.126073, -109.278857, 816).Pack(78);
                var totalLength = packets.Select(x => x.Length - 8).Sum();

                var m2 = MessageMO.Unpack(packets[1], buffer) as ChatMessageMO;
                var m1 = MessageMO.Unpack(packets[0], buffer) as ChatMessageMO;
                var m3 = MessageMO.Unpack(packets[2], buffer) as ChatMessageMO;

                if (!m3.Complete)
                    throw new Exception();

                if (m3.Text != text)
                    throw new Exception();

            }
            catch (Exception e)
            {

            }
        }




        [TestMethod]
        public async Task Bysky()
        {
            try
            {
                var buffer = new InMemoryBuffer();
                var bytes = "1204043F0D01286A1E27669F08768272FA8125E0B0BFCC6A0E14610A3481EFF952B4B05CFC2F53E7D2E013AEA013560F103035C55933596F5A4ED89BD6EB046DCD1C00CA".ToByteArray();
                var m = MessageMT.Unpack(bytes, buffer) as ChatMessageMT;
            }
            catch (Exception e)
            {

            }
        }


        [TestMethod]
        public void PackLegacy()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                var buffer = new InMemoryBuffer();

                //var bytes = "118B26010037393939393734303536323A46524F4D373939393937343035363228382E362E32302D31303A33352855544329293A20CFF0E8E2E5F2".ToByteArray();
                var bytes1 = "119607010037393939373136383933384062792D736B792E6E65743AD2E5F1F220322031323A3237".ToByteArray();

                var m1 = Legacy_MessageMT.Unpack(bytes1, buffer);
            }
            catch (Exception e)
            {

            }
        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <returns></returns>
        //        [TestMethod]
        //        public async Task Unpack__ANYTest()
        //        {
        //            try
        //            {
        //                var buffer = "120104324501288AA2087699F76D8272FA8125E0B0BFCC6A0E14E153F13C596F5A4E5C1A07804B0A536E8902932E27396F3FF0C08E0116".ToByteArray();
        //                //var buffer = Convert.FromBase64String("EgEEMUUBKGoeJ2afCHaCcvqBJeCwv8xqDhQhV/E8kVUcN1kVaEp0KZnchC9K/VuCOGdMiAB9");
        //                var _message = MessageMO.Unpack(buffer);

        //                if (_message is CheckMessagesMO)
        //                {

        //                }

        //                if (_message == null)
        //                    Assert.Fail();
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <returns></returns>
        //        [TestMethod]
        //        public async Task Pack__WeatherMOTest()
        //        {
        //            try
        //            {
        //                string s1 = Encoding.UTF8.GetString("118310010037393939393734303536323A46524F4D37393939393734303536322832382E352E32302D30363A35302855544329293A2054657374".ToByteArray());
        //                string s2 = Encoding.UTF8.GetString("37393939393734303536323A46524F4D37393939393734303536322832382E352E32302D30363A35302855544329293A2054657374".ToByteArray());

        //                var message = WeatherMO.Create(12.12109375, -9.12109375);

        //                var buffer = message.Pack();
        //                string hex = buffer.ToHexString();

        //                var _message = MessageMO.Unpack(buffer) as WeatherMO;

        //                var __message = MessageMO.Unpack("0104173501288AA2087699F76D001000185EA872809B56258200D9".ToByteArray());

        //                if (_message == null)
        //                    Assert.Fail();
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }


        //        [TestMethod]
        //        public void ParseWeather()
        //        {
        //            try
        //            {
        //                var buffer = "1200055c8c4fddda039841d6ad3d801994dd5902a08100bab4044084a64b6b0034c8bab4044084b23b5b0044204097d60088d074690f80085997f600685076670f800606e8ce1e00159aeeac015021ebd21e0015caeeac019001015d5a03200379".ToByteArray();
        //                var weather = MessageMT.Unpack(buffer);
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }


        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        [TestMethod]
        //        public async Task Pack__WeatherMTTest()
        //        {
        //            var r = await new HttpClient().GetAsync("http://demo.iridium360.ru/connect/weather?auth=d9fc554e3ad74919bf274e11bdfe07c3&lat=55.67578125&lon=37.255859375&interval=6");
        //            var s = await r.Content.ReadAsStringAsync();



        //            try
        //            {
        //                var ffff = JsonConvert.DeserializeObject<i360WeatherForecast>(s);

        //                var fs = ffff.Forecasts.Select(x => new i360PointForecast()
        //                {
        //                    Lat = x.Lat,
        //                    Lon = x.Lon,
        //                    DayInfos = x.DayInfos,
        //                    Forecasts = x.Forecasts.Select(z => new i360Forecast()
        //                    {
        //                        Cloud = z.Cloud,
        //                        Date = z.Date,
        //                        Precipitation = z.Precipitation,
        //                        WindSpeed = z.WindSpeed,
        //                        WindDirection = z.WindDirection,
        //                        Temperature = z.Temperature,
        //                        SnowRisk = z.SnowRisk,
        //                        Pressure = z.Pressure

        //                    }).Take(16).ToList()

        //                }).FirstOrDefault();

        //                var message = WeatherMT.Create(fs);

        //                //var message = WeatherMT.Create(new i360PointForecast()
        //                //{
        //                //    Lat = 55.12345678,
        //                //    Lon = 37.12345678,
        //                //    TimeOffset = 3,
        //                //    DayInfos = new List<i360DayInfo>()
        //                //          {
        //                //              new i360DayInfo()
        //                //              {
        //                //                   DateDay = DateTime.Now,
        //                //                   Forecasts = new List<i360Forecast>()
        //                //                   {
        //                //                        new i360Forecast()
        //                //                        {
        //                //                             Cloud = 80,
        //                //                              HourOffset = 0,
        //                //                               Precipitation = 12,
        //                //                                Pressure = 740,
        //                //                                  SnowRisk = false,
        //                //                                   Temperature = 13,
        //                //                                    WindDirection = 124,
        //                //                                     WindSpeed = 5.2
        //                //                        },
        //                //                        new i360Forecast()
        //                //                        {
        //                //                             Cloud = 60,
        //                //                              HourOffset = 6,
        //                //                               Precipitation = 50,
        //                //                                Pressure = 741,
        //                //                                  SnowRisk = true,
        //                //                                   Temperature = 15,
        //                //                                    WindDirection = 189,
        //                //                                     WindSpeed = 2.9
        //                //                        }
        //                //                   }
        //                //              },
        //                //          }
        //                //});
        //                var buffer = message.Pack();
        //                string hex = buffer.ToHexString();


        //                string b = string.Join("", buffer.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

        //                //if (hex != "0005141D97D051088DB00D020D02230AA8E63190C90805A3")
        //                //Assert.Fail();

        //                var _message = MessageMT.Unpack(buffer) as WeatherMT;

        //                if (_message == null)
        //                    Assert.Fail();

        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }


        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        [TestMethod]
        //        public void Pack__ChatMessageMOTest2()
        //        {
        //            try
        //            {
        //                ChatMessageMO emo1 = ChatMessageMO.Create(new Subscriber("79999740562", SubscriberNetwork.Mobile)
        //                        , 123
        //                        , 56789
        //                        , "part 1"
        //                        , -33.425238, -71.700784
        //                        , null
        //                        );

        //                ChatMessageMO emo2 = ChatMessageMO.Create(null
        //                        , null
        //                        , null
        //                        , "part 2"
        //                        , null
        //                        , null
        //                        );

        //                ChatMessageMO emo3 = ChatMessageMO.Create(null
        //                        , null
        //                        , null
        //                        , "part 3"
        //                        , null
        //                        , null
        //                        );


        //                var b1 = emo1.Pack();
        //                var b2 = emo2.Pack();
        //                var b3 = emo3.Pack();

        //                var __emo1 = (ChatMessageMO)MessageMO.Unpack(b1);
        //                var __emo2 = (ChatMessageMO)MessageMO.Unpack(b2);
        //                var __emo3 = (ChatMessageMO)MessageMO.Unpack(b3);
        //            }
        //            catch (Exception e)
        //            {

        //            }

        //        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Pack__ChatMessageMTTest2()
        {
            ChatMessageMT emt = ChatMessageMT.Create(new Subscriber("hello@world", SubscriberNetwork.Email)
                    , 123
                    , null
                    , "The markdown document and any attachments, such as images, will then be sent to your email address"
                    , null
                    );


            var packets = emt.Pack();
            var emt2 = __Unpack(packets) as ChatMessageMT;
        }



        IMessageMT __Unpack(List<byte[]> packets)
        {
            var buffer = new InMemoryBuffer();
            IMessageMT message = null;

            foreach (var p in packets)
                message = MessageMT.Unpack(p, buffer);

            return message;
        }



        //        /// <summary>
        //        ///  сообщения
        //        /// </summary>
        //        [TestMethod]
        //        public void Pack__ChatMessageMOTest()
        //        {
        //            for (ushort i = 0; i < 100; i++)
        //            {
        //                ushort? nullable;
        //                int? nullable4;
        //                ushort? nullable1;
        //                int? nullable5;
        //                int? nullable6;
        //                if ((i % 2) != 0)
        //                {
        //                    nullable1 = new ushort?((ushort)i);
        //                }
        //                else
        //                {
        //                    nullable = null;
        //                    nullable1 = nullable;
        //                }
        //                ChatMessageMO emo = ChatMessageMO.Create(new Subscriber("hello@world", SubscriberNetwork.Email)
        //                    , (ushort)(i * i)
        //                    , i
        //                    , (string)new string('t', i)
        //                    );
        //                var emo2 = (ChatMessageMO)MessageMO.Unpack(emo.Pack());
        //                if (emo.Subscriber != emo2.Subscriber)
        //                {
        //                    throw new InvalidOperationException("ChatId");
        //                }
        //                nullable = emo.Conversation;
        //                if (nullable.HasValue)
        //                {
        //                    nullable5 = new int?(nullable.GetValueOrDefault());
        //                }
        //                else
        //                {
        //                    nullable4 = null;
        //                    nullable5 = nullable4;
        //                }
        //                int? nullable2 = nullable5;
        //                nullable = emo2.Conversation;
        //                if (nullable.HasValue)
        //                {
        //                    nullable6 = new int?(nullable.GetValueOrDefault());
        //                }
        //                else
        //                {
        //                    nullable4 = null;
        //                    nullable6 = nullable4;
        //                }
        //                int? nullable3 = nullable6;
        //                if (!((nullable2.GetValueOrDefault() == nullable3.GetValueOrDefault()) & (nullable2.HasValue == nullable3.HasValue)))
        //                {
        //                    throw new InvalidOperationException("Conversation");
        //                }
        //                if (emo.Text != (emo2.Text ?? ""))
        //                {
        //                    throw new InvalidOperationException("Text");
        //                }
        //                if (emo.Subject != (emo2.Subject ?? ""))
        //                {
        //                    throw new InvalidOperationException("Subject");
        //                }
        //            }
        //        }

        //        [TestMethod]
        //        public void Pack__EmptyMO()
        //        {
        //            for (int i = 0; i < 0x3e8; i++)
        //            {
        //                string greeting = (string)new string('x', i);
        //                EmptyMO ymo = EmptyMO.Create(greeting);
        //                byte[] buffer = ymo.Pack();
        //                EmptyMO ymo2 = MessageMO.Unpack(buffer) as EmptyMO;
        //                if (ymo2.Length != ymo.Length)
        //                {
        //                    throw new InvalidOperationException("lenght");
        //                }
        //                if (ymo2.Greeting != greeting)
        //                {
        //                    throw new InvalidOperationException("gretings");
        //                }
        //            }
        //        }


        //        [TestMethod]
        //        public void Pack_VK()
        //        {
        //            var str = @"the new version of a good one for you can be fun and you should get a free game for a bit but it’s";


        //            var message = FreeTextMO.Create(str);


        //            var buffer = message.Pack();

        //            var _message = MessageMO.Unpack(buffer) as FreeTextMO;

        //            if (message.Text != _message.Text)
        //                Assert.Fail();
        //        }


        //        [TestMethod]
        //        public void Pack__FreeTextMO()
        //        {
        //            FreeTextMO tmo = FreeTextMO.Create("\nApple представила iPhone SE — первый смартфон компании в 2020 году. Аппарат стал преемником одноименного девайса, выпущенного в 2016 году. Об этом \x00abЛенте.ру\x00bb сообщил представитель компании.\nСмартфон получил внешний вид, схожий с дизайном iPhone 8: 4,7-дюймовый Retina IPS-экран, кнопка Home с дактилоскопическим сенсором Touch ID, крупные горизонтальные рамки на передней панели. На задней панели девайса расположена одинарная камера разрешением 12 мегапикселей. SE имеет чип Apple A13, как у iPhone 11 и 64 гигабайт встроенной памяти в минимальной комплектации.\n\x00abДешевый\x00bb iPhone имеет стеклянный корпус с рамкой из металла, который защищен от воды и пыли. Доступны белый, черный и красный цвета корпуса. У белой версии смартфона фронтальная панель все равно черная. Устройство базируется на актуальной iOS 13, имеет NFC с поддержкой бесконтактной оплаты Apple Pay, несъемный аккумулятор, емкость которого не раскрывается.\nСтоимость базовой версии iPhone SE составит 40 тысяч рублей — это самый дешевый актуальный смартфон компании. Продажи в России начнутся 24 апреля.\n");
        //            FreeTextMO tmo2 = (FreeTextMO)MessageMO.Unpack(tmo.Pack());
        //            if (!tmo.Text.Equals(tmo2.Text))
        //            {
        //                throw new InvalidOperationException();
        //            }
        //            tmo = FreeTextMO.Create("Смартфон получил внешний вид");
        //            tmo2 = (FreeTextMO)MessageMO.Unpack(tmo.Pack());
        //            if (!tmo.Text.Equals(tmo2.Text))
        //            {
        //                throw new InvalidOperationException();
        //            }
        //            for (int i = 1; i < 100; i++)
        //            {
        //                tmo = FreeTextMO.Create((string)new string('x', i));
        //                byte[] buffer = tmo.Pack();
        //                tmo2 = (FreeTextMO)MessageMO.Unpack(buffer);
        //                if (!tmo.Text.Equals(tmo2.Text))
        //                {
        //                    throw new InvalidOperationException();
        //                }
        //            }
        //        }


        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        [TestMethod]
        //        public void ParseFromDevice()
        //        {
        //            var bug = MessageMO.Unpack(StringToByteArray("0104150501288260C779D9177AA920D3A71BABC0302A0E0005")) as ChatMessageMO;

        //            Debug.WriteLine($"bug => `{bug.Text}`");

        //            var nobug = MessageMO.Unpack(StringToByteArray("0104180501288260C779D9177AA920D3A71BABC0D036C9A238090052")) as ChatMessageMO;
        //            Debug.WriteLine($"nobug => `{nobug.Text}`");


        //            var tests = new string[] {
        //                @"
        //01 04 1C 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
        //A7 1B AB C0 20 5B D6 42 38 43 53 41 4D 02 00 95",
        //                @"
        //01 04 1E 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
        //A7 1B AB C0 D0 36 89 66 59 0B E1 0C 4D 05 35 09
        //00 7C",
        //                @"
        //01 04 16 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
        //A7 1B AB C0 F0 42 95 93 00 F8",
        //                @"01 04 16 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
        //A7 1B AB C0 F0 5A 95 03 00 80",
        //            };


        //            foreach (var test in tests)
        //            {
        //                var message = MessageMO.Unpack(StringToByteArray(test));

        //                if (message is ChatMessageMO chatMessage)
        //                {
        //                    Debug.WriteLine($"Messge is `{message.GetType()}`");

        //                    Debug.WriteLine($"\t, chat:`{chatMessage.Subscriber}`");
        //                    Debug.WriteLine($"\t, id:`{chatMessage.Id}`");
        //                    Debug.WriteLine($"\t, converssation:`{chatMessage.Conversation}`");
        //                    Debug.WriteLine($"\t, subject:`{chatMessage.Subject}`");
        //                    Debug.WriteLine($"\t, text:`{chatMessage.Text}`");
        //                }

        //            }

        //        }




        /// <summary>
        ///     
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hex)
        {
            while (hex.Contains(" ") || hex.Contains("\r") || hex.Contains("\n"))
            {
                hex = hex.Replace(" ", "");
                hex = hex.Replace("\r", "");
                hex = hex.Replace("\n", "");
            }

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        private static byte[] Merge(List<byte[]> arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }


    }
}