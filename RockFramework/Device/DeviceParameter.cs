using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Rock.Util;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Rock.Exceptions;

namespace Rock
{
    [DebuggerDisplay("{Id} -> {CachedValue}")]
    public class DeviceParameter
    {
        public readonly Parameter Id;
        private int? cachedValue;

        /// <summary>
        /// Значение параметра (кэшированное)
        /// </summary>
        public Enum CachedValue
        {
            get
            {
                var type = Id.GetAttribute<ValuesAttribute>()?.Value;

                if (type == null || cachedValue == null)
                    return null;

                return Enum.GetValues(type).Cast<Enum>().FirstOrDefault(x => Convert.ToInt32(x) == cachedValue.Value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool HasCachedValue
        {
            get
            {
                return RawValue != null && RawValue.Length == 1 && RawValue[0] != byte.MaxValue;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public byte[] RawValue
        {
            get
            {
                return cachedValueBytes;
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
        public List<Enum> Options { get; private set; } = new List<Enum>();

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
        public readonly Guid GattId;



        private Hashtable options = new Hashtable();
        private DateTime cachedTime;
        protected byte[] cachedValueBytes = new byte[0];
        private IFramework Rock;
        private IDevice device;


        public DeviceParameter(IFramework Rock, IDevice device, Parameter id)
        {
            this.Rock = Rock;
            this.device = device;
            this.Id = id;

            string gatt = id.GetAttribute<GattCharacteristicAttribute>()?.Value;

            if (!string.IsNullOrEmpty(gatt))
                this.GattId = Guid.Parse(gatt);
            else
                Debugger.Break();

            IsReadonly = id.HasAttribute<ReadonlyAttribute>();
            Group = id.GetAttribute<GroupAttribute>()?.Key;
            OrderInGroup = id.GetAttribute<GroupAttribute>()?.Order ?? 0;
            Description = id.GetAttribute<DescriptionAttribute>()?.Value;
            IsEvent = id.HasAttribute<EventAttribute>();
            IsPersisted = id.HasAttribute<PersistedAttribute>();

            IsSwitch = id.HasAttribute<SwitchAttribute>();
            IsCheck = id.HasAttribute<CheckAttribute>();
            IsList = id.HasAttribute<ListAttribute>();

            var type = id.GetAttribute<ValuesAttribute>()?.Value;

            if (type != null)
            {
                Options = Enum.GetValues(type)
                    .OfType<Enum>()
                    .Where(x => !x.HasAttribute<HiddenAttribute>())
                    .OrderBy(x => x.GetAttribute<OrderAttribute>()?.Value ?? int.MaxValue)
                    .ToList();
            }
        }

        public Task SaveValue(Enum value)
        {
            return device.SaveDeviceParameter(this.Id, value);
        }

        public void removeValueOption(Enum value)
        {
            this.Options.Remove(value);
        }

        public DateTime getCachedTime()
        {
            return this.cachedTime;
        }


        public byte[] getCachedValueBytes()
        {
            return this.cachedValueBytes;
        }


        public Hashtable getOptions()
        {
            return this.options;
        }

        public List<Enum> getOptionsIndex()
        {
            return this.Options;
        }

        public bool isCachedValueUsable()
        {
            return this.cachedValue.HasValue && this.cachedValue != 999;
        }

        public string labelForValue(int value)
        {
            return (string)this.options[value];
        }

        public void setOptions(Hashtable options)
        {
            this.options = options;
        }

        public void setOptionsIndex(List<Enum> optionsIndex)
        {
            this.Options = optionsIndex;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Изменился?</returns>
        public bool UpdateCachedValue(byte[] value)
        {
            if (IsEvent)
            {
                ConsoleLogger.WriteLine($"Event: `{Id}`");
                return true;
            }

            if (value != null && value.Length != 1)
            {
                throw new Exception($"Length of value for parameter is {value.Length}");
            }

            if (value?.FirstOrDefault() == byte.MaxValue)
            {
                throw new DeviceIsLockedException();
            }


            bool changed = value?.FirstOrDefault() != cachedValue;

            this.cachedValueBytes = value;
            this.cachedValue = value?.FirstOrDefault();
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
