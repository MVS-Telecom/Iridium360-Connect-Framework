using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{

    public class BalanceMT : MessageMT
    {
        public override MessageType Type => MessageType.Balance;

        public DateTime Time { get; set; }
        public DateTime? MonthlyBegin { get; set; }
        public DateTime? MonthlyNext { get; set; }
        public int? Balance { get; set; }
        public int? Units { get; set; }
        public int? Usages { get; set; }

        private static DateTime START = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write((uint)(Time - START).TotalDays, 14);
            writer.Write((uint)Time.Hour, 5);
            writer.Write((uint)(Time.Minute / 15d), 2);

            if (MonthlyBegin == null && MonthlyNext == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);

                writer.Write((uint)(MonthlyBegin.Value - START).TotalDays, 14);
                writer.Write((uint)(MonthlyNext.Value - START).TotalDays, 14);
            }

            writer.Write((int?)Balance, 15);
            writer.Write((int?)Units, 15);
            writer.Write((int?)Usages, 15);
        }

        protected override void unpack(BinaryBitReader reader)
        {
            Time = START.AddDays(reader.ReadUInt(14));
            Time = Time.AddHours(reader.ReadUInt(5));
            Time = Time.AddMinutes(reader.ReadUInt(2) * 15);

            if (reader.ReadBoolean())
            {
                MonthlyBegin = START.AddDays(reader.ReadUInt(14));
                MonthlyNext = START.AddDays(reader.ReadUInt(14));
            }

            Balance = (int?)reader.ReadIntNullable(15);
            Units = (int?)reader.ReadIntNullable(15);
            Usages = (int?)reader.ReadIntNullable(15);
        }


        public static BalanceMT Create(ProtocolVersion version, DateTime time, DateTime? monthlyBegin, DateTime? monthlyNext, int? balance, int? units, int? usages)
        {
            BalanceMT response = Create<BalanceMT>(version);

            response.Time = time;
            response.MonthlyBegin = monthlyBegin;
            response.MonthlyNext = monthlyNext;
            response.Balance = balance;
            response.Units = units;
            response.Usages = usages;

            // --->
            return response;
        }
    }
}
