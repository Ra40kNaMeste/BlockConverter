using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConverterToBlock.Converters.Reader;
using System.Windows;
using System.Threading;
using System.IO;
using BlocksLib.Blocks.CustomControls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlockConverter;

namespace BlockConverter.ViewModels.SettingViewModel
{
    //Генеральные настройки: язык, тема и т.д.
    class GeneralSettings:IGeneralSettingThreeItem
    {
        #region Constructions
        private GeneralSettings(Window settingWindow)
        {
            IsReload = false;
            SettingWindow = settingWindow;
            Settings = new();
            CheckSettings();
            SettingsForSave = Settings.Values.ToList();
        }

        #endregion //Constructions

        #region VisualProperties

        private Action onCanReload;
        public event Action OnCanReload { add { onCanReload += value; } remove { onCanReload -= value; } }

        public Dictionary<string, GeneralSettingsItem> Settings { get; set; }

        #endregion//VisualProperties
        private Window SettingWindow { get; init; }
        private List<GeneralSettingsItem> SettingsForSave { get; set; }
        private static Dictionary<string, Dictionary<string, Type>> PatternSetters =>
            new()
            {
                {
                    Resources.Resources.BasicGeneralSettings,
                    new()
                    {
                        { Resources.Resources.GeneralSettingsTheme, typeof(ThemeSetting) },
                        { Resources.Resources.GeneralSettingsLanguage, typeof(LanguageSetting) }
                    }
                }
            };
        private void CheckSettings()
        {
            Settings = PatternSetters.ToDictionary(i => i.Key, j => (Settings.ContainsKey(j.Key) ? Settings[j.Key] : GetBoundSettingsItem())
                        .LoadSettingsPattern(j.Value, SettingWindow));
        }
        private GeneralSettingsItem GetBoundSettingsItem()
        {
            GeneralSettingsItem res = new();
            res.RequestReloadAppEvent += RequestReloadAppItem;
            return res;
        }

        private void RequestReloadAppItem(object sender, EventArgs e) => IsReload = true;

        public void SaveSettings()
        {
            BlockConverter.Settings.Default.Save();
        }
        private bool IsReload { get; set; }

        public FrameworkElement ShowValue => throw new NotImplementedException();

        public void Accept()
        {
            foreach (var setting in SettingsForSave)
                setting.Accept();
            IsReload = false;
            
        }
        public static GeneralSettings ReadSettings() => ReadSettings(null);
        public static GeneralSettings ReadSettings(Window settingWindow)
        {
            return new(settingWindow);
        }

        public void AcceptIsSave()
        {
            foreach (var setting in SettingsForSave)
                setting.AcceptIsSave();
            BlockConverter.Settings.Default.Save();
            if (IsReload)
                onCanReload?.Invoke();
        }
    }

    class GeneralSettingsItem:IGeneralSettingThreeItem, INotifyRequestReloadApp
    {
        public GeneralSettingsItem()
        {
            Settings = new();
            VisualSettings = new();
        }
        public Dictionary<string, IGeneralSettingThreeItem> VisualSettings { get; set; }
        public List<IGeneralSettingThreeItem> Settings { get; set; }

        public FrameworkElement ShowValue => throw new NotImplementedException();

        public event RequestReloadAppHandler RequestReloadAppEvent;

        public GeneralSettingsItem LoadSettingsPattern(Dictionary<string, Type> settings, Window settingWindow)
        {
            VisualSettings = settings
                .Select(i => new { Key = i.Key, Value = Settings.FirstOrDefault(j => j.GetType() == i.Value) ?? ConsructSetting(i.Value) })
                .ToDictionary(i => i.Key, i => i.Value);
            Settings = VisualSettings.Values.ToList();
            return this;
        }
        public void Accept()
        {
            foreach (var item in Settings)
                item.Accept();
        }
        private IGeneralSettingThreeItem ConsructSetting(Type type)
        {
            object res = type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
            if (res is IGeneralSettingThreeItem setting)
            {
                if (res is INotifyRequestReloadApp temp)
                    temp.RequestReloadAppEvent += (object sender, EventArgs e) => 
                    RequestReloadAppEvent?.Invoke(this, e);
                return setting;
            }
            throw new ArgumentException(type.FullName);
        }

        public void AcceptIsSave()
        {
            foreach (var item in Settings)
                item.AcceptIsSave();
        }
    }

    delegate void RequestReloadAppHandler(object sender, EventArgs e);
    interface INotifyRequestReloadApp
    {
        public event RequestReloadAppHandler RequestReloadAppEvent;
    }
    interface IGeneralSettingThreeItem
    {
        public FrameworkElement ShowValue { get; }
        public void Accept();
        public void AcceptIsSave();
    }

    public abstract class GeneralSetting : IGeneralSettingThreeItem, INotifyPropertyChanged
    {
        public FrameworkElement ShowValue { get; protected set; }

        private bool isSetValue = false;

        private string value;

        public string Value
        {
            get => value;
            set
            {
                if (!isSetValue)
                {
                    oldValue = value;
                    isSetValue = true;
                }
                this.value = value;
                OnPropertyChanged();
            }
        }

        protected string oldValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Accept();

        public virtual bool IsSave() => oldValue != Value;
        protected void OnPropertyChanged([CallerMemberName] string prop = null) => PropertyChanged?.Invoke(this, new(prop));

        public void AcceptIsSave()
        {
            if (IsSave())
                Accept();
        }
    }

    public abstract class StringGeneralSetting : GeneralSetting
    {
        public StringGeneralSetting()
        {
            ShowValue = new TextBox();
            Binding binding = new("Value");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            BindingOperations.SetBinding(ShowValue, TextBox.TextProperty, binding);
        }
    }

    public abstract class ComboBoxGeneralSetting : GeneralSetting
    {
        public ComboBoxGeneralSetting()
        {
            ShowValue = new ComboBox();
            SetBinding(Selector.SelectedItemProperty, "Value");
            SetBinding(Selector.ItemsSourceProperty, "Values");
        }
        private void SetBinding(DependencyProperty property, string path)
        {
            Binding binding = new(path);
            binding.Source = this;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(ShowValue, property, binding);
        }

        private IEnumerable<string> values;
        public IEnumerable<string> Values
        {
            get => values;
            protected set
            {
                values = value;
                OnPropertyChanged();
            }
        }
    }

    public abstract class ComboBoxByDictionaryGeneralSettings<T> : ComboBoxGeneralSetting
    {
        protected abstract string Default { get; }
        public ComboBoxByDictionaryGeneralSettings(T defaultKey)
        {
            Values = DictionaryValues.Values;
            Value = DictionaryValues.Keys.Contains(defaultKey) ? DictionaryValues[defaultKey] : DictionaryValues.First().Value;
        }
        protected abstract Dictionary<T, string> DictionaryValues { get; }

        protected T GetSelectItemKey() => DictionaryValues.Where(i => i.Value == Value).First().Key;
    }

    class ThemeSetting : ComboBoxByDictionaryGeneralSettings<string>
    {
        private const string pathDefaultAppTheme = @"LightTheme";

        private static ResourceDictionary oldResources;

        public ThemeSetting() : base(Settings.Default.Theme)
        {
        }

        public override void Accept()
        {
            Application.Current.Resources.MergedDictionaries.Remove(oldResources);
            string pathSelectTheme = GetSelectItemKey();
            Settings.Default.Theme = pathSelectTheme;
            SetTheme(pathSelectTheme);
        }

        protected override Dictionary<string, string> DictionaryValues => appThemes;

        protected override string Default => pathDefaultAppTheme;

        private Dictionary<string, string> appThemes = new()
        {
            { pathDefaultAppTheme, Resources.Resources.LightThemeName },
            //{ "DarkTheme", Resources.Resources.DarkThemeName }
        };

        private const string pathAppThemes = @"Themes\";


        private void SetTheme(string selectTheme)
        {
            ResourceDictionary newTheme = new()
            {
                Source = new Uri(pathAppThemes + selectTheme + ".xaml", UriKind.Relative)
            };
            oldResources = newTheme;
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }
    }
    class LanguageSetting : ComboBoxByDictionaryGeneralSettings<string>, INotifyRequestReloadApp
    {
        public LanguageSetting() : base(Settings.Default.Language)
        {
            if (Settings.Default.IsFirstLoad)
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                Value = DictionaryValues.Keys.Contains(language) ? DictionaryValues[language] : DictionaryValues.First().Value;
                Settings.Default.IsFirstLoad = false;
                Settings.Default.Language = language;
                Settings.Default.Save();
            }
        }

        protected override Dictionary<string, string> DictionaryValues => language;

        protected override string Default => "en-US";

        private Dictionary<string, string> language = new()
        {
            { "en-US", Resources.Resources.enUSLanguage },
            { "ru-RU", Resources.Resources.ruRULanguage }
        };

        public event RequestReloadAppHandler RequestReloadAppEvent;

        public override void Accept()
        {
            string language = GetSelectItemKey();
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(language);
            Settings.Default.Language = language;

            RequestReloadAppEvent?.Invoke(this, new());
        }
    }
}
