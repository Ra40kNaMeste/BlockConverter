using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Reflection;

namespace BlocksLib.Blocks
{
    [TemplatePart(Name = "selectNoBlock", Type=typeof(FrameworkElement))]
    /// <summary>
    /// Логика взаимодействия для BlockIf.xaml
    /// </summary>
    public partial class BlockIf : BlockCycleBase
    {
        #region Constructors
        public BlockIf()
        {
            Loaded += BlockIf_Loaded;
            PropertiesForGetSelect.Add(() => IsNoPanelSelect);
            PropertiesForSetSelect.Add(i => IsNoPanelSelect = i);

            PropertyForGetByName.Add((name) => name == BlockPropertyConverter.ConvertToString(BlockProperties.False) && IsNoPanelSelect);
            PropertyForSetByName.Add((name, val) =>
            {
                bool res = name == BlockPropertyConverter.ConvertToString(BlockProperties.False);
                if (res) IsNoPanelSelect = val;
                return res;
            });

        }
        static BlockIf()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockIf), new FrameworkPropertyMetadata(typeof(BlockIf)));
            FalseColorProperty = DependencyProperty.Register("FalseColor", typeof(Brush), typeof(BlockIf), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits));
            IsNoPanelSelectProperty = DependencyProperty.Register("IsNoPanelSelect", typeof(bool), typeof(BlockIf), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                 (sender, e) => SelectBlockChanged(sender, new(BlockTypes.Block, BlockProperties.False, null), e)));

            FalseSourceProperty = DependencyProperty.Register("FalseSource", typeof(BlockPanel),
                typeof(BlockIf), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSourceUpdate));
        }
        #endregion //Constructors

        #region PrimarySettings
        private void BlockIf_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectNoBlock != null)
                SelectNoBlock.MouseDown += SelectNoBlock_MouseDown;
        }

        private void SelectNoBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsNoPanelSelect = !IsNoPanelSelect;
            e.Handled = true;
        }

        #endregion //PrimarySettings

        #region PublicPropertys
        public static readonly DependencyProperty FalseColorProperty;
        public Brush FalseColor
        {
            get { return (Brush)GetValue(FalseColorProperty); }
            set { SetValue(FalseColorProperty, value); }
        }
        public static DependencyProperty IsNoPanelSelectProperty;
        public bool IsNoPanelSelect
        {
            get { return (bool)GetValue(IsNoPanelSelectProperty); }
            set { SetValue(IsNoPanelSelectProperty, value); }
        }
        public static readonly DependencyProperty FalseSourceProperty;
        public BlockPanel FalseSource
        {
            get { return (BlockPanel)GetValue(FalseSourceProperty); }
            set { SetValue(FalseSourceProperty, value); }
        }
        #endregion //PublicPropertys

        #region OverrideMethods
        public override List<BlockSelectPropertyMetadata> GetSelectBlockPropertys()
        {
            List<BlockSelectPropertyMetadata> res = base.GetSelectBlockPropertys();
            if (IsNoPanelSelect)
                res.Add(new(BlockTypes.If, BlockPropertyConverter.ConvertToString(BlockProperties.False), FalseSource));
            return res;
        }


        public override BlockSelectPropertyMetadata FindPanel(BlockPanel panel)
        {
            if (FalseSource == panel)
                return new(BlockTypes.If, BlockPropertyConverter.ConvertToString(BlockProperties.False), panel);
            return base.FindPanel(panel);
        }

        public override BlockPanel GetPanel(BlockSelectPropertyMetadata property)
        {
            if (property.Property == BlockPropertyConverter.ConvertToString(BlockProperties.False))
                return FalseSource;
            return base.GetPanel(property);
        }

        #endregion //OverrideMethods

        protected FrameworkElement selectNoBlock;
        protected FrameworkElement SelectNoBlock { get { return selectNoBlock ??= GetTemplateChild("selectNoBlock") as FrameworkElement; } }

    }
}