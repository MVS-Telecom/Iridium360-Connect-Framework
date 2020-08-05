using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Legacy
{
    public static class ByfferHelper
    {
        private const string BUFFER_DATABASE_NAME = "buffer.realm";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static Realm GetBufferInstance()
        {
            lock (BUFFER_DATABASE_NAME)
            {
                return Realm.GetInstance(GetBufferConfig());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static RealmConfiguration GetBufferConfig()
        {
            return new RealmConfiguration(BUFFER_DATABASE_NAME)
            {
                SchemaVersion = 5,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetBufferDbPath()
        {
            return GetBufferConfig().DatabasePath;
        }
    }




    /// <summary>
    /// 
    /// </summary>
    internal class Part : RealmObject
    {
        [PrimaryKey, Indexed]
        public string InnerId { get; set; }

        [Indexed]
        public int Id { get; set; }

        public int Index { get; set; }

        public int TotalParts { get; set; }

        public byte[] Content { get; set; }
    }



    internal abstract class RealmCopyDisplay<T> where T : RealmObject, new()
    {
        protected T source { get; private set; }


        public RealmCopyDisplay(T source)
        {
            this.source = Copy(source);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        private static TRealmObject Copy<TRealmObject>(TRealmObject source) where TRealmObject : RealmObject, new()
        {
            if (!source.IsManaged)
                return source;

            TRealmObject copy = new TRealmObject();

            try
            {
                var properties = source
                    .GetType()
                    .GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .ToList();

                properties.ForEach(property =>
                {
                    try
                    {
                        if (property.CanWrite)
                        {
                            var value = property.GetValue(source);
                            property.SetValue(copy, value, null);
                        }
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }
                });
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
            }

            return copy;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    internal class __Part : RealmCopyDisplay<Part>, IPart
    {
        public uint Id => (uint)source.Id;
        public uint Index => (uint)source.Index;
        public uint TotalParts => (uint)source.TotalParts;
        public byte[] Content => source.Content;


        public __Part(Part part) : base(part)
        {

        }

    }




    /// <summary>
    /// 
    /// </summary>
    internal class RealmPartsBuffer : IPartsBuffer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Clear(uint id)
        {
            using (var realm = ByfferHelper.GetBufferInstance())
            {
                var toRemove = realm.All<Part>().Where(x => x.Id == id);

                realm.Write(() =>
                {
                    realm.RemoveRange(toRemove);
                });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<IPart> GetParts(uint id)
        {
            using (var realm = ByfferHelper.GetBufferInstance())
            {
                var parts = realm
                    .All<Part>()
                    .Where(x => x.Id == id)
                    .ToList()
                    .Select(x => (IPart)new __Part(x))
                    .ToList();

                return parts;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetPartsCount(uint id)
        {
            using (var realm = ByfferHelper.GetBufferInstance())
            {
                var count = realm
                    .All<Part>()
                    .Where(x => x.Id == id)
                    .Count();

                return (uint)count;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        public void SavePart(IPart part)
        {
            using (var realm = ByfferHelper.GetBufferInstance())
            {
                realm.Write(() =>
                {
                    realm.Add(new Part()
                    {
                        InnerId = $"{part.Id}:{part.Index}",
                        Id = (int)part.Id,
                        Index = (int)part.Index,
                        TotalParts = (int)part.TotalParts,
                        Content = part.Content

                    }, update: true);
                });
            }
        }
    }
}
