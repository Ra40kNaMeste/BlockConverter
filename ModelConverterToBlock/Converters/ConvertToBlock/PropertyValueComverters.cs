using ModelConverterToBlock.Converters.Reader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.ConvertToBlock
{
    public class PropertyValueConverterParameters
    {
        public PropertyValueConverterParameters(RegexPropertyArray props)
        {
            Properties = props;
            IsContinue = true;
            Result = "";
        }
        public RegexPropertyArray Properties { get; set; }
        public string Result { get; set; }
        internal bool IsContinue { get; set; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class PropertyValueConverter : ICloneable, INotifyPropertyChanged
    {
        private PropertyValueConverter successor;
        [JsonProperty()]
        public PropertyValueConverter Successor
        {
            get { return successor; }
            set
            {
                successor = value;
                OnPropertyChanged();
            }
        }



        public virtual object Clone()
        {
            PropertyValueConverter res = (PropertyValueConverter)GetType().GetConstructor(new Type[] { }).Invoke(new object[] { });
            res.Successor = (PropertyValueConverter)Successor?.Clone();
            return res;
        }

        protected virtual void PropertiesToStringOverride(PropertyValueConverterParameters props) { }

        public void PropertiesToString(PropertyValueConverterParameters props)
        {
            Successor?.PropertiesToString(props);
            if (props.IsContinue)
                PropertiesToStringOverride(props);
        }
        public PropertyValueConverter SetSuccessor(PropertyValueConverter successor)
        {
            Successor = successor;
            return this;
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new(prop));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class PropertyEmptyValueConverter : PropertyValueConverter
    {

    }

    public interface IWordPropertyValueConverter
    {
        public string Word { get; set; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyUnionValueConverter : PropertyValueConverter, IWordPropertyValueConverter
    {
        public PropertyUnionValueConverter() => Word = "";
        public PropertyUnionValueConverter(string separator) => Word = separator;
        private string word;
        [JsonProperty()]
        public string Word
        {
            get { return word; }
            set
            {
                word = value;
                OnPropertyChanged();
            }
        }

        public override object Clone()
        {
            PropertyUnionValueConverter res = (PropertyUnionValueConverter)base.Clone();
            res.Word = Word;
            return res;
        }

        protected override void PropertiesToStringOverride(PropertyValueConverterParameters props)
        {
            StringBuilder res = new StringBuilder();
            foreach (var item in props.Properties)
            {
                res.Append(item.Name + Word);
            }
            int count = res.Length;
            if (count != 0)
                res.Remove(count - Word.Length, Word.Length);
            props.Result = res.ToString();
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyCleanExcapeSymbolsValueConverter : PropertyValueConverter
    {
        //private static PropertyCleanExcapeSymbolsValueConverter visual;
        //public static PropertyCleanExcapeSymbolsValueConverter GetInstance() => visual ??= new();
        public PropertyCleanExcapeSymbolsValueConverter() { }
        protected override void PropertiesToStringOverride(PropertyValueConverterParameters props)
        {
            props.Result = props.Result.Trim(new char[] { '\n', '\r', '\t', ' ' });
        }
        public override object Clone()
        {
            return base.Clone();
        }
    }

    public enum Direction
    {
        Left, Right
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyAppendWordValueConverter : PropertyUnionValueConverter
    {
        private Direction direction;
        [JsonProperty()]
        public Direction Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                OnPropertyChanged();
            }
        }

        public PropertyAppendWordValueConverter() { Word = ""; direction = Direction.Left; }
        public PropertyAppendWordValueConverter(string word) { Word = word; }
        protected override void PropertiesToStringOverride(PropertyValueConverterParameters props)
        {
            props.Result = Direction == Direction.Left ? Word + props.Result : props.Result + Word;
        }


        public override object Clone()
        {
            var res = (PropertyAppendWordValueConverter)base.Clone();
            res.Direction = Direction;
            res.Word = Word;
            return res;
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyIfNotValueConverter : PropertyUnionValueConverter
    {

        public PropertyIfNotValueConverter() : base() { }
        public PropertyIfNotValueConverter(string word) { Word = word; }

        protected override void PropertiesToStringOverride(PropertyValueConverterParameters props)
        {
            if (props.Result == "")
            {
                props.IsContinue = false;
                props.Result = Word;
            }
        }
    }

    public class PartNotFoundException : Exception
    {
        public PartNotFoundException() { }
        public PartNotFoundException(string part) : base(part) { }
    }

    public enum NamesPropertyValueConverterItem
    {
        Name,
        Default,
        Input,
        Output
    }

    public class PropertyValueConverterItem
    {
        public PropertyValueConverterItem(NamesPropertyValueConverterItem name, PropertyEmptyValueConverter converter)
        {
            Name = name;
            Converter = converter;
        }
        public NamesPropertyValueConverterItem Name { get; init; }
        public PropertyEmptyValueConverter Converter { get; init; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyGenerateValueConverterTemlplate:ICloneable
    {
        private static PropertyEmptyValueConverter GetDefaultConverter() => new()
        {
            Successor = new PropertyCleanExcapeSymbolsValueConverter()
            {
                Successor = new PropertyUnionValueConverter("")
            }
        };

        public PropertyGenerateValueConverterTemlplate()
        {
            Converters = new();
        }
        public PropertyGenerateValueConverterTemlplate(PropertyEmptyValueConverter converter) : this()
        {
            Converters.Add(new(NamesPropertyValueConverterItem.Default, converter));
        }
        public PropertyGenerateValueConverterTemlplate(IEnumerable<PropertyValueConverterItem> converters) : this()
        {
            foreach (var converter in converters)
            {
                Converters.Add(converter);
            }
        }

        [JsonProperty]
        private List<PropertyValueConverterItem> Converters { get; set; }

        public object Clone()
        {
            var converters = Converters.Select((i) => new PropertyValueConverterItem(i.Name, (PropertyEmptyValueConverter)i.Converter.Clone()));
            return new PropertyGenerateValueConverterTemlplate(converters);
        }

        public List<PropertyValueConverterItem> GetConverters()
        {
            return Converters;
        }
        public PropertyEmptyValueConverter GetConverter(NamesPropertyValueConverterItem name)
        {
            PropertyEmptyValueConverter res;
            TryCreateConverter(name, out res);
            return res;
        }
        public bool TryCreateConverter(NamesPropertyValueConverterItem name, out PropertyEmptyValueConverter result)
        {
            var res = Converters.Where(i => i.Name == name).FirstOrDefault();
            if (res == null)
            {
                res = new PropertyValueConverterItem(name, GetDefaultConverter());
                Converters.Add(res);
                result = res.Converter;
                return true;
            }
            result = res.Converter;
            return false;
        }

        public bool AddConverter(PropertyValueConverterItem converterItem)
        {
            if(RemoveConverter(converterItem.Name))
            {
                Converters.Add(converterItem);
                return true;
            }
            return false;
        }

        public bool RemoveConverter(NamesPropertyValueConverterItem name)
        {
            var temp = Converters.Where(i => i.Name == name).ToList();
            if (temp.Count() > 0)
            {
                foreach (var item in temp)
                {
                    Converters.Remove(item);
                }
                return true;
            }
            return false;
        }
    }

    public class PropertyValueTreeCollection : ICollection<PropertyValueConverter>, INotifyCollectionChanged
    {
        //Идём снизу вверх
        public PropertyEmptyValueConverter Root { get; init; }
        private ObservableCollection<PropertyValueConverter> Values { get; init; }

        public int Count
        {
            get { return Values.Count; }
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public PropertyValueTreeCollection(PropertyEmptyValueConverter root)
        {
            Root = root;
            Values = new();
            var next = root.Successor;
            if (next == null)
                return;
            Values.Add(next);
            while (next.Successor != null)
            {
                next = next.Successor;
                Values.Insert(0, next);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { Values.CollectionChanged += value; }
            remove { Values.CollectionChanged -= value; }
        }



        public void Add(PropertyValueConverter item)
        {
            if (Values.Contains(item) || item == Root)
                throw new ArgumentException("Обнаружены совпадающие элементы");
            Root.Successor = item;
            item.Successor = Values.LastOrDefault();
            Values.Add(item);
        }

        public void Clear()
        {
            Values.Clear();
            Root.Successor = null;
        }

        public bool Contains(PropertyValueConverter item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(PropertyValueConverter[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(PropertyValueConverter item)
        {
            int index = Values.IndexOf(item);
            if (index < 0)
                return false;
            if (index == Values.Count - 1)
                Root.Successor = item.Successor;
            else
                Values[index + 1].Successor = item.Successor;
            Values.Remove(item);
            return true;
        }

        public bool Insert(int index, PropertyValueConverter item)
        {
            if (index < 0 || index > Values.Count)
                return false;
            if (Values.Contains(item))
                throw new ArgumentException("Обнаружены совпадающие элементы");

            PropertyValueConverter next = index == Values.Count ? Root : Values[index];
            item.Successor = next.Successor;
            next.Successor = item;
            Values.Insert(index, item);
            return true;
        }

        public int IndexOf(PropertyValueConverter item) => Values.IndexOf(item);

        public IEnumerator<PropertyValueConverter> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }


    }

}
