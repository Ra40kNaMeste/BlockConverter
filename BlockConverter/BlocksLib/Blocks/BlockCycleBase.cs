using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace BlocksLib.Blocks
{


    [TemplatePart(Name = "selectYesBlock", Type = typeof(FrameworkElement))]
    public abstract class BlockCycleBase : MultiChildBlockBase
    {
        #region Constructors
        public BlockCycleBase()
        {
            Loaded += BlockCycleBase_Loaded;
            PropertiesForGetSelect.Add(() => IsYesPanelSelect);
            PropertiesForSetSelect.Add(i => IsYesPanelSelect = i);

            PropertyForGetByName.Add((name) => name == BlockPropertyConverter.ConvertToString(BlockProperties.True) && IsYesPanelSelect);
            PropertyForSetByName.Add((name, val) => 
            {
                bool res = name == BlockPropertyConverter.ConvertToString(BlockProperties.True);
                if (res) IsYesPanelSelect = val;
                return res;
            });

        }
        static BlockCycleBase()
        {
            TrueSourceProperty = DependencyProperty.Register("TrueSource", typeof(BlockPanel),
                typeof(BlockCycleBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, OnSourceUpdate));
            TrueColorProperty = DependencyProperty.Register("TrueColor", typeof(Brush), typeof(BlockCycleBase), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits));
            IsYesPanelSelectProperty = DependencyProperty.Register("IsYesPanelSelect", typeof(bool), typeof(BlockCycleBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                 (sender, e) => SelectBlockChanged(sender, new(BlockTypes.Block, BlockProperties.True, null), e)));
            TrueTextProperty = DependencyProperty.Register("TrueText", typeof(string), typeof(BlockCycleBase), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            FalseTextProperty = DependencyProperty.Register("FalseText", typeof(string), typeof(BlockCycleBase), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        }
        #endregion //Constructors

        #region PublicPropertys
        public static readonly DependencyProperty TrueColorProperty;
        public Brush TrueColor
        {
            get { return (Brush)GetValue(TrueColorProperty); }
            set { SetValue(TrueColorProperty, value); }
        }

        public static readonly DependencyProperty TrueTextProperty;
        public string TrueText
        {
            get { return (string)GetValue(TrueTextProperty); }
            set { SetValue(TrueTextProperty, value); }
        }

        public static readonly DependencyProperty FalseTextProperty;
        public string FalseText
        {
            get { return (string)GetValue(FalseTextProperty); }
            set { SetValue(FalseTextProperty, value); }
        }

        public static readonly DependencyProperty TrueSourceProperty;
        public BlockPanel TrueSource
        {
            get { return (BlockPanel)GetValue(TrueSourceProperty); }
            set { SetValue(TrueSourceProperty, value); }
        }
        public static readonly DependencyProperty IsYesPanelSelectProperty;
        public bool IsYesPanelSelect
        {
            get { return (bool)GetValue(IsYesPanelSelectProperty); }
            set { SetValue(IsYesPanelSelectProperty, value); }
        }
        #endregion //PublicPropertys

        #region PrimarySetting
        private void BlockCycleBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectYesBlock != null)
                SelectYesBlock.MouseDown += SelectYesBlock_MouseDown;
        }

        private void SelectYesBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsYesPanelSelect = !IsYesPanelSelect;
            e.Handled = true;
        }

        #endregion //PrimarySetting

        #region OverrideMethods

        public override BlockSelectPropertyMetadata FindPanel(BlockPanel panel)
        {
            if (TrueSource == panel)
                return new(BlockTypes.Cycle, BlockPropertyConverter.ConvertToString(BlockProperties.True), panel);
            return base.FindPanel(panel);
        }
        public override BlockPanel GetPanel(BlockSelectPropertyMetadata property)
        {
            if (property.Property == BlockPropertyConverter.ConvertToString(BlockProperties.True))
                return TrueSource;
            return base.GetPanel(property);
        }

        public override List<BlockSelectPropertyMetadata> GetSelectBlockPropertys()
        {
            List<BlockSelectPropertyMetadata> res = base.GetSelectBlockPropertys();
            if (IsYesPanelSelect)
                res.Add(new(BlockTypes.Cycle, BlockPropertyConverter.ConvertToString(BlockProperties.True), TrueSource));
            return res;
        }

        #endregion //OverrideMethods

        #region ProtectedProperties
        protected FrameworkElement selectYesBlock;
        protected FrameworkElement SelectYesBlock { get { return selectYesBlock ??= GetTemplateChild("selectYesBlock") as FrameworkElement; } }

        #endregion//ProtectedPropertys
    }
}
