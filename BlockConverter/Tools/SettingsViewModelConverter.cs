using BlockConverter.ViewModels;
using BlockConverter.ViewModels.SettingViewModel;
using ModelConverterToBlock.Converters.ConvertToBlock;
using ModelConverterToBlock.Converters.Reader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BlockConverter.Tools
{
    //Набор конвертеров для окна настроек
    class StringToPropertyConverter : IValueConverter
    {
        static StringToPropertyConverter() => ConverterToTypeDictionary = PropertyToStringVisualNameConverter.GetConverterToStringDictionary
            .ToDictionary((i) => i.Value, (i) => i.Key);
        internal static Dictionary<string, Type> ConverterToTypeDictionary = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            Type res;
            return ConverterToTypeDictionary.TryGetValue(((ListBoxItem)value).Content.ToString(), out res) ?
                res.GetConstructor(new Type[] { }).Invoke(new object[] { }) : throw new ArgumentException("Не найденно определения типа");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class KeyToStringVisualWordConverter : IValueConverter
    {
        private Dictionary<BlockConstruction, string> constructions = new()
        {
            { BlockConstruction.Cycle, Resources.Resources.BlockConstructionCycle },
            { BlockConstruction.ForCycle, Resources.Resources.BlockConstructionForCycle },
            { BlockConstruction.Function, Resources.Resources.BlockConstructionFunction },
            { BlockConstruction.If, Resources.Resources.BlockConstructionIf },
            { BlockConstruction.Input, Resources.Resources.BlockConstructionInput },
            { BlockConstruction.Method, Resources.Resources.BlockConstructionMethod },
            { BlockConstruction.Operation, Resources.Resources.BlockConstructionOperation },
            { BlockConstruction.Output, Resources.Resources.BlockConstructionOutput },
            { BlockConstruction.PostCycle, Resources.Resources.BlockConstructionPostCycle },
            { BlockConstruction.Switch, Resources.Resources.BlockConstructionSwitch },
            { BlockConstruction.Return, Resources.Resources.BlockConstructionReturn}
        };
        private Dictionary<NamesPropertyValueConverterItem, string> constructionsNumber = new()
        {
            { NamesPropertyValueConverterItem.Default , Resources.Resources.BlockConstructionPropertyBody },
            { NamesPropertyValueConverterItem.Input, Resources.Resources.BlockConstructionPropertyInput },
            { NamesPropertyValueConverterItem.Output, Resources.Resources.BlockConstructionPropertyOutput },
            { NamesPropertyValueConverterItem.Name, Resources.Resources.BlockConstructionPropertyName }
        };
        private Dictionary<NamesPropertyValueConverterItem, string> ConstructionsNumber => constructionsNumber;
        private Dictionary<BlockConstruction, string> Constructions => constructions;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is KeyPropertyConvertersDicionary res)
                return Constructions[res.Construction] + ConstructionsNumber[res.NameProperty];
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BlockNameToStringVisualWordConverter : IValueConverter
    {
        private Dictionary<VisualBlockTypes, string> constructions = new()
        {
            { VisualBlockTypes.Cycle, Resources.Resources.BlockConstructionCycle },
            { VisualBlockTypes.For, Resources.Resources.BlockConstructionForCycle },
            { VisualBlockTypes.Begin, Resources.Resources.BlockConstructionBegin },
            { VisualBlockTypes.End, Resources.Resources.BlockConstructionEnd },
            { VisualBlockTypes.If, Resources.Resources.BlockConstructionIf },
            { VisualBlockTypes.Input, Resources.Resources.BlockConstructionInput },
            { VisualBlockTypes.Method, Resources.Resources.BlockConstructionMethod },
            { VisualBlockTypes.Operation, Resources.Resources.BlockConstructionOperation },
            { VisualBlockTypes.Output, Resources.Resources.BlockConstructionOutput },
            { VisualBlockTypes.PostCycle, Resources.Resources.BlockConstructionPostCycle },
            { VisualBlockTypes.Switch, Resources.Resources.BlockConstructionSwitch }
        };
        public Dictionary<VisualBlockTypes, string> Constructions => constructions;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VisualBlockTypes res)
                return Constructions[res];
            throw new ArgumentException();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class VisualPropertiesConverter : IValueConverter
    {
        private static IValueConverter directionConverter = new DirectionConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<UIElement> res = new();
            if (value is PropertyValueConverter)
            {
                string propName;
                if (!PropertyToStringVisualNameConverter.GetConverterToStringDictionary.TryGetValue(value.GetType(), out propName))
                    throw new ArgumentException("Не найденно определения типа");
                res.Add(new TextBlock() { Text = propName });
            }
            if (value is IWordPropertyValueConverter)
            {
                res.Add(new TextBlock() { Text = Resources.Resources.VisualPropertiesWordName });
                TextBox wordText = new();
                Binding binding = new("Word");
                binding.Source = value;
                wordText.SetBinding(TextBox.TextProperty, binding);
                res.Add(wordText);
            }

            if (value is PropertyAppendWordValueConverter)
            {
                res.Add(new TextBlock() { Text = Resources.Resources.VisualPropertiesDirectionForAppendWord });
                ComboBox box = new();
                //ComboBoxItem leftItem = new();
                //BindingOperations.SetBinding(leftItem, ComboBoxItem.ContentProperty, new Binding())
                box.Items.Add(Resources.Resources.VisualPropertiesLeftDirection);
                box.Items.Add(Resources.Resources.VisualPropertiesRightDirection);
                Binding binding = new("Direction");
                binding.Source = value;
                binding.Converter = directionConverter;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Mode = BindingMode.TwoWay;
                box.SetBinding(Selector.SelectedItemProperty, binding);
                res.Add(box);
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class PropertyToStringVisualNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res;
            return GetConverterToStringDictionary.TryGetValue(value.GetType(), out res) ?
                res : throw new ArgumentException("Не найденно определения типа");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        internal static Dictionary<Type, string> GetConverterToStringDictionary => new()
        {
            { typeof(PropertyUnionValueConverter), Resources.Resources.UnionPropertyValueConverterName },
            { typeof(PropertyCleanExcapeSymbolsValueConverter), Resources.Resources.CleanEscapePropertyValueConverterName },
            { typeof(PropertyAppendWordValueConverter), Resources.Resources.AppendWordPropertyValueConverter },
            { typeof(PropertyIfNotValueConverter), Resources.Resources.ActionIfEmptyStringPropertyValueConverter }
        };
    }

    public class DirectionConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString();
            if (val == Resources.Resources.VisualPropertiesLeftDirection)
                return Direction.Left;
            if (val == Resources.Resources.VisualPropertiesRightDirection)
                return Direction.Right;
            throw new ArgumentException(val);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Direction)value switch
        {
            Direction.Left => Resources.Resources.VisualPropertiesLeftDirection,
            Direction.Right => Resources.Resources.VisualPropertiesRightDirection,
            _ => throw new ArgumentException(value.ToString())
        };

    }
    public class StyleNameConverter : IValueConverter
    {
        private Dictionary<string, string> StyleNameResources = new()
        {
            { "Default", Resources.Resources.DefaultStyleName },
            { "Style", Resources.Resources.CustomStyleName }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] temp = value.ToString().Split('_');
            if (temp.Length == 0)
                throw new ArgumentException(value.ToString());
            string res;
            if (StyleNameResources.TryGetValue(temp[0], out res))
            {
                if (temp.Length > 1)
                    return res + temp[1];
                return res;
            }
            throw new ArgumentException(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
