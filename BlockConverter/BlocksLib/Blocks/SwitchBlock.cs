using BlocksLib.Blocks.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BlocksLib.Blocks
{
    public class SwitchBlock : MultiChildBlockBase, INotifyPropertyChanged
    {
        static SwitchBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchBlock), new FrameworkPropertyMetadata(typeof(SwitchBlock)));
            ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<SwitchBlockItem>), typeof(SwitchBlock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        }
        public SwitchBlock()
        {
            Items = new();
            Items.CollectionChanged += Items_CollectionChanged;

            PropertiesForGetSelect.Add(() => IsSelectItem);
            PropertiesForSetSelect.Add(i =>
            {
                foreach (var item in Items)
                    item.IsSelect = true;
            });

            PropertyForGetByName.Add(name =>
            {
                var temp = Items.Where(i => i.Key == name).FirstOrDefault();
                return temp != null && temp.IsSelect;
            });
            PropertyForSetByName.Add((name, val) =>
            {
                var temp = Items.Where(i => i.Key == name).FirstOrDefault();
                if (temp == null)
                    return false;
                temp.IsSelect = val;
                return true;
            });
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SwitchBlockItem item in e.NewItems)
                {
                    SetParentBlock(item.Value, this);
                }
            }
        }

        public static readonly DependencyPropertyKey ItemsPropertyKey;
        public ObservableCollection<SwitchBlockItem> Items
        {
            get { return (ObservableCollection<SwitchBlockItem>)GetValue(ItemsPropertyKey.DependencyProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        public override BlockSelectPropertyMetadata FindPanel(BlockPanel panel)
        {
            foreach (var item in Items)
            {
                if (item.Value == panel)
                    return new BlockSelectPropertyMetadata(BlockTypes.Switch, item.Key, panel);
            }
            return base.FindPanel(panel);
        }

        public override BlockPanel GetPanel(BlockSelectPropertyMetadata property)
        {
            if (property.Type == BlockTypes.Switch)
            {
                string propertyName = property.Property;
                return Items.Where(i => i.Key == propertyName).FirstOrDefault().Value;
            }
            return base.GetPanel(property);
        }
        public override List<BlockSelectPropertyMetadata> GetSelectBlockPropertys()
        {
            var res = base.GetSelectBlockPropertys();
            foreach (var item in Items)
            {
                if (item.IsSelect)
                    res.Add(new BlockSelectPropertyMetadata(BlockTypes.Switch, item.Key, item.Value));
            }
            return res;
        }

        private CustomCommand addItem;
        public CustomCommand AddItem { get { return addItem ??= new(AddItemCommand, CanAddItemCommand); } }
        private void AddItemCommand(object obj)
        {
            BlockPanel panel = new();
            panel.Children.Add(new BlockEmpty());
            SwitchBlockItem item = new();
            item.Value = panel;
            item.Key = obj == null ? "" : obj.ToString();
            item.ItemsBlock = Items;
            item.SelectChanged += Item_SelectChanged;
            PropertiesForGetSelect.Add(() => item.IsSelect);
            PropertiesForSetSelect.Add(i => item.IsSelect = i);
            Items.Add(item);
        }

        private bool CanAddItemCommand(object obj)
        {
            string key = obj == null ? "" : obj.ToString();
            return Items.FirstOrDefault((i) => i.Key == key) == null;
        }

        public bool IsSelectItem { get; private set; }
        private void Item_SelectChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
                IsSelectItem = true;
            else
                IsSelectItem = Items.Any(i => i.IsSelect);

            SwitchBlockItem item = (SwitchBlockItem)sender;
            SelectBlockChanged(this, new(BlockTypes.Switch, item.Key, null), e.OldValue, e.NewValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SwitchBlockItem : DependencyObject, INotifyPropertyChanged, IHorizontalCenter, INotifyDataErrorInfo
    {
        public IEnumerable<SwitchBlockItem> ItemsBlock { get; set; }
        public SwitchBlockItem()
        {
            Value = new();
        }

        public static DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(SwitchBlockItem), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, KeyChangedCallback));

        private static void KeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwitchBlockItem blockItem = (SwitchBlockItem)d;
            foreach (var item in blockItem.ItemsBlock)
            {
                item.OnNotifyPropertyChanged("Key");
                item.UpdateValidate();
            }
        }
        public string Key
        {
            get => (string)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        public static DependencyProperty IsSelectProperty = DependencyProperty.Register("IsSelect", typeof(bool), typeof(SwitchBlockItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, SelectBlockChanged));

        private static void SelectBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwitchBlockItem block = (SwitchBlockItem)d;
            block.OnSelectChanged(block, new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue));
            block.OnNotifyPropertyChanged("Key");
        }
        private void OnSelectChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            selectChanged?.Invoke(sender, e);
        }
        public bool IsSelect
        {
            get => (bool)GetValue(IsSelectProperty);
            set => SetValue(IsSelectProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(BlockPanel), typeof(SwitchBlockItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, ValueChangedCallback));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwitchBlockItem item = (SwitchBlockItem)d;
            Binding binding = new("HorizontalCenter");
            binding.Source = e.NewValue;
            BindingOperations.SetBinding(item, HorizontalCenterProperty, binding);
        }

        public BlockPanel Value
        {
            get { return (BlockPanel)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        private CustomCommand selectItem;

        private event RoutedPropertyChangedEventHandler<bool> selectChanged;

        public event RoutedPropertyChangedEventHandler<bool> SelectChanged
        {
            add { selectChanged += value; }
            remove { selectChanged -= value; }
        }

        public CustomCommand SelectItem { get { return selectItem ??= new CustomCommand((object obj) => { IsSelect = !IsSelect; }); } }

        public static DependencyProperty HorizontalCenterProperty = DependencyProperty.Register("HorizontalCenter", typeof(double), typeof(SwitchBlockItem), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, HorizontalCenterChangedUpdate));

        private static void HorizontalCenterChangedUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwitchBlockItem item = (SwitchBlockItem)d;
            item.OnNotifyPropertyChanged("HorizontalCenter");
        }

        public double HorizontalCenter
        {
            get { return (double)GetValue(HorizontalCenterProperty); }
            set { SetValue(HorizontalCenterProperty, value); }
        }


        public void UpdateValidate()
        {
            if (ItemsBlock != null && ItemsBlock.Where(i => i.Key == Key).Count() > 1)
                AddError("Key", Resources.Resources.SwitchBlockSameKeysError);
            else
                RemoveErrors("Key");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new(name));

        protected void AddError(string property, string error)
        {
            if (errors.ContainsKey(property))
                errors[property] = error;
            else
                errors.Add(property, error);
            OnErrorsChanged(property);
        }
        protected void RemoveErrors(string property)
        {
            errors.Remove(property);
            OnErrorsChanged(property);
        }

        private Dictionary<string, string> errors = new();

        public bool HasErrors => errors.Any();

        protected void OnErrorsChanged(string property)
        {
            ErrorsChanged?.Invoke(this, new(property));
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
        }
    }
}
