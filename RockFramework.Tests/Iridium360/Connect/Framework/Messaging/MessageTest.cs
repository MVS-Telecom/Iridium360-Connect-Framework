using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    [TestClass]
    public class MessageTest
    {


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Pack__WeatherMOTest()
        {
            var message = WeatherMO.Create();
            var buffer = message.Pack();

            var _message = MessageMO.Unpack(buffer) as WeatherMO;

            if (_message == null)
                Assert.Fail();
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
                ChatMessageMO emo = ChatMessageMO.Create((string)new string('X', 0x10)
                    , (ushort)(i * i)
                    , i
                    , (string)new string('t', i)
                    , (string)new string('s', i)
                    );
                var emo2 = (ChatMessageMO)MessageMO.Unpack(emo.Pack());
                if (emo.ChatId != (emo2.ChatId ?? ""))
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


            foreach(var test in tests)
            {
                var message = MessageMO.Unpack(StringToByteArray(test));

                if(message is ChatMessageMO chatMessage)
                {
                    Debug.WriteLine($"Messge is `{message.GetType()}`");

                    Debug.WriteLine($"\t, chat:`{chatMessage.ChatId}`");
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