using ModelConverterToBlock.Converters.ConvertToBlock;
using ModelConverterToBlock.Converters.Reader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlockConverter.ViewModels.SettingViewModel
{
    class KeyPropertyConvertersDicionary
    {
        public KeyPropertyConvertersDicionary(NamesPropertyValueConverterItem nameProp, BlockConstruction construction)
        {
            NameProperty = nameProp;
            Construction = construction;
        }
        public NamesPropertyValueConverterItem NameProperty { get; set; }
        public BlockConstruction Construction { get; set; }
    }
    class VisualPropertyValueTreeCollection : PropertyValueTreeCollection, INotifyPropertyChanged
    {
        public VisualPropertyValueTreeCollection(PropertyEmptyValueConverter root) : base(root)
        {
            FillInputConverters();

            foreach (var item in this)
            {
                if (item is INotifyPropertyChanged p)
                    p.PropertyChanged += UpdateOutputValue;
            }
            Root.PropertyChanged += UpdateOutputValue;

            CollectionChanged += UpdateOutputValue;
            InputValues = new() { "a", "b", "c" };
            CollectionChanged += AddItemNotifyPropertyChanged;
            UpdateOutputValue(this, new EventArgs());
        }

        #region Command
        #region HeadCommand
        private CustomCommand addCommand;
        public CustomCommand AddCommand { get { return addCommand ??= new CustomCommand(AddCommandBody); } }

        private CustomCommand removeCommand;
        public CustomCommand RemoveCommand { get { return removeCommand ??= new CustomCommand(RemoveCommandBody); } }

        private CustomCommand insertCommand;
        public CustomCommand InsertCommand { get { return insertCommand ??= new CustomCommand(InsertCommandBody); } }
        #endregion//HeadCommand
        #region BodyCommand
        public void AddCommandBody(object parameter)
        {
            Add((PropertyValueConverter)InputConvertersDictionary[SelectInputConverter].Clone());
        }
        public void RemoveCommandBody(object parameter)
        {
            if (Select != null)
                Remove(Select);
        }
        public void InsertCommandBody(object parameter)
        {
            Insert(IndexOf(Select), (PropertyValueConverter)InputConvertersDictionary[SelectInputConverter].Clone());
        }
        //public bool CanSelectCommand(object parameter) => parameter is PropertyValueConverter;
        #endregion//BodyCommand
        #endregion //Command

        #region VisualProperties
        private PropertyValueConverter select;
        public PropertyValueConverter Select
        {
            get { return select; }
            set
            {
                select = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> inputValues;
        public ObservableCollection<string> InputValues
        {
            get { return inputValues; }
            set
            {
                inputValues = value;
                OnPropertyChanged();
            }
        }
        private string outputValue;
        public string OutputValue
        {
            get { return outputValue; }
            private set
            {
                outputValue = value;
                OnPropertyChanged();
            }
        }

        private string outputByZeroValue;
        public string OutputByZeroValue
        {
            get { return outputByZeroValue; }
            private set
            {
                outputByZeroValue = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, PropertyValueConverter> InputConvertersDictionary { get; set; }
        public List<string> InputConverters { get; set; }
        public string SelectInputConverter { get; set; }

        #endregion //VisualProperties

        private void AddItemNotifyPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged p)
                        p.PropertyChanged += UpdateOutputValue;
                }
            }
        }
        private void UpdateOutputValue(object sender, EventArgs e)
        {
            List<IRegexProperty> inputs = InputValues.Select((i) => (IRegexProperty)new RegexPropertyValue(i, -1)).ToList();
            RegexPropertyArray array = new("", inputs);
            PropertyValueConverterParameters param = new(array);
            Root.PropertiesToString(param);
            OutputValue = param.Result;

            param = new(new RegexPropertyArray("", new List<IRegexProperty>()));
            Root.PropertiesToString(param);
            OutputByZeroValue = param.Result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void FillInputConverters()
        {
            InputConvertersDictionary = new()
            {
                { Resources.Resources.UnionPropertyValueConverterName, new PropertyUnionValueConverter() },
                { Resources.Resources.CleanEscapePropertyValueConverterName, new PropertyCleanExcapeSymbolsValueConverter() },
                { Resources.Resources.AppendWordPropertyValueConverter, new PropertyAppendWordValueConverter() },
                { Resources.Resources.ActionIfEmptyStringPropertyValueConverter, new PropertyIfNotValueConverter() }
            };
            InputConverters = InputConvertersDictionary.Keys.ToList();
        }
    }

}
