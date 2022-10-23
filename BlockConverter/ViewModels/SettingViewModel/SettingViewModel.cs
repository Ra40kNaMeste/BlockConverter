using ModelConverterToBlock.Converters.ConvertToBlock;
using ModelConverterToBlock.Converters.Reader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;

namespace BlockConverter.ViewModels.SettingViewModel
{
    class SettingViewModel
    {
        public SettingViewModel(Window window)
        {
            FillPropertyConverters();
            StyleManager = new();
            StyleManager.OnCanReload += SetTrueReload;
            GeneralSettings = GeneralSettings.ReadSettings(window);
            GeneralSettings.OnCanReload += SetTrueReload;
        }

        private void SetTrueReload() => IsReload = true;

        private void FillPropertyConverters()
        {
            PropertyConverters = new();
            foreach (var item in ConverterSetting.PropertyConverters)
            {
                var converters = item.Value.GetConverters();
                int i = 1;
                foreach (PropertyValueConverterItem converter in converters)
                {
                    PropertyConverters.Add(new KeyPropertyConvertersDicionary(converter.Name, item.Key), new((PropertyEmptyValueConverter)converter.Converter.Clone()));
                    i++;
                }
            }
        }
        private bool IsReload { get; set; }
        private void AcceptGeneralSettings()
        {
            GeneralSettings.AcceptIsSave();
            GeneralSettings.SaveSettings();
            if (IsReload)
            {
                IsReload = false;
                LanguageAcceptShowDialog dialog = new();
                if (dialog.ShowDialog() == true)
                {
                    MainWindow window = new();
                    Window oldWindow = Application.Current.MainWindow;
                    window.Resources = oldWindow.Resources;
                    Application.Current.MainWindow = window;
                    window.Show();
                    oldWindow.Close();
                }
            }
        }

        public static void SetSettings()
        {
            BlockStyleManager.ReadStyles();
            GeneralSettings settings = GeneralSettings.ReadSettings();
            settings.Accept();
        }

        #region Command

        #region HeadCommand
        private CustomCommand acceptCommand;
        public CustomCommand Accept { get { return acceptCommand ??= new CustomCommand(AcceptCommand); } }
        #endregion//HeadCommand

        #region BodyCommand
        public void AcceptCommand(object parameter)
        {
            foreach (var item in PropertyConverters)
            {
                ConverterSetting.PropertyConverters[item.Key.Construction].AddConverter(new(item.Key.NameProperty, item.Value.Root));
            }

            ConverterSetting.SavePropertyConverters();
            StyleManager.Accept();
            AcceptGeneralSettings();

            if (parameter is Window win)
                win.Close();

        }
        #endregion//BodyCommand

        #endregion //Command

        #region VisualProperties

        public GeneralSettings GeneralSettings { get; set; }
        public Dictionary<KeyPropertyConvertersDicionary, VisualPropertyValueTreeCollection> PropertyConverters { get; set; }

        public BlockStyleManager StyleManager { get; init; }
        //public VisualPropertyValueTreeCollection SelectConverterCollection { get; set; }
        #endregion//VisualProperties
    }
}
