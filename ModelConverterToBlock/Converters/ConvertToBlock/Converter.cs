using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConverterToBlock.Converters.Reader;
using ModelConverterToBlock.Blocks;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;
using ModelConverterToBlock.Logger;
using System.Resources;

namespace ModelConverterToBlock.Converters.ConvertToBlock
{
    /// <summary>
    /// Класс для конвертации текста в блоки
    /// </summary>
    public class ConverterToBlock : INotifyPropertyChanged
    {
        public ConverterToBlock()
        {
            Reader = new();
        }

        #region Methods

        #region ReadFileInstructions
        /// <summary>
        /// Отменить чтение из файла
        /// </summary>
        public void CancelRead()
        {
            tokenReadSource.Cancel();
            tokenReadSource = new();
            BlockLoggerManager.LogAsync(Resources.ConverterToBlockCancelReadMessage, LogLevels.Info, Thread.CurrentThread);
        }

        /// <summary>
        /// Чтение инструкций из файла в асинхронном режиме 
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public async void ReadAsync(string path)
        {
            await Task.Run(() => Read(path, tokenReadSource.Token));
        }

        public async void ReadAsync(Uri path)
        {
            await Task.Run(() => Read(path, tokenReadSource.Token));
        }

        /// <summary>
        /// Чтение инструкций из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Read(string path)
        {

            IsRead = false;
            Reader.Read(path);
            IsRead = true;
            BlockLoggerManager.Log(string.Format(Resources.ConverterToBlockReadMessage, path), LogLevels.Info, Thread.CurrentThread);
        }
        public void Read(Uri path)
        {

            IsRead = false;
            Reader.Read(path);
            IsRead = true;
            BlockLoggerManager.Log(string.Format(Resources.ConverterToBlockReadMessage, path.OriginalString), LogLevels.Info, Thread.CurrentThread);

        }
        private void Read(string path, CancellationToken token)
        {
            lock (ReadLocker)
            {
                if (!token.IsCancellationRequested)
                    Read(path);
            }
        }
        private void Read(Uri path, CancellationToken token)
        {
            lock (ReadLocker)
            {
                if (!token.IsCancellationRequested)
                    Read(path);
            }
        }
        #endregion //ReadFileInstructions

        #region Convert

        /// <summary>
        /// Отменить конвертацию
        /// </summary>
        public void CancelConvert()
        {
            tokenConvertSource.Cancel();
            tokenConvertSource = new();
            BlockLoggerManager.Log(Resources.ConverterToBlockCancelConvertMessage, LogLevels.Info, Thread.CurrentThread);
        }
        /// <summary>
        /// Производит поиск функций и их конвертацию в блоки
        /// </summary>
        /// <param name="str">строка с кодом</param>
        /// <returns></returns>
        public List<BeginBlockComponent> Convert(string str)
        {
            List<FuncConvertLink> converters = new();
            if (!IsRead)
                throw new PartNotFoundException(Resources.ConvertToBlockFailedNotLoadLaguange);
            //Загрузка конструкций
            var constructions = Reader.Context.Constructions;
            if (!constructions.ContainsKey(BlockConstruction.Function))
                throw new PartNotFoundException(Resources.ConvertToBlockFailedNotFindFunctionInstruction);
            //Создание нового контекста
            ConverterContext context = new(str, constructions);
            //Поиск определений функций
            var funcRegexes = constructions[BlockConstruction.Function];
            RegexPropertyArray array;
            int startIndex = 0;
            bool isNotFindFunc = false;
            while (!isNotFindFunc)
            {
                isNotFindFunc = true;
                foreach (var item in funcRegexes)
                {
                    if ((array = item.FindProperties(str, startIndex, str.Length - startIndex)) != null)
                    {
                        var temp = (FuncConvertLink)ConverterSetting.ConvertToLink(BlockConstruction.Function, array);
                        converters.Add(temp);
                        startIndex = temp.End;
                        isNotFindFunc = false;
                        break;
                    }
                }
            }
            BlockLoggerManager.LogAsync(string.Format(Resources.ConvertToBlockFindFunction, converters.Count), LogLevels.Info, Thread.CurrentThread);
            List<BeginBlockComponent> res = new();
            object resLocker = new();
            var resConverters = Parallel.ForEach(converters,
                new ParallelOptions() { CancellationToken = tokenConvertSource.Token }, (i) => 
                {
                    BeginBlockComponent result = i.Convert(context);
                    lock(resLocker)
                        res.Add(result);
                    });
            if (resConverters.IsCompleted)
                return res;
            return new List<BeginBlockComponent>();
        }

        #endregion //Convert

        #endregion//Methods

        #region PrivateFields

        private object ReadLocker = new object();
        private CancellationTokenSource tokenReadSource = new();
        private CancellationTokenSource tokenConvertSource = new();


        #endregion //ProvateFields

        #region Properties
        private bool isRead = false;
        /// <summary>
        /// Готов ли к конвертации
        /// </summary>
        public bool IsRead
        {
            set
            {
                isRead = value;
                OnPropertyChanged();

            }
            get { return isRead; }
        }
        private PatternlanguangeReader reader;
        public PatternlanguangeReader Reader
        {
            get { return reader; }
            private set
            {
                reader = value;
                OnPropertyChanged();
            }
        }

        #endregion //Properties


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public static class ConverterSetting
    {
        #region PropertyConverterSavePaths

        private const string pathPropertyConverters = "PropertyConverters.json";
        private const string pathDefaultPropertyConverters = "DefaultResource/DefaultPropertyConverters.json";

        #endregion //PropertyConverterSavePaths

        static ConverterSetting()
        {
            PropertyConverters = new();
        }

        /// <summary>
        /// Коллекция для конвертации массива свойств в одно свойство
        /// </summary>
        public static Dictionary<BlockConstruction, PropertyGenerateValueConverterTemlplate> PropertyConverters { get; set; }
        
        /// <summary>
        /// Сохранение коллекции для конвертации массива свойств
        /// </summary>
        public static void SavePropertyConverters()
        {
            string res = JsonConvert.SerializeObject(PropertyConverters, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            using StreamWriter fs = new(Path.GetFullPath(pathPropertyConverters), false);
            fs.WriteLine(res);
            BlockLoggerManager.LogAsync(Resources.ConverterSettingPropertyConvertersSaveMessage, LogLevels.Debug, Thread.CurrentThread);
        }

        /// <summary>
        /// Чтение коллекции для конвертации массива свойств
        /// </summary>
        public static void OpenPropertyConverters()
        {
            if (!LoadPropertyConverters(pathPropertyConverters))
            {
                LoadPropertyConverters(pathDefaultPropertyConverters);
                BlockLoggerManager.LogAsync(Resources.ConverterSettingLoadDefaultConverters, LogLevels.Info, Thread.CurrentThread);
            }
            else
                BlockLoggerManager.LogAsync(Resources.ConverterSettingLoadCustomConverters, LogLevels.Info, Thread.CurrentThread);

            if(FillNotFindPropertyConverters())
                BlockLoggerManager.LogAsync(Resources.ConverterSettingNotFindConverters, LogLevels.Info, Thread.CurrentThread);

            if (PropertyConverters.ContainsKey(BlockConstruction.None))
                PropertyConverters.Remove(BlockConstruction.None);
        }

        /// <summary>
        /// Читает файл, сохранённый пользователем
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Удалось ли считать</returns>
        private static bool LoadPropertyConverters(string path)
        {

            try
            {
                using StreamReader sr = new(path);
                string res = sr.ReadToEnd();
                PropertyConverters = JsonConvert.DeserializeObject<Dictionary<BlockConstruction, PropertyGenerateValueConverterTemlplate>>
                    (res, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                BlockLoggerManager.LogAsync(string.Format(Resources.ConverterSettingLoadPropertiesFromFileMessage, path), LogLevels.Debug, Thread.CurrentThread);
                return true;
            }
            catch (Exception)
            {
                BlockLoggerManager.LogAsync(string.Format(Resources.ConverterSettingFailedLoadPropertiesFromFileMessage, path), LogLevels.Debug, Thread.CurrentThread);
                return false;
            }
        }

        /// <summary>
        /// Проверяет наотсутствие конвертеров для свойств и дополняет
        /// </summary>
        /// <returns>Было ли добавлены непрочитанные свойства</returns>
        private static bool FillNotFindPropertyConverters()
        {
            bool res = false;
            var properties = Enum.GetValues(typeof(BlockConstruction));
            RegexPropertyArray regices = new("", new List<IRegexProperty>());
            foreach (BlockConstruction item in properties)
            {
                if (item == BlockConstruction.None || PropertyConverters.ContainsKey(item))
                    continue;
                res = true;
                PropertyConverters.Add(item, new PropertyGenerateValueConverterTemlplate());
                ConvertToLink(item, regices);
                BlockLoggerManager.LogAsync(string.Format(Resources.ConverterSettingAddingNewGenerationPropertyMessage, item.ToString()), LogLevels.Debug, Thread.CurrentThread);
            }
            return res;
        }

        /// <summary>
        /// Находит конвертер для конструкции
        /// </summary>
        /// <param name="construction">Конструкция</param>
        /// <param name="properties">Свойства, которые необходимо конвертировать</param>
        /// <returns></returns>
        internal static ConverterLinkToBlockBase ConvertToLink(BlockConstruction construction, RegexPropertyArray properties) => construction switch
        {
            BlockConstruction.Return => new BlockConverterLink(properties, new BlockEndComponent(), PropertyConverters?[construction]),
            BlockConstruction.Switch => new SwitchConverterLink(properties, PropertyConverters?[construction]),
            BlockConstruction.If => new IfConvertLink(properties, PropertyConverters?[construction]),
            BlockConstruction.Cycle => new CycleConvertLink(new BlockCycleComponent(), properties, PropertyConverters?[construction]),
            BlockConstruction.PostCycle => new CycleConvertLink(new BlockPostCycleComponent(), properties, PropertyConverters?[construction]),
            BlockConstruction.ForCycle => new CycleConvertLink(new BlockForComponent(), properties, PropertyConverters?[construction]),
            BlockConstruction.Function => new FuncConvertLink(properties, PropertyConverters?[construction], PropertyConverters?[BlockConstruction.Return]),
            BlockConstruction.Operation => new BlockConverterLink(properties, new BlockComponent(), PropertyConverters?[construction]),
            BlockConstruction.Input => new BlockConverterLink(properties, new BlockInputComponent(), PropertyConverters?[construction]),
            BlockConstruction.Output => new BlockConverterLink(properties, new BlockOutputComponent(), PropertyConverters?[construction]),
            BlockConstruction.Method => new MethodConvertLink(properties, PropertyConverters?[construction]),

            _ => throw new NotImplementedException(),
        };

    }

    /// <summary>
    /// Контекст для конвертации
    /// </summary>
    class ConverterContext
    {
        /// <summary>
        /// Строка, в которой происходит поиск
        /// </summary>
        public string Root { get; init; }

        /// <summary>
        /// Определения дополненных регулярных выражений для поиска в куске кода определений блоков
        /// </summary>
        private Dictionary<BlockConstruction, List<CustomRegexByPattern>> Regexes { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">Строка для поиска</param>
        /// <param name="regexes">Коллекция определений блоков</param>
        public ConverterContext(string root, Dictionary<BlockConstruction, List<CustomRegexByPattern>> regexes)
        {
            Root = root;
            Regexes = regexes.OrderByDescending((i) => i.Key).ToDictionary((i) => i.Key, (i) => i.Value);
        }

        /// <summary>
        /// Ищет цепочку из найденных блоков в свойстве
        /// </summary>
        /// <param name="property">Свойство для поиска</param>
        /// <returns>Цепочка блоков</returns>
        public IEnumerable<ConverterLinkToBlockBase> GetBodyProperty(IRegexProperty property)
        {
            List<ConverterLinkToBlockBase> res = new();
            int start = property.StartIndex, end = property.EndIndex;
            RegexPropertyArray array;
            while (start <= end && end - start > 0 && (array = FindConstructionProperties(start, end, out BlockConstruction construction, new List<BlockConstruction>() { BlockConstruction.Function })) != null && array.Length != 0)
            {
                res.Add(ConverterSetting.ConvertToLink(construction, array));
                start = array.StartIndex + array.Length;
                BlockLoggerManager.LogAsync(string.Format(Resources.ConverterContextAddConstructionMessage,
                    construction.ToString(), array.StartIndex, array.Length), LogLevels.Debug, Thread.CurrentThread);
            }
            return res;
        }
        
        /// <summary>
        /// Ищет в куске кода ближайшее определение блока
        /// </summary>
        /// <param name="start">Начало поиска</param>
        /// <param name="end">Конец поиска</param>
        /// <param name="construction">Найденная конструкция</param>
        /// <param name="excludeConstructions">Исключаемые из поиска конструкции</param>
        /// <returns>Первое найденное определение блока</returns>
        private RegexPropertyArray FindConstructionProperties(int start, int end, out BlockConstruction construction, IEnumerable<BlockConstruction> excludeConstructions)
        {
            List<(RegexPropertyArray, BlockConstruction)> finders = new();
            foreach (var item in Regexes)
            {
                if (excludeConstructions.Contains(item.Key))
                    continue;

                List<CustomRegexByPattern> patternRegexes = item.Value;
                foreach (var regex in patternRegexes)
                {
                    RegexPropertyArray array = regex.FindProperties(Root, start, end - start);
                    if (array != null && array.StartIndex + array.Length <= end)
                    {
                        finders.Add((array, item.Key));
                        BlockLoggerManager.LogAsync(string.Format(
                            Resources.ConverterContextFindConstructionMessage, item.Key, array.StartIndex, array.Length),
                            LogLevels.Debug, Thread.CurrentThread);
                    }
                }
            }
            if (finders.Count == 0)
            {
                construction = BlockConstruction.None;
                return null;
            }
            var min = finders.Aggregate((min, curr) => curr.Item1.StartIndex < min.Item1.StartIndex ? curr : min);
            construction = min.Item2;

            return min.Item1;
        }
    }

    /// <summary>
    /// Классы для конвертации массива свойств в блоки, с этими свойствами
    /// </summary>
    abstract class ConverterLinkToBlockBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockProperties">Массив свойств</param>
        /// <param name="visual">Конвертер свойства Name</param>
        public ConverterLinkToBlockBase(RegexPropertyArray blockProperties, PropertyGenerateValueConverterTemlplate visual)
        {
            ArrayProperties = blockProperties;
            Start = blockProperties.StartIndex;
            End = blockProperties.StartIndex + blockProperties.Length;

            Visual = visual.GetConverter(NamesPropertyValueConverterItem.Default);
        }
        protected PropertyValueConverter Visual { get; init; }

        public abstract IBlockComponent Convert(ConverterContext context);
        private RegexPropertyArray ArrayProperties { get; init; }
        protected RegexPropertyArray GetValueProperty(BlockIfChilds property) => GetValueProperty(property.ToString());
        protected RegexPropertyArray GetValueProperty(BlockProperty property) => GetValueProperty(property.ToString());
        /// <summary>
        /// Ищет в контексте массив свойств по имени
        /// </summary>
        /// <param name="property">Имя свойства</param>
        /// <returns>Массив найденных свойств</returns>
        protected RegexPropertyArray GetValueProperty(string property)
        {
            return ArrayProperties.GetProperties().Where((i) => i.Name == property).FirstOrDefault()
                as RegexPropertyArray ?? new RegexPropertyArray("", new List<IRegexProperty>());
        }

        /// <summary>
        /// Конвертирует массив свйоств в одно свойство
        /// </summary>
        /// <param name="array">Массиы свойств</param>
        /// <param name="converter">Конвертер массива свойств в свойство</param>
        /// <returns></returns>
        protected string ConvertRegexArrayToString(RegexPropertyArray array, PropertyValueConverter converter)
        {
            PropertyValueConverterParameters parameters = new(array);
            converter.PropertiesToString(parameters);
            return parameters.Result;
        }
        public int Start { get; init; }
        public int End { get; init; }
    }

    class BlockConverterLink : ConverterLinkToBlockBase
    {
        private IBlockComponent component;
        public BlockConverterLink(RegexPropertyArray blockProperties, IBlockComponent patternBlock, PropertyGenerateValueConverterTemlplate converterValue) : base(blockProperties, converterValue)
        {
            component = patternBlock;
        }

        public override IBlockComponent Convert(ConverterContext context)
        {
            IBlockComponent res = (IBlockComponent)component.Clone();
            res.Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.Content), Visual);
            return res;
        }
    }

    class MethodConvertLink : BlockConverterLink
    {
        public MethodConvertLink(RegexPropertyArray array, PropertyGenerateValueConverterTemlplate converterValue) : base(array, component, converterValue)
        {

            EndVisual = converterValue.GetConverter(NamesPropertyValueConverterItem.Output);
            NameVisual = converterValue.GetConverter(NamesPropertyValueConverterItem.Name);
        }
        private static BlockMethodComponent component = new();
        private PropertyValueConverter EndVisual { get; init; }
        private PropertyValueConverter NameVisual { get; init; }
        public override IBlockComponent Convert(ConverterContext context)
        {
            BlockMethodComponent res = (BlockMethodComponent)base.Convert(context);
            res.Input = ConvertRegexArrayToString(GetValueProperty(BlockProperty.BlockName), NameVisual);
            res.Output = ConvertRegexArrayToString(GetValueProperty(BlockProperty.EndBlockContent), EndVisual);
            return res;
        }
    }

    abstract class ConverterLinkWidthBody : ConverterLinkToBlockBase
    {
        public ConverterLinkWidthBody(RegexPropertyArray blockProperties, PropertyGenerateValueConverterTemlplate coverterValue) : base(blockProperties, coverterValue) { }
        protected static IBlockComponent ConvertBody(ConverterContext context, IRegexProperty property)
        {
            BlockLoggerManager.LogAsync(string.Format(Resources.ConverterLinkWidthBodyStartConvertBody, property.ToString()), LogLevels.Debug, Thread.CurrentThread);
            var converters = context.GetBodyProperty(property);
            BlockLoggerManager.LogAsync(Resources.ConverterLinkWidthBodyFindConvertersBody, LogLevels.Debug, Thread.CurrentThread);

            IBlockComponent res = null, current = res, temp;
            foreach (var item in converters)
            {
                BlockLoggerManager.LogAsync(string.Format(Resources.ConverterLinkWidthBodyFindConverterBody, item.ToString()), LogLevels.Debug, Thread.CurrentThread);
                if (current == null)
                    res = current = item.Convert(context);
                else
                {
                    temp = item.Convert(context);
                    current.Add(temp, new BlockComponentMetadata());
                    current = temp;
                }
            }
            return res;
        }
    }

    class FuncConvertLink : ConverterLinkWidthBody
    {
        public FuncConvertLink(RegexPropertyArray array, PropertyGenerateValueConverterTemlplate funcConverter, PropertyGenerateValueConverterTemlplate endBlockConverter) : base(array, funcConverter)
        {
            funcConverter.RemoveConverter(NamesPropertyValueConverterItem.Default);
            Visual = funcConverter.GetConverter(NamesPropertyValueConverterItem.Input);
            EndVisual = endBlockConverter.GetConverter(NamesPropertyValueConverterItem.Default);
            NameVisual = funcConverter.GetConverter(NamesPropertyValueConverterItem.Name);
        }
        private PropertyValueConverter EndVisual { get; init; }
        private PropertyValueConverter NameVisual { get; init; }
        public override BeginBlockComponent Convert(ConverterContext context)
        {
            BeginBlockComponent begin = new BeginBlockComponent()
            {
                Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.Content), Visual),
                Name = ConvertRegexArrayToString(GetValueProperty(BlockProperty.BlockName), NameVisual)
            };
            begin.Add(ConvertBody(context, GetValueProperty(BlockProperty.ChildBlocks)), new BlockComponentMetadata());
            IBlockComponent end = GetEndBlockComponent(begin);
            if (!(end is BlockEndComponent))
                end.Add(new BlockEndComponent() { Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.EndBlockContent), EndVisual) }, new BlockComponentMetadata());
            return begin;
        }
        private static IBlockComponent GetEndBlockComponent(IBlockComponent start)
        {
            IBlockComponent current = start, temp;
            while ((temp = current.GetComponent(new BlockComponentMetadata())) != null)
                current = temp;
            return current;
        }
    }

    class IfConvertLink : ConverterLinkWidthBody
    {
        public IfConvertLink(RegexPropertyArray array, PropertyGenerateValueConverterTemlplate converterValue) : base(array, converterValue) { }

        public override IBlockComponent Convert(ConverterContext context)
        {
            BlockIfComponent res = new() { Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.Content), Visual) };
            res.Add(ConvertBody(context, GetValueProperty(BlockIfChilds.TrueBlock)), new BlockIfComponentMetadata(BlockIfChilds.TrueBlock));
            res.Add(ConvertBody(context, GetValueProperty(BlockIfChilds.FalseBlock)), new BlockIfComponentMetadata(BlockIfChilds.FalseBlock));
            return res;
        }
    }

    class CycleConvertLink : ConverterLinkWidthBody
    {
        private IBlockComponent GenerateComponent { get; init; }
        public CycleConvertLink(IBlockComponent generateCycle, RegexPropertyArray array, PropertyGenerateValueConverterTemlplate converterValue) : base(array, converterValue)
        {
            if (!(generateCycle is BlockCycleComponent))
                throw new ArgumentException(generateCycle.ToString());
            GenerateComponent = generateCycle;
        }
        public override IBlockComponent Convert(ConverterContext context)
        {
            IBlockComponent res = (IBlockComponent)GenerateComponent.Clone();
            res.Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.Content), Visual);
            res.Add(ConvertBody(context, GetValueProperty(BlockProperty.ChildBlocks)), new BlockCycleComponentMetadata(BlockProperty.ChildBlocks));
            return res;
        }
    }

    class SwitchConverterLink : ConverterLinkWidthBody
    {
        public SwitchConverterLink(RegexPropertyArray array, PropertyGenerateValueConverterTemlplate converterValue) : base(array, converterValue) { }
        public override IBlockComponent Convert(ConverterContext context)
        {
            IBlockComponent res = new BlockSwitchComponent() { Content = ConvertRegexArrayToString(GetValueProperty(BlockProperty.Content), Visual) };
            var childs = GetValueProperty(BlockProperty.ChildBlocks).GetProperties();
            foreach (var item in childs)
            {
                string key = item.GetProperty("Key").GetProperties().FirstOrDefault().Name;
                res.Add(ConvertBody(context, item.GetProperty("Value")),
                    new BlockSwitchComponentMetadata(key));
            }
            return res;
        }
    }

    public static class IRegexPropertyExtension
    {
        public static IRegexProperty GetProperty(this IRegexProperty property, string name) => property.GetProperties().Where((i) => i.Name == name).FirstOrDefault();
    }
}
