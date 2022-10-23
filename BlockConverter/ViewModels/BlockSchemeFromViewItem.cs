using BlockConverter.BlockFactorys;
using BlocksLib.Blocks;
using BlocksLib.Blocks.Tools;
using BlockConverter.Tools;
using ModelConverterToBlock.Blocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using BlockConverter.ViewModels.UndoRedoCommands;
using ModelConverterToBlock.Converters.Reader;

namespace BlockConverter.ViewModels
{
    //Класс с одной схемой
    class BlockSchemeFromViewItem : INotifyPropertyChanged
    {
        #region VisualProperties
        private Panel rootPanel;
        /// <summary>
        /// Панель, на которой компануются блоки
        /// </summary>
        public Panel RootPanel
        {
            get { return rootPanel; }
            private set
            {
                rootPanel = value;
                FillFactories();
                OnPropertyChanged();
            }
        }
        #endregion //VisualProperties


        #region Commands

        #region CommandHead

        private CustomCommand addBlockCommand;
        public CustomCommand AddBlockCommand => addBlockCommand ??= new(AddBlockCommandBody, CanAddBlockCommandBody);

        private CustomCommand removeBlockCommand;
        public CustomCommand RemoveBlockCommand => removeBlockCommand ??= new(RemoveBlockCommandBody, CanRemoveBlockCommandBody);

        private CustomCommand undoCommand;
        public CustomCommand UndoCommand => undoCommand ??= new(UndoCommandBody, CanUndoCommandBody);

        private CustomCommand redoCommand;
        public CustomCommand RedoCommand => redoCommand ??= new(RedoCommandBody, CanRedoCommandBody);

        private CustomCommand copyCommand;
        public CustomCommand CopyCommand => copyCommand ??= new(CopyCommandBody, CanCopyCommandBody);

        private CustomCommand pasteCommand;
        public CustomCommand PasteCommand => pasteCommand ??= new(PasteCommandBody, CanPasteCommandBody);

        #endregion//CommandHead

        #region CommandBody

        private bool CanAddBlockCommandBody(object parameter) => parameter is BlockBase;
        private void AddBlockCommandBody(object parameter)
        {
            SelectDictionary.IsIgnoreSelect = true;
            if (parameter is BlockBase block)
            {
                var parameters = ParametersAllBlocksManager.GetElement(block);
                block = CreateBlock(parameters);
                List<IUndoRedoCommand> commands = new();
                var selectBlocks = SelectDictionary.GetSelectElements();
                foreach (var selectBlock in selectBlocks)
                {
                    var selectProps = selectBlock.GetSelectBlockPropertys();
                    foreach (var property in selectProps)
                    {
                        IUndoRedoCommand command;
                        AddBlockByProperty(block, selectBlock, property, out command);
                        commands.Add(command);
                    }
                    selectBlock.SetAllSelect(false);
                }
                undoRedoCommandManager.AddCommand(new UndoBlockMultiCommand(commands));
            }
            SelectDictionary.IsIgnoreSelect = false;
        }

        private bool CanRemoveBlockCommandBody(object parameter) => SelectDictionary.IsOneSelectElement;
        private void RemoveBlockCommandBody(object parameter)
        {
            SelectDictionary.IsIgnoreSelect = true;
            List<IUndoRedoCommand> commands = new();
            var removeBlocks = SelectDictionary.GetSelectElements();
            foreach (var block in removeBlocks)
            {
                IUndoRedoCommand command;
                BlockBase parentBlock;
                if (RemoveBlock(block, out command, out parentBlock))
                {
                    commands.Add(command);
                    if(parentBlock != null)
                        parentBlock.IsRootBlockSelect = true;
                }
            }
            undoRedoCommandManager.AddCommand(new UndoBlockMultiCommand(commands));
            SelectDictionary.IsIgnoreSelect = false;
        }

        private bool CanUndoCommandBody(object parameter) => undoRedoCommandManager.CanRunUndoCommand();
        private void UndoCommandBody(object parameter) => undoRedoCommandManager.Undo();

        private bool CanRedoCommandBody(object parameter) => undoRedoCommandManager.CanRunRedoCommand();
        private void RedoCommandBody(object parameter) => undoRedoCommandManager.Redo();

        private bool CanCopyCommandBody(object parameter) => SelectDictionary.IsOneSelectElement;
        private void CopyCommandBody(object parameter)
        {
            IEnumerable<BlockBase> selectBlocks;
            if ((selectBlocks = SelectDictionary.GetSelectElements()).Count() == 0)
                return;

            //List<BlockBase> res = new();
            //foreach (var block in selectBlocks)
            //    res.AddRange(FindAllChildBlocksRecurs(block));

            //var blocks = res.Distinct().Select(i => BlockComponentOperations.CloneWithChilds((IBlockComponent)GetComponent(i))).ToArray();
            var blocks = SelectDictionary.GetSelectElements()
                .GroupBy(i => i.Parent)
                .Aggregate((i, j) => i.Count() > j.Count() ? i : j)
                .Select(i => new { Panel = (BlockPanel)i.Parent, Block = i })
                .OrderBy(i => i.Panel.Children.IndexOf(i.Block))
                .Select(i => BlockComponentOperations.CloneWithChilds((IBlockComponent)GetComponent(i.Block))).ToArray();
            int count = blocks.Count() - 1;
            for (int i = 0; i < count; i++)
                blocks[i].Add(blocks[i + 1], new BlockComponentMetadata());
            ClipBoard = blocks[0];
        }
        private List<BlockBase> FindAllChildBlocksRecurs(BlockBase block)
        {
            List<BlockBase> res = new();
            res.Add(block);
            IBlockComponent component = GetComponent(block);
            var metaDates = component.GetAllPossibleMetadates();

            foreach (var item in metaDates)
            {
                Panel itemPanel = BlockModelToViewConverter.FindPanelByMetadata(block, item);
                if (itemPanel == null)
                    continue;

                foreach (var removeBlock in itemPanel.Children)
                    if ((removeBlock is BlockBase temp) && !(temp is BlockEmpty))
                        res.AddRange(FindAllChildBlocksRecurs(temp));
            }
            return res;

        }

        private bool CanPasteCommandBody(object parameter) => ClipBoard != null;
        private void PasteCommandBody(object parameter)
        {
            var selects = SelectDictionary.GetSelectElements();
            List<IUndoRedoCommand> commands = new();
            foreach (var item in selects)
            {
                var properties = item.GetSelectBlockPropertys();
                foreach (var property in properties)
                {
                    var temp = BlockComponentOperations.CloneTree(ClipBoard);
                    IBlockComponent startBlock = temp, endBlock = FindEndComponent(temp);
                    ICollection<BlockBase> blocks = BlockModelToViewConverter.InsertBlockComponentToView(temp, property.Panel, this, item);
                    BlockComponentMetadata metaData = ConvertBlockVisualPropertyToBlockComponentMetaData(property);
                    BlockComponentOperations.InsertBlock(GetComponent(item), temp, metaData);
                    commands.Add(new UndoPasteBlockSchemeCommand(this, blocks, item, startBlock, endBlock, property));
                }
            }
            undoRedoCommandManager.AddCommand(new UndoBlockMultiCommand(commands));
        }
        #endregion //CommandBody

        #region Function

        public BlockBase CreateBlock(FactoryParameters parameters)
        {
            BlockNameToStringVisualWordConverter converter = new();
            BlockFactoryBase factory = GetFactory(parameters.BlockType).Item1;
            BlockBase blockBase = factory.GetBlock((IBlockComponent)parameters.BlockComponent.Clone());
            blockBase.SetResourceReference(FrameworkElement.StyleProperty, parameters.CustomStyle);
            blockBase.Text = converter.Constructions[parameters.BlockType];
            return blockBase;
        }

        protected internal bool AddBlockByProperty(BlockBase block, BlockBase fromBlock, BlockSelectPropertyMetadata property, out IUndoRedoCommand undoCommand)
        {
            var factory = GetFactory(ParametersAllBlocksManager.GetVisualBlockTypesByBlock(block));

            BlockBase child = factory.Item1.AddBlock((IBlockComponent)factory.Item2.Clone(), fromBlock, property.Panel);
            child.IsRootBlockSelect = true;
            child.Text = block.Text;
            BlockComponentOperations.InsertBlock(GetComponent(fromBlock),
                GetComponent(child), ConvertBlockVisualPropertyToBlockComponentMetaData(property));
            undoCommand = new UndoAddBlockSchemeCommand(this, child, fromBlock, property);
            return true;
        }
        protected internal bool RemoveBlock(BlockBase block)
        {
            IUndoRedoCommand temp;
            return RemoveBlock(block, out temp);
        }
        protected internal bool RemoveBlock(BlockBase block, out IUndoRedoCommand undoCommand)
        {
            return RemoveBlock(block, out undoCommand, out _);
        }

        protected internal bool RemoveBlock(BlockBase block, out IUndoRedoCommand undoCommand, out BlockBase parentBlock)
        {
            IBlockComponent component = GetComponent(block);
            BlockPanel panel = (BlockPanel)block.Parent;

            if (component == rootComponent || panel == null)
            {
                undoCommand = null;
                parentBlock = null;
                return false;
            }
            int index;
            BlockBase parent;
            if ((index = panel.Children.IndexOf(block)) == 0)
            {
                parentBlock = null;
                if ((parent = MultiChildBlockBase.GetParentBlock(panel)) == null)
                {
                    undoCommand = null;
                    return false;
                }
                BlockComponentMetadata metaData = ConvertBlockVisualPropertyToBlockComponentMetaData(parent.FindPanel(panel));
                BlockComponentOperations.RemoveBlock(GetComponent(parent), metaData);
            }
            else 
            {
                parent = (BlockBase)panel.Children[index - 1];
                BlockComponentOperations.RemoveBlock(GetComponent(parent), new BlockComponentMetadata());
                parentBlock = parent;
            }
            var removes = RemoveChildBlocksRecurs(block);

            removes.Insert(0, new UndoRemoveBlockSchemeCommand(this, block, parent, parent.FindPanel(panel)));
            undoCommand = new UndoBlockMultiCommand(removes);
            
            GetFactory(ParametersAllBlocksManager.GetVisualBlockTypesByBlock(block)).Item1.RemoveBlock(block);
            return true;
        }

        private List<IUndoRedoCommand> RemoveChildBlocksRecurs(BlockBase block)
        {
            IBlockComponent component = GetComponent(block);
            var metaDates = component.GetAllPossibleMetadates();
            List<IUndoRedoCommand> removes = new();

            foreach (var item in metaDates)
            {
                Panel itemPanel = BlockModelToViewConverter.FindPanelByMetadata(block, item);
                if (itemPanel == null)
                    continue;
                List<IUndoRedoCommand> removesChild = new();

                List<BlockBase> removeBlocks = new();
                foreach (var removeBlock in itemPanel.Children)
                    if ((removeBlock is BlockBase temp) && !(temp is BlockEmpty))
                        removeBlocks.Add(temp);

                foreach (var removeBlock in removeBlocks)
                {
                    IUndoRedoCommand command;
                    RemoveBlock(removeBlock, out command);
                    removesChild.Add(command);
                }
                removes.Add(new UndoBlockMultiCommand(removesChild));
            }
            return removes;
        }

        protected internal UndoRedoCommandManager undoRedoCommandManager = new();

        private IBlockComponent ClipBoard { get; set; }

        #endregion//Function

        #endregion//Command

        #region Constructions
        public BlockSchemeFromViewItem()
        {
            FillFactories();
            Tune();
        }
        #endregion //Constructions

        #region FactoriesLib
        //словарь фабрик. Ключ - тип блока. BlockTypes или Type Для всех моделей
        public Dictionary<VisualBlockTypes, (BlockFactoryBase, IBlockComponent)> Factories { get; set; }

        /// <summary>
        /// Возвращает фабрику и соответствующий компонент
        /// </summary>
        /// <param name="name">Имя фабрики</param>
        /// <returns></returns>
        public (BlockFactoryBase, IBlockComponent) GetFactory(VisualBlockTypes name)
        {
            (BlockFactoryBase, IBlockComponent) res;
            if (Factories.TryGetValue(name, out res))
                return res;
            throw new ArgumentException("Фабрика не найдена");
        }

        //Заполняет нестатические фабрики
        private void FillFactories()
        {
            Factories = new();
            AddFactory(VisualBlockTypes.Begin, new BlockBeginFactory(RootPanel));
            AddFactory(VisualBlockTypes.Operation, new BlockFactory());
            AddFactory(VisualBlockTypes.End, new BlockEndFactory());
            AddFactory(VisualBlockTypes.Input, new BlockInputFactory());
            AddFactory(VisualBlockTypes.Output, new BlockOutputFactory());

            AddFactory(VisualBlockTypes.Method, new BlockMethodFactory());
            AddFactory(VisualBlockTypes.If, new BlockIfFactory());
            AddFactory(VisualBlockTypes.Switch, new BlockSwitchFactory());

            AddFactory(VisualBlockTypes.Cycle, new BlockCustomCycleFactory(BlockCycleTypes.BlockCycle));
            AddFactory(VisualBlockTypes.PostCycle, new BlockCustomCycleFactory(BlockCycleTypes.BlockPostCycle));
            AddFactory(VisualBlockTypes.For, new BlockCustomCycleFactory(BlockCycleTypes.BlockForCycle));
        }

        //Вызывается на изменение выделенной библиотеки выделения блоков
        private void UpdateSelectDictionary()
        {
            foreach (var item in Factories)
                item.Value.Item1.SetSelectDictionary(SelectDictionary);
        }


  

        //создаёт и добавляет фабрику в словарь
        private void AddFactory(VisualBlockTypes key, BlockFactoryBase factory)
        {
            var par = ParametersAllBlocksManager.FactoryStyles.Where((i) => i.BlockType == key).FirstOrDefault();

            //factory.DefaultStyleName = par.DefaultStyle;
            factory.StyleName = par.CustomStyle;
            Factories.Add(key, (factory, par.BlockComponent));
        }


        #endregion //FactoriesLi

        #region ToolsViewAndModelData
        //коллекция для выделения блоков. Хранит выделенные блоки
        private SelectManagerDictionary selectDictionary;
        public SelectManagerDictionary SelectDictionary
        {
            get { return selectDictionary; }
            private set
            {
                selectDictionary = value;
                UpdateSelectDictionary();
                OnPropertyChanged();
            }
        }

        //Блок-начало программы
        private BeginBlockComponent rootComponent;
        public BeginBlockComponent RootComponent
        {
            get { return rootComponent; }
            set
            {
                rootComponent = value;
                Tune();
                OnPropertyChanged();
            }
        }

        //Сброс
        private void Tune()
        {
            RootPanel = new Grid();
            SelectDictionary = new();
            if (RootComponent != null)
                BlockModelToViewConverter.ConvertToView(RootComponent, this);
        }

        //Находит связанный с блоком компонент
        internal static IBlockComponent GetComponent(BlockBase block)
        {
            BindingExpression expression = BindingOperations.GetBindingExpression(block, BlockBase.TextProperty);
            if (expression == null || !(expression.DataItem is IBlockComponent))
                return null;
            return (IBlockComponent)expression.DataItem;
        }

        //Возвращает метаданные нахождения дочернего блока
        internal static BlockComponentMetadata ConvertBlockVisualPropertyToBlockComponentMetaData(BlockSelectPropertyMetadata visualProperty)
        {

            switch (visualProperty.Type)
            {
                case BlockTypes.Switch:
                    return new BlockSwitchComponentMetadata(visualProperty.Property);
                default:
                    return BlockPropertyConverter.ConvertToStandartProperty(visualProperty.Property) switch
                    {
                        BlockProperties.NextBlock => new BlockComponentMetadata(),
                        BlockProperties.True => new BlockIfComponentMetadata(BlockIfChilds.TrueBlock),
                        BlockProperties.False => new BlockIfComponentMetadata(BlockIfChilds.FalseBlock),
                        _ => throw new ArgumentException("Не корректное свойство")
                    };
            }
        }

        private IBlockComponent FindEndComponent(IBlockComponent component)
        {
            IBlockComponent res = component, temp;
            while ((temp = res.GetComponent(new()))!=null)
                res = temp;
            return res;
        }

        #endregion//ToolsViewAndModelData

        //Реализаци интерфейса
        private void OnPropertyChanged([CallerMemberName] string str = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    sealed class AddBlockParameters
    {
        public BlockBase TargetBlock { get; set; }
        public IEnumerable<BlockSelectPropertyMetadata> Properties { get; set; }
    }



}
