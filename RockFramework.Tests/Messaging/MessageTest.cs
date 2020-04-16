namespace RockFramework.Tests.Messaging
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rock.Iridium360.Messaging;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void Pack__ChatMessageMOTest()
        {
            for (int i = 0; i < 100; i++)
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
                ChatMessageMO emo = ChatMessageMO.Create((string)new string('X', 0x10), nullable1, (string)new string('t', i), (string)new string('s', i));
                ChatMessageMO emo2 = (ChatMessageMO)Message.Unpack(emo.Pack());
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
                EmptyMO ymo2 = Message.Unpack(buffer) as EmptyMO;
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
            FreeTextMO tmo2 = (FreeTextMO)Message.Unpack(tmo.Pack());
            if (!tmo.Text.Equals(tmo2.Text))
            {
                throw new InvalidOperationException();
            }
            tmo = FreeTextMO.Create("Смартфон получил внешний вид");
            tmo2 = (FreeTextMO)Message.Unpack(tmo.Pack());
            if (!tmo.Text.Equals(tmo2.Text))
            {
                throw new InvalidOperationException();
            }
            for (int i = 1; i < 100; i++)
            {
                tmo = FreeTextMO.Create((string)new string('x', i));
                byte[] buffer = tmo.Pack();
                tmo2 = (FreeTextMO)Message.Unpack(buffer);
                if (!tmo.Text.Equals(tmo2.Text))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        [TestMethod]
        public void ParseFromDevice()
        {
            Message message = Message.Unpack(StringToByteArray("0104150501288260C779D9177AA920D3A71BABC0302A0E0005"));
        }

        public static byte[] StringToByteArray(string hex) =>
            Enumerable.ToArray<byte>(Enumerable.Select<int, byte>(from x in Enumerable.Range(0, hex.Length) select x, delegate (int x) {
                return Convert.ToByte(hex.Substring(x, 2), 0x10);
            }));

        //[Serializable, CompilerGenerated]
        //private sealed class <>c
        //{
        //    public static readonly MessageTest.<>c<>9 = new MessageTest.<>c();
        //public static Func<int, bool> <>9__0_0;

        //    internal bool <StringToByteArray>b__0_0(int x) =>
        //        ((x % 2) == 0);
    //}
}
}

