using BlockConverter.Tools;
using BlocksLib.Blocks;
using ModelConverterToBlock.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockConverter.ViewModels.UndoRedoCommands
{
    internal class UndoRedoCommandManager
    {
        private const int MaxLevel = 50;
        public UndoRedoCommandManager()
        {
            MemoryStack = new();
            FutureStack = new();
        }
        private Stack<IUndoRedoCommand> MemoryStack { get; set; }
        private Stack<IUndoRedoCommand> FutureStack { get; set; }
        public void AddCommand(IUndoRedoCommand command)
        {
            foreach (var item in FutureStack)
                item.BlockChanged -= AppendBlockChanged;
            FutureStack.Clear();
            MemoryStack.Push(command);
            command.BlockChanged += AppendBlockChanged;
            //if (MemoryStack.Count > MaxLevel)
            //{

            //}
        }
        public bool CanRunUndoCommand() => MemoryStack.Count != 0;
        public void Undo()
        {
            IUndoRedoCommand command = MemoryStack.Pop();
            FutureStack.Push(command);
            command.Undo();
        }

        public bool CanRunRedoCommand() => FutureStack.Count != 0;
        public void Redo()
        {
            IUndoRedoCommand command = FutureStack.Pop();
            MemoryStack.Push(command); 
            command.Redo();
        }
        private void AppendBlockChanged(object sender, BlockChangedEventArgs e)
        {
            foreach (var item in MemoryStack)
                item.ApppendBlockChanged(sender, e);
            foreach (var item in FutureStack)
                item.ApppendBlockChanged(sender, e);
        }
    }
    internal class BlockChangedEventArgs
    {
        public BlockChangedEventArgs(BlockBase old, BlockBase current)
        {
            OldBlock = old;
            CurrentBlock = current;
        }
        public BlockBase OldBlock { get; init; }
        public BlockBase CurrentBlock { get; init; }
    }
    internal delegate void BlockChangedHandler(object sender, BlockChangedEventArgs e);
    internal interface IUndoRedoCommand
    {
        public event BlockChangedHandler BlockChanged;
        public void ApppendBlockChanged(object sender, BlockChangedEventArgs e);
        public void Undo();
        public void Redo();
    }

    internal class UndoAddBlockSchemeCommand : IUndoRedoCommand
    {
        internal UndoAddBlockSchemeCommand(BlockSchemeFromViewItem scheme, BlockBase block, BlockBase fromBlock, BlockSelectPropertyMetadata property)
        {
            Scheme = scheme;
            Block = block;
            FromBlock = fromBlock;
            Property = property;
        }
        protected BlockSchemeFromViewItem Scheme { get; init; }
        protected internal BlockBase Block { get; private set; }
        protected BlockBase FromBlock { get; private set; }
        public BlockSelectPropertyMetadata Property { get; protected set; }

        public virtual event BlockChangedHandler BlockChanged;

        public virtual void Redo()
        {
            IUndoRedoCommand command;
            Scheme.AddBlockByProperty(Block, FromBlock, Property, out command);
            BlockChanged?.Invoke(this, new(Block, ((UndoAddBlockSchemeCommand)command).Block));
        }

        public virtual void Undo()
        {
            Scheme.RemoveBlock(Block);
        }

        public void ApppendBlockChanged(object sender, BlockChangedEventArgs e)
        {
            if (e.OldBlock == Block)
            {
                IBlockComponent component = BlockSchemeFromViewItem.GetComponent(Block);
                
                Block = e.CurrentBlock;
                
            }
            if (e.OldBlock == FromBlock)
            {
                FromBlock = e.CurrentBlock;
                Property = new BlockSelectPropertyMetadata(Property.Type, Property.Property, FromBlock.GetPanel(Property));
            }
        }
    }

    internal class UndoRemoveBlockSchemeCommand:UndoAddBlockSchemeCommand
    {
        internal UndoRemoveBlockSchemeCommand(BlockSchemeFromViewItem scheme, BlockBase block, BlockBase fromBlock, BlockSelectPropertyMetadata property) :
            base(scheme, block, fromBlock, property)
        { }
        public override event BlockChangedHandler BlockChanged;
        public override void Redo()
        {
            Scheme.RemoveBlock(Block);
        }

        public override void Undo()
        {
            IUndoRedoCommand command;
            Scheme.AddBlockByProperty(Block, FromBlock, Property, out command);
            BlockChanged?.Invoke(this, new(Block, ((UndoAddBlockSchemeCommand)command).Block));
        }
    }

    internal class UndoBlockMultiCommand : IUndoRedoCommand
    {
        public UndoBlockMultiCommand(IEnumerable<IUndoRedoCommand> commands)
        {
            Commands = commands;
            foreach (var item in commands)
                item.BlockChanged += OnBlockChanged;
        }
        protected IEnumerable<IUndoRedoCommand> Commands { get; init; }

        public event BlockChangedHandler BlockChanged;

        private void OnBlockChanged(object sender, BlockChangedEventArgs e) => BlockChanged?.Invoke(this, e);

        public void Redo()
        {
            foreach (var command in Commands)
                command.Redo();
        }

        public void Undo()
        {
            foreach (var command in Commands)
                command.Undo();
        }

        public void ApppendBlockChanged(object sender, BlockChangedEventArgs e)
        {
            foreach (var item in Commands)
                item.ApppendBlockChanged(sender, e);
        }
    }

    internal class UndoPasteBlockSchemeCommand : IUndoRedoCommand
    {
        public UndoPasteBlockSchemeCommand(BlockSchemeFromViewItem scheme, IEnumerable<BlockBase> blocks,
            BlockBase parentBlock, IBlockComponent startComponent, IBlockComponent endComponent, BlockSelectPropertyMetadata metaData)
        {
            Blocks = blocks.ToList();
            StartComponent = startComponent;
            endComponent.Remove(new());
            Scheme = scheme;
            ParentBlock = parentBlock;
            Metadata = metaData;
            RemoveCommands = new();
        }

        private BlockSchemeFromViewItem Scheme { get; set; }
        private BlockBase ParentBlock { get; set; }
        private List<BlockBase> Blocks { get; set; }
        private Stack<IUndoRedoCommand> RemoveCommands { get; set; }
        private IBlockComponent StartComponent { get; set; }
        private BlockSelectPropertyMetadata Metadata { get; set; }

        public event BlockChangedHandler BlockChanged;

        public void ApppendBlockChanged(object sender, BlockChangedEventArgs e)
        {
            if(Blocks.Contains(e.OldBlock))
                Blocks[Blocks.IndexOf(e.OldBlock)] = e.CurrentBlock;
            if (ParentBlock == e.OldBlock)
                ParentBlock = e.CurrentBlock;
            foreach (var item in RemoveCommands)
                item.ApppendBlockChanged(sender, e);
        }

        public void Redo()
        {
            foreach (var item in RemoveCommands)
            {
                item.Undo();
            }
            RemoveCommands.Clear();
        }

        public void Undo()
        {
            foreach (var item in Blocks)
            {
                IUndoRedoCommand command;
                if(Scheme.RemoveBlock(item, out command))
                {
                    command.BlockChanged += UndoCommandsBlockChanged;
                    RemoveCommands.Push(command);
                }
            }
        }

        private void UndoCommandsBlockChanged(object sender, BlockChangedEventArgs e)
        {
            BlockChanged?.Invoke(this, e);
        }
    }

}
