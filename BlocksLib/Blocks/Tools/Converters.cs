using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Collections.ObjectModel;

namespace BlocksLib.Blocks.Tools
{
    internal static class CultureManager
    {
        private static CultureInfo culture;
        public static void SaveCulture(CultureInfo newCulture)
        {
            if (culture == null)
                culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
        }
        public static void SetOldCulture()
        {
            Thread.CurrentThread.CurrentCulture = culture;
            culture = null;
        }

    }

    public class ConverterMultiply : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CultureManager.SaveCulture(culture);
            double res = (double)value * System.Convert.ToDouble(parameter);
            CultureManager.SetOldCulture();
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / System.Convert.ToDouble(parameter);
        }
    }

    public class ConverterSumm : IValueConverter, IMultiValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CultureManager.SaveCulture(culture);
            double res = (double)value + System.Convert.ToDouble(parameter);
            CultureManager.SetOldCulture();
            return res;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double res = 0;
            foreach (var item in values)
                if (item is double)
                    res += (double)item;
            return res;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - System.Convert.ToDouble(parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterThicnessToHeight : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Thickness thickness = (Thickness)value;
            double res = thickness.Top + thickness.Bottom;
            if (parameter is double par)
                res *= par;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterThicnessToWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Thickness thickness = (Thickness)value;
            double res = thickness.Left + thickness.Right;
            if (parameter is double par)
                res *= par;
            return res;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterDoubleToThicness : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double res)
                return new Thickness(res, 0, res, 0);
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertDoubleToRhombus : IMultiValueConverter, IValueConverter
    {
        //public IValueConverter[] ConvertersToDouble { get; set; }
        //public int[] ConvertersNumberGroup { get; set; }
        /// <summary>
        /// конвертация в прямоугольник для блока условий с какой-то математческой магией
        /// </summary>
        /// <param name="values">параментры. высота длина и отступ </param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Если есть, то будет отвечать за колличество элементов подмассивов</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3)
                throw new ArgumentException();
            double height = (double)values[0];
            double width = (double)values[1];
            double padding = (double)values[2];
            double res = width / Math.Sqrt(3) + height + 4 * padding / Math.Sqrt(3); ;
            return res;

        }
        /// <summary>
        /// конвертация в квадрат типа Size
        /// </summary>
        /// <param name="value"></param>
        /// один параметр - длина и ширина
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Rect(new Point(0, 0), (Size)value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterMax : IMultiValueConverter
    {
        /// <summary>
        /// Сравнивает по хэш-коду. Параметр - число элементов в каждой группе
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CultureManager.SaveCulture(culture);
            double separator = System.Convert.ToDouble(parameter);
            CultureManager.SetOldCulture();
            double max;
            double current;
            max = current = 0;
            int size = values.Length;
            for (int i = 0; i < size; i++)
            {
                if (!(values[i] is double))
                    continue;
                current += (double)values[i];
                if (i == separator)
                {
                    max = Math.Max(current, max);
                    current = 0;
                }
            }
            max = Math.Max(current, max);
            return max;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterSwitchThicknessToWidth : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness th = (Thickness)values[0];
            double aw = (double)values[1];
            return aw - th.Left - th.Right;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MethodPropertiesToTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                string res = "";
                if (values[0].ToString() != "")
                    res += values[0] + "=";
                return string.Format("{0}{1}({2})", res, values[1], values[2]);
            }
            throw new ArgumentException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string val = value.ToString();
            int index1 = val.IndexOf('=');
            int index2 = val.IndexOf('(');
            if (index2 < 0 || index2 == val.Length - 1)
                return new object[]
                {
                    "", val, ""
                };
            if (index1 < 0)
                return new object[]
                {
                    "",
                    val.Substring(0, index2),
                    val.Substring(index2 + 1, val.Length - 2 - index2)
                };
            return new object[]
            {
                val.Substring(0, index1),
                val.Substring(index1 + 1, index2 - index1 - 1),
                val.Substring(index2 + 1, val.Length - 2 - index2)
            };
        }
    }

}