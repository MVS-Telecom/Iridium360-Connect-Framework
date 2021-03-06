﻿using Iridium360.Connect.Framework.Exceptions;
using Iridium360.Connect.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework
{
    public interface IDeviceParameter
    {
        /// <summary>
        /// 
        /// </summary>
        Parameter Id { get; }

        /// <summary>
        /// 
        /// </summary>
        Guid GattId { get; }

        /// <summary>
        /// 
        /// </summary>
        bool HasCachedValue { get; }


        /// <summary>
        /// 
        /// </summary>
        Enum CachedValue { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void RestoreCachedValue(Enum value);

        /// <summary>
        /// Название группы, к которой относится параметр
        /// </summary>
        Group? Group { get; }

        /// <summary>
        /// Позиция в группе
        /// </summary>
        int OrderInGroup { get; }

        /// <summary>
        /// Параметр можно изменять
        /// </summary>
        bool IsReadonly { get; }

        /// <summary>
        /// Описание параметра
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Параметр доступен (можно прочитать/записать)
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Значение параметра не нужно очищать при отключении от устройства
        /// </summary>
        bool IsPersisted { get; }

        /// <summary>
        /// Это событие - значение не сохраняется
        /// </summary>
        bool IsEvent { get; }

        /// <summary>
        /// Возможные значения параметра
        /// </summary>
        List<Enum> Options { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsSwitch { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsCheck { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsList { get; }
    }




    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Id} -> {CachedValue}")]
    internal abstract class BaseDeviceParameter : IDeviceParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public Parameter Id { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public bool HasCachedValue
        {
            get
            {
                return cachedValue != null && cachedValue != byte.MaxValue;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected byte? cachedValue
        {
            get
            {
                return (bytes?.Length > 0 && bytes?.FirstOrDefault() >= 0 && bytes?.FirstOrDefault() < byte.MaxValue)
                    ? bytes.FirstOrDefault()
                    : (byte?)null;
            }
        }


        /// <summary>
        /// Значение параметра (кэшированное)
        /// </summary>
        public Enum CachedValue
        {
            get
            {
                if (IsEvent)
                    return null;

                if (type == null)
                    throw new ArgumentNullException($"Parameter `{Id}` enum type is null!");

                return Enum.GetValues(type).Cast<Enum>().FirstOrDefault(x => Convert.ToInt32(x) == cachedValue);
            }
        }

        /// <summary>
        /// Название группы, к которой относится параметр
        /// </summary>
        public Group? Group { get; private set; }

        /// <summary>
        /// Позиция в группе
        /// </summary>
        public int OrderInGroup { get; private set; }

        /// <summary>
        /// Параметр можно изменять
        /// </summary>
        public bool IsReadonly { get; private set; }

        /// <summary>
        /// Описание параметра
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Параметр доступен (можно прочитать/записать)
        /// </summary>
        public bool IsAvailable { get; internal set; } = false;

        /// <summary>
        /// Значение параметра не нужно очищать при отключении от устройства
        /// </summary>
        public bool IsPersisted { get; private set; }

        /// <summary>
        /// Это событие - значение не сохраняется
        /// </summary>
        public bool IsEvent { get; private set; }

        /// <summary>
        /// Возможные значения параметра
        /// </summary>
        public abstract List<Enum> Options { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSwitch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCheck { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsList { get; private set; }

        /// <summary>
        /// Id GATT характеристики по которой происходит чтение/запись параметра
        /// </summary>
        public Guid GattId { get; private set; }



        private DateTime? cachedTime;
        protected byte[] bytes = new byte[0];
        private IFramework framework;
        private IDevice device;
        protected Type type;


        public BaseDeviceParameter(IFramework framework, IDevice device, Parameter id)
        {
            this.Id = id;
            this.framework = framework;
            this.device = device;
            this.type = Id.GetAttribute<ValuesAttribute>()?.Value;

            string gatt = Id.GetAttribute<GattCharacteristicAttribute>()?.Value;

            if (!string.IsNullOrEmpty(gatt))
                this.GattId = Guid.Parse(gatt);


            IsReadonly = id.HasAttribute<ReadonlyAttribute>();
            Group = id.GetAttribute<GroupAttribute>()?.Key;
            OrderInGroup = id.GetAttribute<GroupAttribute>()?.Order ?? 0;
            Description = id.GetAttribute<DescriptionAttribute>()?.Value;
            IsEvent = id.HasAttribute<EventAttribute>();
            IsPersisted = id.HasAttribute<PersistedAttribute>();

            IsSwitch = id.HasAttribute<SwitchAttribute>();
            IsCheck = id.HasAttribute<CheckAttribute>();
            IsList = id.HasAttribute<ListAttribute>();

            //var type = id.GetAttribute<ValuesAttribute>()?.Value;

            //if (type != null)
            //{
            //    Options = Enum.GetValues(type)
            //        .OfType<Enum>()
            //        .Where(x => !x.HasAttribute<HiddenAttribute>())
            //        .OrderBy(x => x.GetAttribute<OrderAttribute>()?.Value ?? int.MaxValue)
            //        .ToList();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RestoreCachedValue(Enum value)
        {
            if (value == null)
                return;

            try
            {
                UpdateCachedValue(new int[] { Convert.ToInt32(value) });
            }
            catch (DeviceIsLockedException e1)
            {
                Debugger.Break();
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="new"></param>
        /// <returns>Изменился?</returns>
        internal bool UpdateCachedValue(int[] @new)
        {
            if (IsEvent)
            {
                ConsoleLogger.WriteLine($"Event: `{Id}`");
                return true;
            }

            if (@new != null && @new.Length != 1)
            {
                Debugger.Break();
                throw new Exception($"Length of value for parameter is {@new.Length}");
            }

            if (@new?.FirstOrDefault() == byte.MaxValue || @new?.FirstOrDefault() == 999 || @new?.FirstOrDefault() == -1)
                throw new DeviceIsLockedException();



            bool changed = @new?.FirstOrDefault() != cachedValue;

            this.bytes = new byte[1] { (byte)@new[0] };
            this.cachedTime = DateTime.Now;

            if (changed)
            {
                ConsoleLogger.WriteLine($"Parameter changed: [{Id}] -> {CachedValue}");
            }

            return changed;
        }


        public override string ToString()
        {
            return $"{Id} -> {CachedValue}";
        }
    }
}
