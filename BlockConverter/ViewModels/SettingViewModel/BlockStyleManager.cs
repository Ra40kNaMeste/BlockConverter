using BlocksLib.Blocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlockConverter.ViewModels.SettingViewModel
{

    public class BlockStyleManager
    {

        #region Constructor
        public BlockStyleManager()
        {
            ReadCurrentStyles();
            BlocksStyles = ReadFillSyles();
            foreach (var item in BlocksStyles)
            {
                item.Value.SelectStyle = FindStyles(item.Value.Styles, SelectStyles[item.Key]);
            }
        }

        #endregion//Constructor

        //Библиотека стилей: ключ - неполное название, значение - список стилей
        public Dictionary<VisualBlockTypes, BlockListStyle> BlocksStyles { get; init; }

        private Action onCanReload;
        public event Action OnCanReload { add { onCanReload += value; } remove { onCanReload -= value; } }

        #region Paths
        //Путь для сохранения имён шаблонов
        private const string saveStylesPath = "Setting/CurrentStyles.txt";

        //Возвращает путь библиотеки стилей для выбранного блока
        private static Uri GetUriByKey(string key) => new Uri("pack://application:,,,/BlocksLib;component/Themes/CustomTheme/" + key + "Styles.xaml");

        #endregion//Paths

        #region Tools

        /// <summary>
        /// Возвращает библиотеку всех возможных стилей
        /// </summary>
        /// <returns></returns>
        private static Dictionary<VisualBlockTypes, BlockListStyle> ReadFillSyles()
        {
            Dictionary<VisualBlockTypes, BlockListStyle> res = new();
            foreach (var item in ParametersAllBlocksManager.FactoryStyles)
            {
                //Создание элемента
                BlockListStyle listStyle = new(GetUriByKey(item.BlockType.ToString()), item);
                //Выделенный объект - первый в коллекции
                listStyle.SelectStyle = listStyle.Styles.FirstOrDefault();

                res.Add(item.BlockType, listStyle);
            }
            return res;
        }


        //ищет список стилей из коллекции по имени блока
        private static BlockListStyleItem FindStyles(IEnumerable<BlockListStyleItem> styles, string name)
        {
            return styles.Where(i => i.Name == name).FirstOrDefault();
        }

        //устанавливает стиль в ресурсы приложения по ключу
        private static void SetStyle(string key, Style style)
        {
            Application.Current.Resources.Remove(key);
            Application.Current.Resources.Add(key, style);
        }

        #endregion//Tools

        /// <summary>
        /// Применяет настройки
        /// </summary>
        public void Accept()
        {
            Dictionary<VisualBlockTypes, string> stylesForWrite = new();
            foreach (var item in BlocksStyles)
            {
                string keyStyle = ParametersAllBlocksManager.ConvertNameBlockToNameStyle(item.Key);
                SetStyle(keyStyle, item.Value.SelectStyle.Block.Style);
                stylesForWrite.Add(item.Key, item.Value.SelectStyle.Name);
            }
            if (BlocksStyles.Count != 0)
                onCanReload?.Invoke();
            WriteCurrentStyles(stylesForWrite);
        }

        /// <summary>
        /// Чтение и применение стилей блоков
        /// </summary>
        /// <returns></returns>
        protected internal static bool ReadStyles()
        {
            try
            {
                var styles = ReadFillSyles();
                var temp = styles.GroupJoin(SelectStyles, s => s.Key, sS => sS.Key, (s, sS) =>
                    new { Key = s.Key, ListStyleItem = s.Value.Styles.Where(i => i.Name == sS.FirstOrDefault().Value)
                    .FirstOrDefault() })
                    .Where((i) => i.ListStyleItem.Name != BlockListStyle.DefaultName)
                    .Select(i => new { Key = i.Key, Style = i.ListStyleItem.Block.Style });
                foreach (var item in temp)
                    SetStyle(ParametersAllBlocksManager.ConvertNameBlockToNameStyle(item.Key), item.Style);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region SelectStyles

        //Список установленных стилей. Ключ - имя блока, значение - имя стиля

        private static Dictionary<VisualBlockTypes, string> selectStyles = ReadCurrentStyles();

        private static Dictionary<VisualBlockTypes, string> SelectStyles => selectStyles;

        //Выполняет чтение из файла установленных стилей
        private static Dictionary<VisualBlockTypes, string> ReadCurrentStyles()
        {
            Dictionary<VisualBlockTypes, string> res = new();
            try
            {
                using (StreamReader sr = new(saveStylesPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] strs = line.Split(' ');
                        res.Add(ParametersAllBlocksManager.GetVisualBlockTypesByString(strs[0]), strs[1]);
                    }
                }

            }
            catch (Exception)
            {
                res.Clear();
                foreach (var item in ParametersAllBlocksManager.FactoryStyles)
                {
                    res.Add(item.BlockType, BlockListStyle.DefaultName);
                }
            }
            return res;
        }

        //Записывает в файл установленные стили
        private static void WriteCurrentStyles(Dictionary<VisualBlockTypes, string> styles)
        {
            try
            {
                using (StreamWriter sw = new(saveStylesPath, false))
                {
                    foreach (var item in styles)
                    {
                        sw.WriteLine(item.Key.ToString() + ' ' + item.Value);
                    }
                }
            }
            catch (Exception)
            {

                throw new InvalidOperationException();
            }
        }
        #endregion//SelectStyles
    }

    public class BlockListStyle
    {
        #region VisualProperties
        public ObservableCollection<BlockListStyleItem> Styles { get; init; }
        public BlockListStyleItem SelectStyle { get; set; }

        #endregion//VisualProperties


        #region Constructions

        internal BlockListStyle(Uri path, FactoryParameters parameters)
        {
            Styles = new();

            BlockBase CreateBlock(Style style)
            {

                BlockBase res = parameters.GetPrototype();
                res.Style = style;
                return res;
            }
            //Создание стиля по умолчанию
            BlockListStyleItem def = new(DefaultName, parameters.GetPrototype());
            Styles.Add(def);

            try
            {
                //Чтение стилей из библиотеки
                ResourceDictionary dic = new() { Source = path };
                Dictionary<string, Style> customDic = FindResourceDictionary(dic, parameters.BlockPrototype.GetType());
                foreach (var key in customDic.Keys)
                {
                    Styles.Add(new BlockListStyleItem(key, CreateBlock(customDic[key])));
                }
            }
            catch (Exception) { }
        }
        #endregion//Constructions

        #region Tools

        //Имя стиля по умолчанию
        protected internal const string DefaultName = "Default";

        //Ищет стили из библиотеки для типа блока
        private static Dictionary<string, Style> FindResourceDictionary(ResourceDictionary dic, Type targetType)
        {
            Dictionary<string, Style> res = new();
            foreach (var item in dic.Keys)
                if ((dic[item] is Style style) && style.TargetType == targetType)
                    res.Add(item.ToString(), style);
            return res;
        }

        #endregion//Tools
    }

    public class BlockListStyleItem
    {
        public BlockListStyleItem() { }
        public BlockListStyleItem(string name, BlockBase block)
        {
            Name = name;
            Block = block;
        }
        public BlockBase Block { get; set; }
        public string Name { get; set; }
    }
}
