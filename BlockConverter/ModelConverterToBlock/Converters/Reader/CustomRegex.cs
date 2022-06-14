using ModelConverterToBlock.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ModelConverterToBlock.Converters.Reader
{
    //Класс, который ищет в тексте все свойства, сопостовляя с шаблоном
    class CustomRegex
    {
        private List<CustomRegexComponent> roots = new();
        internal CustomRegex(string pattern, PropertyContext context)
        {
            roots = StringInterprets.GenerationLinkSystem(pattern, context);
        }
        /// <summary>
        /// Найти свойства в строке
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="startIndex">начало поиска</param>
        /// <param name="length">длина анализа строки</param>
        /// <returns></returns>
        public RegexPropertyArray FindProperties(string str, int startIndex, int length)
        {
            List<CustomRegexLinkData> dates = new();

            foreach (var root in roots)
            {
                CustomRegexLinkData data = root.Start(str, startIndex, length);
                if (!data.IsSuccuss)
                    return null;
                dates.Add(data);
            }
            if (dates.Count != 0)
            {
                RegexPropertyArray res = null;
                bool isFirst = true;
                foreach (var data in dates)
                {
                    if (isFirst)
                    {
                        res = ConvertToArray(data);
                        isFirst = false;
                    }
                    else
                        res.Union(ConvertToArray(data));
                }
                return res;
            }
            return null;
        }
        //Преобразует CustomRegexLinkData в RegexPropertyArray
        private static RegexPropertyArray ConvertToArray(CustomRegexLinkData data)
        {
            return new RegexPropertyArray("Properties", data.Properties.Properties).AbjustStartCorr(data.Index).AbjustEndCorr(data.Index + data.Length);
        }
    }
    //Делает то же. что и CustomRegex, но компонует свойства в другие свойства
    class CustomRegexByPattern
    {
        private CustomRegex TargetRegex { get; set; }
        private Dictionary<string, PropertyArrangemetrenBase> Finders { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">шаблон разбиения</param>
        /// <param name="context">контекст</param>
        /// <param name="patternProperties">библиотека, где ключ - имя свойства, значение - шаблон</param>
        internal CustomRegexByPattern(string pattern, PropertyContext context, Dictionary<string, string> patternProperties)
        {
            TargetRegex = new(pattern, context);
            Finders = new();
            foreach (var item in patternProperties)
            {
                Finders.Add(item.Key, CreateChain(item.Value));
            }
        }
        //создание цепочки шаблона
        private static PropertyArrangemetrenBase CreateChain(string pattern)
        {

            IEnumerable<string> links = StringInterprets.FindBlockLink(pattern);
            PropertyArrangemetrenBase res = null, current;
            foreach (var item in links)
            {
                Match match = StringInterprets.propertyRegex.Match(item);
                if (match.Success)
                    current = new PropertyArrangemetren(new PropertyFinder(item));
                else
                    current = new StringArrangemetren(item);
                if (res == null)
                    res = current;
                else
                {
                    current.Previouns = res;
                    res = current;
                }

            }
            return res;
        }
        /// <summary>
        /// Найти свойства в строке
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="startIndex">начало поиска</param>
        /// <param name="length">длина анализа строки</param>
        /// <returns></returns>
        public RegexPropertyArray FindProperties(string str, int startIndex, int length)
        {
            RegexPropertyArray FirstArray = TargetRegex.FindProperties(str, startIndex, length);
            if (FirstArray != null)
            {

                List<IRegexProperty> output = new();
                foreach (var finder in Finders)
                {
                    PropertyArrangemetrenOutputs outputs = new();
                    finder.Value?.Arrange(FirstArray, outputs);
                    output.Add(new RegexPropertyArray(finder.Key, outputs.properties.Properties));
                }
                RegexPropertyArray arr = new RegexPropertyArray("Root", output);
                arr.AbjustStartCorr(FirstArray.StartIndex);
                arr.AbjustEndCorr(FirstArray.EndIndex);
                return arr;
            }

            return null;
        }
    }

    //Специальный наследник IRegexProperty. Имеет независимую длину от Name. Т.е. длину нужно устанавливать самостоятельно
    public class RegexDynamicSizeProperty : IRegexProperty
    {
        public RegexDynamicSizeProperty()
        {
            Name = "";
            StartIndex = int.MaxValue;
            EndIndex = 0;
        }
        public int StartIndex { get; set; }

        public int Length { get { return EndIndex - StartIndex; } }

        public int EndIndex { get; set; }
        private StringBuilder name = new();
        public string Name 
        {
            get { return name.ToString(); }
            private set { }
        }
        /// <summary>
        /// Добавить к имени строку
        /// </summary>
        /// <param name="value">строка для добавления</param>
        public void AddByName(string value) => name.Append(value);
        public ICollection<IRegexProperty> GetProperties()
        {
            return new List<IRegexProperty>();
        }
        public RegexDynamicSizeProperty Union(IRegexProperty property)
        {
            AddByName(property.Name);
            StartIndex = Math.Min(StartIndex, property.StartIndex);
            EndIndex = Math.Max(EndIndex, property.EndIndex);
            return this;
        }
    }
    //Класс находит свойства по шаблону
    class PropertyFinder
    {
        public PropertyFinder() { }
        public PropertyFinder(string property) => Property = StringInterprets.ConvertFindBlockLinkProperty(property).Split('.');
        public PropertyFinder Child { get; set; }
        public string[] Property { get; set; }
        /// <summary>
        /// найти свойства
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ICollection<IRegexProperty> FindProperties(IRegexProperty target)
        {
            var props = target.GetProperties();
            if (props == null || Property.Length == 0)
                return null;
            ICollection<IRegexProperty> property = FindPropertyRecurs(props, 0);
            if (Child == null)
                return property;
            return Child.FindProperties(target);
        }
        private ICollection<IRegexProperty> FindPropertyRecurs(ICollection<IRegexProperty> properties, int i)
        {
            ICollection<IRegexProperty> res = new List<IRegexProperty>();
            foreach (var item in properties)
            {
                if (item.Name == Property[i])
                {
                    var props = item.GetProperties();
                    if (props != null)
                        if (i < Property.Length - 1)
                            res = res.Concat(FindPropertyRecurs(props, i + 1)).ToList();
                        else
                            res.Add(item);
                }
            }
            return res;
        }
    }

    class PropertyArrangemetrenOutputs
    {
        public PropertyArrangemetrenOutputs() => properties = new("", new List<IRegexProperty>());
        public PropertyArrangemetrenOutputs(string name) => properties = new(name, new List<IRegexProperty>());
        public readonly RegexPropertyArray properties;
    }
    //наследники класса составляют своства. При наследовании реализовать метод GetValue, 
    //который добавляет значение в конечное свойство
    abstract class PropertyArrangemetrenBase
    {
        protected abstract void GetValue(IRegexProperty target, PropertyArrangemetrenOutputs outputs);
        public PropertyArrangemetrenBase Previouns { get; set; }
        /// <summary>
        /// Составить новые свойства
        /// </summary>
        /// <param name="target">Массив старыъх свойств</param>
        /// <param name="outputs"> Выход метода</param>
        public void Arrange(IRegexProperty target, PropertyArrangemetrenOutputs outputs)
        {
            Previouns?.Arrange(target, outputs);
            GetValue(target, outputs);
        }
    }
    //Наследники
    class PropertyArrangemetren : PropertyArrangemetrenBase
    {
        public PropertyArrangemetren(PropertyFinder finder) => this.finder = finder;
        private PropertyFinder finder;

        protected override void GetValue(IRegexProperty target, PropertyArrangemetrenOutputs outputs)
        {
            ICollection<IRegexProperty> props = finder.FindProperties(target);
            //bool isFirst = true;
            ICollection<IRegexProperty> findProps = new List<IRegexProperty>();
            foreach (var prop in props)
            {
                var items = prop.GetProperties();
                foreach (var item in items)
                {
                    findProps.Add(item);
                }
            }
            outputs.properties.Union(new RegexPropertyArray("", findProps));
        }
    }
    class StringArrangemetren : PropertyArrangemetrenBase
    {
        public StringArrangemetren(string value) => val = value;
        private string val;
        protected override void GetValue(IRegexProperty target, PropertyArrangemetrenOutputs outputs) => outputs.properties.Union(new RegexPropertyArray("", new List<IRegexProperty>() { new RegexPropertyValue(val, -1) }));
    }
    class RegexPropertyString : IRegexProperty
    {
        public RegexPropertyString(string name) => Name = name;
        public int StartIndex { get { return -1; } }

        public int Length { get { return -1; } }

        public string Name { get; init; }

        public int EndIndex { get { return -1; } }

        public ICollection<IRegexProperty> GetProperties()
        {
            return new List<IRegexProperty>();
        }
    }

}
