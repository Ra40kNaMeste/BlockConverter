using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlocksLib.Blocks.Tools
{
    public class SelectManagerDictionary : SelectDictionary
    {
        public SelectManagerDictionary()
        {
            Mode = new SelectDictionaryOneSelectMode(this);
            Builder = new();
            SelectChanged += OnSelectItem;
            IsIgnoreSelect = false;
        }
        public bool IsIgnoreSelect { get; set; }
        private void OnSelectItem(object sender, BlockSelectPropertyMethodArgs e)
        {

            if (!IsIgnoreSelect)
            {
                if ((ModifierKeys.Control & e.Modifier) != 0)
                    if (!(Mode is SelectDictionaryMultiSelectMode))
                        Mode = new SelectDictionaryMultiSelectMode();
                    else { }
                else if (!(Mode is SelectDictionaryOneSelectMode))
                    Mode = new SelectDictionaryOneSelectMode(this);

                IsIgnoreSelect = true;
                if (e.NewValue)
                    Mode.SelectItem(Builder.CreateBlock((BlockBase)sender, e.Metadata, (ModifierKeys.Shift & e.Modifier) != 0));
                else
                    Mode.UnselectItem(Builder.CreateBlock((BlockBase)sender, e.Metadata, (ModifierKeys.Shift & e.Modifier) != 0));
                SelectElements = SelectElements.Where(i => i.IsSelect).ToList();
                IsIgnoreSelect = false;

            }
            if (e.NewValue)
                SelectElements.Add((BlockBase)sender);
            else
                SelectElements.Remove((BlockBase)sender);
            SelectElements = SelectElements.Distinct().ToList();
        }

        public ISelectDictionaryMode Mode { get; set; }

        private SelectDictionaryModeBuilder Builder { get; init; }
        public void Select(BlockBase block, BlockSelectPropertyMetadata e, bool isEndBlock)
        {
            ISelectDictionaryModeItem item = Builder.CreateBlock(block, e, isEndBlock);
            Mode?.SelectItem(item);
        }
        
        public void ClearSelects()
        {
            IsIgnoreSelect = true;
            var selects = SelectElements.ToList();
            foreach (var item in selects)
                item.SetAllSelect(false);
            IsIgnoreSelect = false;

        }
    }

    public class SelectDictionary
    {
        public SelectDictionary()
        {
            SelectElements = new();
        }

        public SelectDictionary(IEnumerable<BlockBase> elements) => SelectElements = new(elements);

        public void Add(BlockBase element)
        {
            element.SelectChanged += Element_SelectChanged;
            if (element.IsSelect)
                SelectElements.Add(element);
        }

        public void Remove(BlockBase element)
        {
            element.SelectChanged -= Element_SelectChanged;
            if (SelectElements.Contains(element))
                SelectElements.Remove(element);
        }

        public event BlockSelectPropertyMethodHandler SelectChanged;

        private void Element_SelectChanged(object sender, BlockSelectPropertyMethodArgs e)
        {
            SelectChanged?.Invoke(sender, e);
        }

        protected internal List<BlockBase> SelectElements { get; protected set; }
        public IEnumerable<BlockBase> GetSelectElements()
        {
            return SelectElements.ToList();
        }

        public bool IsOneSelectElement
        {
            get { return SelectElements.Count != 0; }
        }
    }

    public interface ISelectDictionaryMode
    {
        public void SelectItem(ISelectDictionaryModeItem item);

        public void UnselectItem(ISelectDictionaryModeItem item);
    }

    public interface ISelectDictionaryModeItem
    {
        public void Select();
        public void UnSelect();
    }



    public class SelectDictionaryOneSelectMode : ISelectDictionaryMode
    {
        private SelectDictionary TargetDictionary { get; init; }
        public SelectDictionaryOneSelectMode(SelectDictionary dictionary)
        {
            TargetDictionary = dictionary;
        }
        public void SelectItem(ISelectDictionaryModeItem item)
        {
            CleanSelectDictionary();
            item.Select();
        }
        private void CleanSelectDictionary()
        {
            var selects = TargetDictionary.SelectElements.ToList();
            foreach (var select in selects)
                select.SetAllSelect(false);
        }
        public void UnselectItem(ISelectDictionaryModeItem item)
        {
            bool isSelect = TargetDictionary.SelectElements.Count > 1;
            CleanSelectDictionary();
            if (isSelect)
                item.Select();
        }
    }
    public class SelectDictionaryMultiSelectMode : ISelectDictionaryMode
    {
        public void SelectItem(ISelectDictionaryModeItem item)
        {
            item.Select();
        }
        public void UnselectItem(ISelectDictionaryModeItem item)
        {
            item.UnSelect();
        }
    }

    public class SelectDictionaryModeBuilder
    {
        private SaveSelectDictionatyOneModeItem item;
        public ISelectDictionaryModeItem CreateBlock(BlockBase block, BlockSelectPropertyMetadata e, bool isEndBlock)
        {
            if (isEndBlock && item != null)
            {
                SaveSelectDictionaryIntervalModeItem temp = item as SaveSelectDictionaryIntervalModeItem ?? new(item.SelectBlock, item.SelectProperty);
                temp.SetSecondBlock(block);
                item = temp;
            }
            else
                item = new SaveSelectDictionatyOneModeItem(block, e);
            return item;
        }
    }

    public class SaveSelectDictionatyOneModeItem : ISelectDictionaryModeItem
    {
        protected internal BlockBase SelectBlock { get; init; }
        protected internal BlockSelectPropertyMetadata SelectProperty { get; set; }
        public SaveSelectDictionatyOneModeItem(BlockBase selectBlock, BlockSelectPropertyMetadata e)
        {
            SelectBlock = selectBlock;
            SelectProperty = e;
        }

        public virtual void Select()
        {
            SelectBlock.SetSelect(SelectProperty, true);
        }

        public virtual void UnSelect()
        {
            SelectBlock.SetSelect(SelectProperty, false);
        }
    }
    public class SaveSelectDictionaryIntervalModeItem : SaveSelectDictionatyOneModeItem
    {
        public SaveSelectDictionaryIntervalModeItem(BlockBase selectBlock, BlockSelectPropertyMetadata e) : base(selectBlock, e)
        {
            SecondProperty = new(BlockTypes.Block, BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock), null);
        }
        public bool SetSecondBlock(BlockBase secondBlock)
        {
            BlockPanel secondPanel = secondBlock.GetPanel(new BlockSelectPropertyMetadata(BlockTypes.Block, BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock), null));
            BlockSelectPropertyMetadata newSelectProperty;
            if ((newSelectProperty = SelectBlock.FindPanel(secondPanel)) == null)
                return false;
            SelectProperty = newSelectProperty;
            SecondSelectBlock = secondBlock;
            TargetPanel = secondPanel;
            return true;
        }
        private BlockBase secondSelectBlock;

        private BlockBase SecondSelectBlock
        {
            get => secondSelectBlock;
            set
            {
                MemorySelectBlock = secondSelectBlock;
                secondSelectBlock = value;
            }
        }
        private BlockSelectPropertyMetadata SecondProperty { get; set; }
        private BlockBase MemorySelectBlock { get; set; }
        private BlockPanel TargetPanel { get; set; }
        public override void Select()
        {
            SelectBlock.SetSelect(SelectProperty, true);
            if (SecondSelectBlock == null)
            {
                return;
            }
            SetSelect(true, TargetPanel.Children.IndexOf(SelectBlock), TargetPanel.Children.IndexOf(SecondSelectBlock));
            SecondSelectBlock.SetSelect(SecondProperty, false);
            SecondSelectBlock.SetSelect(new BlockSelectPropertyMetadata(BlockTypes.Block, BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock), TargetPanel), true);
        }
        public override void UnSelect()
        {
            if (SecondSelectBlock == null)
                return;
            if (MemorySelectBlock == null || SecondSelectBlock == MemorySelectBlock)
                return;

            SetSelect(false, TargetPanel.Children.IndexOf(SecondSelectBlock), TargetPanel.Children.IndexOf(MemorySelectBlock));
            SecondSelectBlock.SetSelect(new BlockSelectPropertyMetadata(BlockTypes.Block, BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock), TargetPanel), true);
        }
        private void SetSelect(bool val, int first, int second)
        {
            if (first == -1)
                first = 0;
            int temp = Math.Min(first, second);
            second = Math.Max(first, second);
            first = temp;
            for (int i = first; i <= second; i++)
            {
                BlockBase block = TargetPanel.Children[i] as BlockBase;
                block.SetSelect(new BlockSelectPropertyMetadata(BlockTypes.Block, BlockPropertyConverter.ConvertToString(BlockProperties.NextBlock), TargetPanel), val);
            }
        }
    }
}
