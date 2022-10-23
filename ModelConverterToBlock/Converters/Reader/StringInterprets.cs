using ModelConverterToBlock.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    //интерпретирует строки. При добавлении новых компонентов добавлять их сюда
    public static class StringInterprets
    {
        #region Parameters
        //Общий вид какого-либо компонента шаблона
        private static readonly Regex findLinkRegex = new(@"#[(][*]?!?(\w|[.])*[)]");
        //Общий вид свойства, не являющегося спец. компонентом шаблона (т.е. все наследникии сама PropertyData)
        public static readonly Regex propertyRegex = new(@"#[(](\w|[.])*[)]");

        public const string PropertyOrSpliter = "|";
        //Список функций для поиска. Если функция возвращает null, то берётся следующая функция
        private static List<Func<string, PropertyContext, LinkReaderDataBase>> funcConverts = new()
        {
            Primitive,
            PropertyReaderLink

        };
        #endregion //Parameters

        #region StringMethods
        //Поиск реализации компонента свойств по ключу.
        internal static LinkReaderDataBase PropertyReaderLink(string key, PropertyContext context)
        {
            Match match = propertyRegex.Match(key);
            if (!match.Success)
                return null;
            key = ConvertFindBlockLinkProperty(match.Value);
            return context.Context.ContainsKey(key) ? (LinkReaderDataBase)context.Context[key].Clone() : null;
        }
        /// <summary>
        /// Генерация цепочки по шаблону
        /// </summary>
        /// <param name="pattern">шаблон</param>
        /// <param name="context">контекст</param>
        /// <returns>цепочка для поиска конструкций</returns>
        internal static List<CustomRegexComponent> GenerationLinkSystem(string pattern, PropertyContext context)
        {
            List<CustomRegexComponent> res = new();
            ICollection<string> strs = FindBlockLink(pattern);
            LinkReaderDataBase current;
            CustomRegexComponent regexStruct = new();
            foreach (var item in strs)
            {
                current = ConvertToLinkReader(item, context);
                regexStruct.Add(current);
                if (regexStruct.IsFill)
                {
                    res.Add(regexStruct);
                    regexStruct = new();
                }
            }
            if (!regexStruct.IsFill)
                res.Add(regexStruct);
            return res;
        }
        /// <summary>
        /// Поиск тела в строке
        /// </summary>
        /// <param name="data">параметры</param>
        /// <param name="startRegex">начало тела</param>
        /// <param name="endRegex">конец тела</param>
        /// <returns></returns>
        internal static string GetBody(CustomRegexLinkData data, Regex startRegex, Regex endRegex)
        {
            string str = data.Root;
            int startIndex = data.Index;
            int startCount = 1, endCount = 0;
            int startLength, endLenght;

            Match startMatch = startRegex.Match(str, startIndex);
            if (!startMatch.Success)
                throw new ArgumentException("Не возможно найти начало блока");
            startLength = startMatch.Length;
            Match endMatch = endRegex.Match(str, startIndex);
            endLenght = endMatch.Length;
            startMatch = startMatch.NextMatch();
            Match match = startMatch;
            while (startCount != endCount)
            {
                if (startMatch.Success && endMatch.Success)
                {
                    if (startMatch.Index < endMatch.Index)
                    {
                        startCount++;
                        match = startMatch;
                        startMatch = startMatch.NextMatch();

                    }
                    else if (endMatch.Success && endMatch.Index < startMatch.Index)
                    {
                        endCount++;
                        match = endMatch;
                        endMatch = endMatch.NextMatch();

                    }

                }
                else if (!startMatch.Success && endMatch.Success)
                {
                    endCount++;
                    match = endMatch;
                    endMatch = endMatch.NextMatch();
                }
                else
                    throw new InvalidSkriptException("Не удалось обнаружить конец", match.Index);
            }
            startIndex = match.Index;
            return str.Substring(data.Index + startLength, startIndex - data.Index - startLength);
        }
        /// <summary>
        /// разбиение строки на подстроки в соответствии с шаблоном findLinkRegex
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ICollection<string> FindBlockLink(string str)
        {
            ICollection<string> res = new List<string>();
            FindBlockLinkRecurs(new RightLinkData(str, 0, str.Length), res);
            return res;
        }
        #endregion//StringMethods

        #region Converters
        /// <summary>
        /// Преобразует в элемент цепочки
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="context">конекст данных для преобразования</param>
        /// <returns>элемент цепочки для поиска конструкций</returns>
        internal static LinkReaderDataBase ConvertToLinkReader(string str, PropertyContext context)
        {
            LinkReaderDataBase res = null;
            foreach (var item in funcConverts)
            {
                res = item.Invoke(str, context);
                if (res != null)
                    break;
            }
            if (res == null)
                return new WordReader(new CommonWordFromLinksGetter(str));
            return res;
        }
        /// <summary>
        /// Избавление от обёртки свойств
        /// </summary>
        /// <param name="property">свойство в обёртке</param>
        /// <returns>свойство без обёртки</returns>
        public static string ConvertFindBlockLinkProperty(string property) => property[2..^1];
        //Самые примитивные компоненты шаблона. Сюда их добавлять
        private static LinkReaderDataBase Primitive(string key, PropertyContext context) => key switch
        {
            "#(w)" => new WordSkiper(),
            "#(!w)" => new NotWordSkiper(),
            "#(s)" => new SpaceSkiper(true),
            "#(*s)" => new SpaceSkiper(false),
            "#(*)" => new Skiper(),
            _ => null
        };
        //добавлять новые наследники IContextNodeReader
        /// <summary>
        /// конвертер типа блока контекста.
        /// </summary>
        /// <param name="type">тип нода</param>
        /// <returns></returns>
        internal static IContextNodeReaderable ConvertToNodeReader(string type) => type switch
        {
            "StretchProperty" => StretchPropertyContextNodeReader.GetObject(),
            "Property" => PropertyContextNodeReader.GetObject(),
            "Body" => BodyContextNodeReader.GetObject(),
            "ArrayParams" => ArrayParamsContextNodeReader.GetObject(),
            "BlockText" => BlockTextToWordNodeReader.GetObject(),
            "Optional" => OptionalContextNodeReader.GetObject(),
            "ByPattern" => ByPatternNodeReader.GetObject(),
            _ => null
        };


        public static BlockProperty ConvertToBlockProperty(string property) => (BlockProperty)Enum.Parse(typeof(BlockProperty), property);

        #endregion

        private static void FindBlockLinkRecurs(CustomRegexLinkData data, ICollection<string> res)
        {
            Match match = findLinkRegex.Match(data.Root, data.Index);
            if (match.Success)
            {
                if (match.Index != data.Index)
                {
                    string str = data.Root.Substring(data.Index, Math.Abs(match.Index - data.Index));
                    res.Add(str);
                    data.Addition(str.Length);
                }
                res.Add(match.Value);
                data.AddMatch(match);
                FindBlockLinkRecurs(data, res);
                return;
            }
            int lengthWord = data is LeftLinkData ? match.Index : data.Root.Length - data.Index;
            if (lengthWord != 0)
                res.Add(data.Root.Substring(data.Index, lengthWord));
            data.Addition(lengthWord);
        }
    }

}
