using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;

namespace BlocksLib.Blocks
{
    interface IHorizontalCenter
    {
        public double HorizontalCenter { get; }
    }

    [TemplatePart(Name = "rootBlock", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "setHC", Type = typeof(FrameworkElement))]
    public abstract class BlockBase : Control, IHorizontalCenter, ICloneable
    {
        #region Constructors
        static BlockBase()
        {
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BlockBase), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            HorizontalCenterProperty = DependencyProperty.Register("HorizontalCenter", typeof(double), typeof(BlockBase), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.NotDataBindable | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            SelectChangedEvent = EventManager.RegisterRoutedEvent("SelectChanged", RoutingStrategy.Direct, typeof(BlockSelectPropertyMethodHandler), typeof(BlockBase));
            RootColorProperty = DependencyProperty.Register("RootColor", typeof(Brush), typeof(BlockBase), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits));

            IsBlockSelectProperty = DependencyProperty.Register("IsBlockSelect", typeof(bool), typeof(BlockBase));
            IsRootBlockSelectProperty = DependencyProperty.Register("IsRootBlockSelect", typeof(bool), typeof(BlockBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                (sender, e)=>SelectBlockChanged(sender, new(BlockTypes.Block, BlockProperties.NextBlock, null), e)));
        }
        public BlockBase()
        {
            Loaded += BlockBase_Loaded;

            PropertiesForGetSelect = new();
            PropertiesForGetSelect.Add(() => IsRootBlockSelect);

            PropertiesForSetSelect = new();
            PropertiesForSetSelect.Add(i => IsRootBlockSelect = i);

            PropertyForGetByName = new();
            PropertyForGetByName.Add((name) => name == BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock) && IsRootBlockSelect);

            PropertyForSetByName = new();
            PropertyForSetByName.Add((name, val) =>
            {
                bool res = name == BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock);
                if (res) IsRootBlockSelect = val;
                return res;
            });
        }

        #endregion//Constructors

        #region PrimarySetting
        private void BlockBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (RootBlock != null)
            {
                RootBlock.PreviewMouseDown -= BlockBase_MouseDown;
                RootBlock.PreviewMouseDown += BlockBase_MouseDown;
            }
            if (HorizontalCenterWidth != null)
            {
                HorizontalCenterWidth.SizeChanged -= BlockBase_SizeChanged;
                HorizontalCenterWidth.SizeChanged += BlockBase_SizeChanged;
                HorizontalCenter = GetHorizontalCenter();
            }
        }

        private void BlockBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HorizontalCenter = GetHorizontalCenter();
        }
        #endregion //PrimarySetting

        #region CustomPropertiesAndEvents

        public static readonly RoutedEvent SelectChangedEvent;
        public event BlockSelectPropertyMethodHandler SelectChanged
        {
            add { AddHandler(SelectChangedEvent, value); }
            remove { RemoveHandler(SelectChangedEvent, value); }
        }

        public static readonly DependencyProperty RootColorProperty;
        public Brush RootColor
        {
            get { return (Brush)GetValue(RootColorProperty); }
            set { SetValue(RootColorProperty, value); }
        }

        public static readonly DependencyProperty IsBlockSelectProperty;
        public bool IsBlockSelect
        {
            get { return (bool)GetValue(IsBlockSelectProperty); }
            set { SetValue(IsBlockSelectProperty, value); }
        }

        public static readonly DependencyProperty IsRootBlockSelectProperty;
        public bool IsRootBlockSelect
        {
            get { return (bool)GetValue(IsRootBlockSelectProperty); }
            set { SetValue(IsRootBlockSelectProperty, value); }
        }
        public static readonly DependencyProperty TextProperty;
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty HorizontalCenterProperty;
        public double HorizontalCenter
        {
            get { return (double)GetValue(HorizontalCenterProperty); }
            private set { SetValue(HorizontalCenterProperty, value); }
        }
        #endregion //CustomPropertysAndEvents

        #region HandlersEvents
        protected static void SelectBlockChanged(DependencyObject sender, BlockSelectPropertyMetadata metadata, DependencyPropertyChangedEventArgs e)
        {
            SelectBlockChanged(sender, metadata, (bool)e.OldValue, (bool)e.NewValue);
        }
        protected static void SelectBlockChanged(DependencyObject sender, BlockSelectPropertyMetadata metadata, bool oldValue, bool newValue)
        {
            BlockBase block = (BlockBase)sender;
            block.RaiseEvent(new BlockSelectPropertyMethodArgs(oldValue, newValue, SelectChangedEvent, Keyboard.Modifiers, block.FindPanel(block.GetPanel(metadata))));
        }

        private void BlockBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsBlockSelect)
                IsRootBlockSelect = !IsRootBlockSelect;
        }

        #endregion //HandlersEvents

        public bool IsSelect
        {
            get { return GetIsSelect(); }
        }

        /// <summary>
        /// Установка всех выделений
        /// </summary>
        /// <param name="value"></param>
        public void SetAllSelect(bool value)
        {
            foreach (var item in PropertiesForSetSelect)
                item.Invoke(value);
        }
        public bool GetSelect(BlockSelectPropertyMetadata e)
        {
            string name = e.Property;
            return GetSelect(name);
        }
        public bool GetSelect(string name)
        {
            foreach (var item in PropertyForGetByName)
                if (item.Invoke(name))
                    return true;
            return false;
        }
        public void SetSelect(BlockSelectPropertyMetadata e, bool val)
        {
            string name = e.Property;
            SetSelect(name, val);
        }
        public void SetSelect(string name, bool val)
        {
            foreach (var item in PropertyForSetByName)
                if (item.Invoke(name, val)) 
                    return;
        }


        #region PrivateAndProtectedPropertys

        protected FrameworkElement rootBlock;
        protected FrameworkElement RootBlock { get { return rootBlock ??= GetTemplateChild("rootBlock") as FrameworkElement; } }
        private FrameworkElement horizontalCenterWidth;
        private FrameworkElement HorizontalCenterWidth { get { return horizontalCenterWidth ??= GetTemplateChild("setHC") as FrameworkElement; } }
        private double GetHorizontalCenter()
        {
            return HorizontalCenterWidth.ActualWidth;
        }

        #endregion //PrivateAndProtectedPropertys

        #region OverrideMethods
        //необходимо переопределять если в блоке добавляете новую развилку

        /// <summary>
        /// Функция возвращает массив выделенный свойств
        /// </summary>
        /// <returns>свойства</returns>
        public virtual List<BlockSelectPropertyMetadata> GetSelectBlockPropertys()
        {
            List<BlockSelectPropertyMetadata> res = new();
            if (IsRootBlockSelect && Parent is BlockPanel panel)
                res.Add(new BlockSelectPropertyMetadata(BlockTypes.Block, BlockProperties.NextBlock, panel));
            return res;
        }
        /// <summary>
        /// Поиск панели в дочерних элементах
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public virtual BlockSelectPropertyMetadata FindPanel(BlockPanel panel)
        {
            if (Parent is BlockPanel parentPanel && parentPanel == panel)
                return new BlockSelectPropertyMetadata(BlockTypes.Block, BlockProperties.NextBlock, panel);
            return null;
        }

        /// <summary>
        /// Возврат панели, по свойству
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual BlockPanel GetPanel(BlockSelectPropertyMetadata property)
        {
            if (property.Property == BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock) && Parent is BlockPanel res)
                return res;
            return null;
        }

        //список функций. Если одна из функций возвратит true, то блок становится выделенным.
        protected List<Func<bool>> PropertiesForGetSelect { get; }
        protected List<Action<bool>> PropertiesForSetSelect { get; }
        protected List<Func<string, bool>> PropertyForGetByName { get; }
        protected List<Func<string, bool, bool>> PropertyForSetByName { get; }

        public virtual object Clone()
        {
            Type t = GetType();
            return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

        #endregion //OverrideMethods

        #region PrivateMethods
        private bool GetIsSelect()
        {
            bool res = false;
            foreach (var item in PropertiesForGetSelect)
                res |= item.Invoke();
            return res;
        }
        #endregion //PrivateMethods
    }
    public static class BlockPropertyConverter
    {
        private static Dictionary<BlockProperties, string> properties = new()
        {
            { BlockProperties.NextBlock, "Root" },
            { BlockProperties.True, "True" },
            { BlockProperties.False, "False" },
            { BlockProperties.Key, "Key" }
        };
        public static string ConvertToString(BlockProperties property) => properties[property];
        public static BlockProperties ConvertToStandartProperty(string property) => properties.Where(i=>i.Value == property).First().Key;

    }
    public enum BlockProperties
    {
        NextBlock,
        True,
        False,
        Key
    }
    public enum BlockTypes
    {
        Block,
        Cycle,
        If,
        Switch
    }

    public class BlockSelectPropertyMetadata
    {
        public BlockSelectPropertyMetadata(BlockTypes type, BlockProperties property, BlockPanel panel) : this(type, BlockPropertyConverter.ConvertToString(property), panel) { }
        public BlockSelectPropertyMetadata(BlockTypes type, string property, BlockPanel panel)
        {
            Type = type;
            Panel = panel;
            Property = property;
        }
        public BlockTypes Type { get; init; }
        public string Property { get; init; }
        public BlockPanel Panel { get; init; }
    }
    public delegate void BlockSelectPropertyMethodHandler(object sender, BlockSelectPropertyMethodArgs e);
    public class BlockSelectPropertyMethodArgs : RoutedEventArgs
    {
        public ModifierKeys Modifier { get; init; }
        public BlockSelectPropertyMetadata Metadata { get; init; }
        public bool OldValue { get; init; }
        public bool NewValue { get; init; }
        public BlockSelectPropertyMethodArgs(bool oldValue, bool newValue, RoutedEvent routedEvent, ModifierKeys modifier, BlockSelectPropertyMetadata metadata) : base(routedEvent)
        {
            Metadata = metadata;
            Modifier = modifier;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
