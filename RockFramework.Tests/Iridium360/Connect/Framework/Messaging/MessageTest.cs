using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rock.Helpers;
using Iridium360.Models;
using Rock;
using System.Text;
using System.IO;
using Iridium360.Connect.Framework.Messaging.Legacy;

namespace Iridium360.Connect.Framework.Messaging
{
    


    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void PackLegacy()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                var buffer = new InMemoryBuffer();

                //var bytes = "118B26010037393939393734303536323A46524F4D373939393937343035363228382E362E32302D31303A33352855544329293A20CFF0E8E2E5F2".ToByteArray();
                var bytes1 = "118B300201206C6F6E6720492068617665206120676F6F64206964656120666F7220757320746F2067657420746F676574686572207769746820796F7520616E64207468652066616D696C792061726520646F696E672077656C6C20616E64207468617420796F752068617665206120776F6E64657266756C204368726973746D6173".ToByteArray();
                var bytes2 = "118B30020037393939393734303536323A46524F4D373939393937343035363228382E362E32302D31323A31372855544329293A204B6964732061726520676F696E6720746F206861766520616E79206D6F726520706963747572657320686F77206C6F6E6720492068617665206120676F6F64206964656120666F7220757320746F2067657420746F676574686572207769746820796F7520616E64207468652066616D696C792061726520646F696E672077656C6C20616E64207468617420796F752068617665206120776F6E64657266756C204368726973746D6173204B6964732061726520676F696E6720746F206861766520616E79206D6F726520706963747572657320686F77".ToByteArray();

                var m1 = Legacy_MessageMT.Unpack(buffer, bytes1);
                var m2 = Legacy_MessageMT.Unpack(buffer, bytes2);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Unpack__ANYTest()
        {
            try
            {
                var buffer = "1201050537AD4A060152".ToByteArray();
                var _message = MessageMO.Unpack(buffer);

                if (_message == null)
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Pack__WeatherMOTest()
        {
            try
            {
                string s1 = Encoding.UTF8.GetString("118310010037393939393734303536323A46524F4D37393939393734303536322832382E352E32302D30363A35302855544329293A2054657374".ToByteArray());
                string s2 = Encoding.UTF8.GetString("37393939393734303536323A46524F4D37393939393734303536322832382E352E32302D30363A35302855544329293A2054657374".ToByteArray());

                var message = WeatherMO.Create(12.12109375, -9.12109375);

                var buffer = message.Pack();
                string hex = buffer.ToHexString();

                var _message = MessageMO.Unpack(buffer) as WeatherMO;

                var __message = MessageMO.Unpack("0104173501288AA2087699F76D001000185EA872809B56258200D9".ToByteArray());

                if (_message == null)
                    Assert.Fail();
            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public async Task Pack__WeatherMTTest()
        {
            var r = await new HttpClient().GetAsync("http://demo.iridium360.ru/connect/weather?auth=d9fc554e3ad74919bf274e11bdfe07c3&lat=55.67578125&lon=37.255859375&interval=6");
            var s = await r.Content.ReadAsStringAsync();



            try
            {
                var ffff = JsonConvert.DeserializeObject<i360WeatherForecast>(s);

                var fs = ffff.Forecasts.Select(x => new i360PointForecast()
                {
                    Lat = x.Lat,
                    Lon = x.Lon,
                    DayInfos = x.DayInfos,
                    Forecasts = x.Forecasts.Select(z => new i360Forecast()
                    {
                        Cloud = z.Cloud,
                        Date = z.Date,
                        Precipitation = z.Precipitation,
                        WindSpeed = z.WindSpeed,
                        WindDirection = z.WindDirection,
                        Temperature = z.Temperature,
                        SnowRisk = z.SnowRisk,
                        Pressure = z.Pressure

                    }).Take(16).ToList()

                }).FirstOrDefault();

                var message = WeatherMT.Create(fs);

                //var message = WeatherMT.Create(new i360PointForecast()
                //{
                //    Lat = 55.12345678,
                //    Lon = 37.12345678,
                //    TimeOffset = 3,
                //    DayInfos = new List<i360DayInfo>()
                //          {
                //              new i360DayInfo()
                //              {
                //                   DateDay = DateTime.Now,
                //                   Forecasts = new List<i360Forecast>()
                //                   {
                //                        new i360Forecast()
                //                        {
                //                             Cloud = 80,
                //                              HourOffset = 0,
                //                               Precipitation = 12,
                //                                Pressure = 740,
                //                                  SnowRisk = false,
                //                                   Temperature = 13,
                //                                    WindDirection = 124,
                //                                     WindSpeed = 5.2
                //                        },
                //                        new i360Forecast()
                //                        {
                //                             Cloud = 60,
                //                              HourOffset = 6,
                //                               Precipitation = 50,
                //                                Pressure = 741,
                //                                  SnowRisk = true,
                //                                   Temperature = 15,
                //                                    WindDirection = 189,
                //                                     WindSpeed = 2.9
                //                        }
                //                   }
                //              },
                //          }
                //});
                var buffer = message.Pack();
                string hex = buffer.ToHexString();


                string b = string.Join("", buffer.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

                //if (hex != "0005141D97D051088DB00D020D02230AA8E63190C90805A3")
                //Assert.Fail();

                var _message = MessageMT.Unpack(buffer) as WeatherMT;

                if (_message == null)
                    Assert.Fail();

            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Pack__ChatMessageMOTest2()
        {
            ChatMessageMO emo = ChatMessageMO.Create(new Subscriber(string.Empty, SubscriberNetwork.Portal)
                    , 123
                    , 56789
                    , null
                    , "The markdown document and any attachments, such as images, will then be sent to your email address"
                    , 32.8192159, -56.1295223
                    );

            var b = emo.Pack();
            var emo2 = (ChatMessageMO)MessageMO.Unpack(b);


            // var emo3 = (ChatMessageMO)MessageMO.Unpack("0104AD3542ABC631ADF0F41DA740180010A88437F124BF7D77669399CE1356CD93D344E1C49D13D94F64774E132A61D53C91FDE4A5CFE4345138A1825B88E324376338E1DDBC4D643F916939A1825B88E38455F324A713964E723AC95B3559DD1BCF132AB885384E4E35394D144EDC39C95B35494D3FA1825B9093FCF6DD994D663A4F58354F5A5A56132AB80539C96FDF9DD984D6F3840A6E218E135C5593D69593D8CCD53BC9E9D4F41380E143828FC3".ToByteArray());
            var emo3 = (ChatMessageMO)MessageMO.Unpack("0104183501286A1E276E5E276A002800103811EA3E1260F890E0232D".ToByteArray());


        }


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

            var b = emt.Pack();
            var h = b.ToHexString();
            var emt2 = MessageMT.Unpack(b) as ChatMessageMT;
        }


        /// <summary>
        ///  сообщения
        /// </summary>
        [TestMethod]
        public void Pack__ChatMessageMOTest()
        {
            for (ushort i = 0; i < 100; i++)
            {
                ushort? nullable;
                int? nullable4;
                ushort? nullable1;
                int? nullable5;
                int? nullable6;
                if ((i % 2) != 0)
                {
                    nullable1 = new ushort?((ushort)i);
                }
                else
                {
                    nullable = null;
                    nullable1 = nullable;
                }
                ChatMessageMO emo = ChatMessageMO.Create(new Subscriber("hello@world", SubscriberNetwork.Email)
                    , (ushort)(i * i)
                    , i
                    , (string)new string('t', i)
                    , (string)new string('s', i)
                    );
                var emo2 = (ChatMessageMO)MessageMO.Unpack(emo.Pack());
                if (emo.Subscriber != emo2.Subscriber)
                {
                    throw new InvalidOperationException("ChatId");
                }
                nullable = emo.Conversation;
                if (nullable.HasValue)
                {
                    nullable5 = new int?(nullable.GetValueOrDefault());
                }
                else
                {
                    nullable4 = null;
                    nullable5 = nullable4;
                }
                int? nullable2 = nullable5;
                nullable = emo2.Conversation;
                if (nullable.HasValue)
                {
                    nullable6 = new int?(nullable.GetValueOrDefault());
                }
                else
                {
                    nullable4 = null;
                    nullable6 = nullable4;
                }
                int? nullable3 = nullable6;
                if (!((nullable2.GetValueOrDefault() == nullable3.GetValueOrDefault()) & (nullable2.HasValue == nullable3.HasValue)))
                {
                    throw new InvalidOperationException("Conversation");
                }
                if (emo.Text != (emo2.Text ?? ""))
                {
                    throw new InvalidOperationException("Text");
                }
                if (emo.Subject != (emo2.Subject ?? ""))
                {
                    throw new InvalidOperationException("Subject");
                }
            }
        }

        [TestMethod]
        public void Pack__EmptyMO()
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                string greeting = (string)new string('x', i);
                EmptyMO ymo = EmptyMO.Create(greeting);
                byte[] buffer = ymo.Pack();
                EmptyMO ymo2 = MessageMO.Unpack(buffer) as EmptyMO;
                if (ymo2.Length != ymo.Length)
                {
                    throw new InvalidOperationException("lenght");
                }
                if (ymo2.Greeting != greeting)
                {
                    throw new InvalidOperationException("gretings");
                }
            }
        }


        [TestMethod]
        public void Pack_VK()
        {
            var str = @"the new version of a good one for you can be fun and you should get a free game for a bit but it’s";


            var message = FreeTextMO.Create(str);


            var buffer = message.Pack();

            var _message = MessageMO.Unpack(buffer) as FreeTextMO;

            if (message.Text != _message.Text)
                Assert.Fail();
        }


        [TestMethod]
        public void Pack__FreeTextMO()
        {
            FreeTextMO tmo = FreeTextMO.Create("\nApple представила iPhone SE — первый смартфон компании в 2020 году. Аппарат стал преемником одноименного девайса, выпущенного в 2016 году. Об этом \x00abЛенте.ру\x00bb сообщил представитель компании.\nСмартфон получил внешний вид, схожий с дизайном iPhone 8: 4,7-дюймовый Retina IPS-экран, кнопка Home с дактилоскопическим сенсором Touch ID, крупные горизонтальные рамки на передней панели. На задней панели девайса расположена одинарная камера разрешением 12 мегапикселей. SE имеет чип Apple A13, как у iPhone 11 и 64 гигабайт встроенной памяти в минимальной комплектации.\n\x00abДешевый\x00bb iPhone имеет стеклянный корпус с рамкой из металла, который защищен от воды и пыли. Доступны белый, черный и красный цвета корпуса. У белой версии смартфона фронтальная панель все равно черная. Устройство базируется на актуальной iOS 13, имеет NFC с поддержкой бесконтактной оплаты Apple Pay, несъемный аккумулятор, емкость которого не раскрывается.\nСтоимость базовой версии iPhone SE составит 40 тысяч рублей — это самый дешевый актуальный смартфон компании. Продажи в России начнутся 24 апреля.\n");
            FreeTextMO tmo2 = (FreeTextMO)MessageMO.Unpack(tmo.Pack());
            if (!tmo.Text.Equals(tmo2.Text))
            {
                throw new InvalidOperationException();
            }
            tmo = FreeTextMO.Create("Смартфон получил внешний вид");
            tmo2 = (FreeTextMO)MessageMO.Unpack(tmo.Pack());
            if (!tmo.Text.Equals(tmo2.Text))
            {
                throw new InvalidOperationException();
            }
            for (int i = 1; i < 100; i++)
            {
                tmo = FreeTextMO.Create((string)new string('x', i));
                byte[] buffer = tmo.Pack();
                tmo2 = (FreeTextMO)MessageMO.Unpack(buffer);
                if (!tmo.Text.Equals(tmo2.Text))
                {
                    throw new InvalidOperationException();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ParseFromDevice()
        {
            var bug = MessageMO.Unpack(StringToByteArray("0104150501288260C779D9177AA920D3A71BABC0302A0E0005")) as ChatMessageMO;

            Debug.WriteLine($"bug => `{bug.Text}`");

            var nobug = MessageMO.Unpack(StringToByteArray("0104180501288260C779D9177AA920D3A71BABC0D036C9A238090052")) as ChatMessageMO;
            Debug.WriteLine($"nobug => `{nobug.Text}`");


            var tests = new string[] {
                @"
01 04 1C 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
A7 1B AB C0 20 5B D6 42 38 43 53 41 4D 02 00 95",
                @"
01 04 1E 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
A7 1B AB C0 D0 36 89 66 59 0B E1 0C 4D 05 35 09
00 7C",
                @"
01 04 16 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
A7 1B AB C0 F0 42 95 93 00 F8",
                @"01 04 16 05 01 28 82 60 C7 79 D9 17 7A A9 20 D3
A7 1B AB C0 F0 5A 95 03 00 80",
            };


            foreach (var test in tests)
            {
                var message = MessageMO.Unpack(StringToByteArray(test));

                if (message is ChatMessageMO chatMessage)
                {
                    Debug.WriteLine($"Messge is `{message.GetType()}`");

                    Debug.WriteLine($"\t, chat:`{chatMessage.Subscriber}`");
                    Debug.WriteLine($"\t, id:`{chatMessage.Id}`");
                    Debug.WriteLine($"\t, converssation:`{chatMessage.Conversation}`");
                    Debug.WriteLine($"\t, subject:`{chatMessage.Subject}`");
                    Debug.WriteLine($"\t, text:`{chatMessage.Text}`");
                }

            }

        }




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



    }
}