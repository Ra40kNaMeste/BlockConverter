using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BlocksLib.Blocks;
using BlocksLib.Blocks.Tools;
using ModelConverterToBlock.Blocks;
using BlockConverter;

namespace BlockConverter.BlockFactorys
{
    abstract class BlockFactoryBase
    {


        //При наследовании обязательно переопределить GetBlock и StyleName. AddBlock и RemoveBlock по желанию
        protected SelectDictionary selectDictionary;
        /// <summary>
        /// устанавливает библиотеку выделений для автоматического добавления в неё блока
        /// </summary>
        /// <param name="dictionary"></param>
        public void SetSelectDictionary(SelectDictionary dictionary) => selectDictionary = dictionary;
        /// <summary>
        /// выставляет стандартные настройки на генерируемый блок
        /// </summary>
        /// <param name="block"></param>
        private void AcceptPrimitiveSettigsBlock(BlockBase block)
        {
            block.SetResourceReference(FrameworkElement.StyleProperty, StyleName);

            if (selectDictionary == null)
                throw new Exception("Задайте библиотеку выделения через SetSelectDictionary");
            selectDictionary.Add(block);
        }
        /// <summary>
        /// вставляет в список генерируемый блок
        /// </summary>
        /// <param name="component">Связываются с ним блоки</param>
        /// <param name="root">блок перед вставляемым блоком</param>
        /// <param name="panel">куда вставлять. Если равно null то берётся родительская панель root</param>
        public virtual BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel panel = null)
        {
            BlockBase block = GetBlock(component);
            if (panel == null)
                panel = (BlockPanel)root.Parent;
            block.SetBinding(BlockBase.TextProperty, GetBinding("Content", component));
            int index = panel.Children.IndexOf(root) + 1;
            AcceptPrimitiveSettigsBlock(block);
            panel.Children.Insert(index, block);
            return block;
        }
        /// <summary>
        /// удаляется блок
        /// </summary>
        /// <param name="block"></param>
        public void RemoveBlock(BlockBase block)
        {
            selectDictionary.Remove(block);
            BlockPanel panel = block.Parent as BlockPanel;
            if (panel != null)
                panel.Children.Remove(block);
        }
        protected static Binding GetBinding(string path, object source)
        {

            Binding binding = new Binding(path);
            binding.Source = source;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            return binding;
        }
        /// <summary>
        /// Переопределяем. Имя стиля в ресурсах
        /// </summary>
        //public string DefaultStyleName { get; set; }
        public string StyleName { get; set; }
        /// <summary>
        /// переопределяем
        /// </summary>
        /// <returns>Тип возвращаемого блока</returns>
        public abstract BlockBase GetBlock(IBlockComponent component);
    }
    class BlockFactory : BlockFactoryBase
    {
        /// <summary>
        /// добавляет блок
        /// </summary>
        /// <param name="component">Связанный компонент из модели</param>
        /// <param name="root">Блок, после которого вставлять</param>
        /// <param name="panel">куда вставлять. Если равно null то берётся родительская панель root</param>
        public override BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel panel = null)
        {
            if (!(component is BlockComponent))
                throw new ArgumentException("ожидался компонент блока");
            return base.AddBlock(component, root, panel);
        }

        public override BlockBase GetBlock(IBlockComponent component)
        {
            return new Block() { TypeName = BaseBlockTypes.Block };
        }
    }
    class BlockBeginFactory : BlockFactoryBase
    {
        public BlockBeginFactory(Panel rootPanel) { RootPanel = rootPanel; }
        private Panel RootPanel { get; init; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="root">Оставить null</param>
        /// <param name="p">Оставить пустым</param>
        public override BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel p = null)
        {
            if (!(component is BlockComponent))
                throw new ArgumentException("ожидался компонент блока");
            if (p != null)
                throw new ArgumentException("невозможно добавить блок начала в панель");
            BlockPanel blockPanel = new();
            blockPanel.IsStretchLastElement = false;
            RootPanel.Children.Add(blockPanel);
            return base.AddBlock(component, root, blockPanel);
        }

        public override BlockBase GetBlock(IBlockComponent component)
        {
            return new Block() { TypeName = BaseBlockTypes.BeginBlock };
        }
    }
    class BlockEndFactory : BlockFactoryBase
    {
        /// <summary>
        /// добавляет блок
        /// </summary>
        /// <param name="component">Связанный компонент из модели</param>
        /// <param name="root">Блок, после которого вставлять</param>
        /// <param name="panel">куда вставлять. Если равно null то берётся родительская панель root</param>
        public override BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel panel = null)
        {
            if (!(component is BlockEndComponent))
                throw new ArgumentException("ожидался конечный компонент");
            return base.AddBlock(component, root, panel);
        }

        public override BlockBase GetBlock(IBlockComponent component)
        {
            return new Block() { TypeName = BaseBlockTypes.EndBlock };
        }
    }
    class BlockInputFactory : BlockFactoryBase
    {
        public override BlockBase GetBlock(IBlockComponent component)
        {
            return new Block() { TypeName = BaseBlockTypes.InputBlock };
        }
    }
    class BlockOutputFactory : BlockInputFactory
    {
        public override BlockBase GetBlock(IBlockComponent component)
        {
            return new Block() { TypeName = BaseBlockTypes.OutputBlock };
        }
    }

    class BlockMethodFactory : BlockFactory
    {
        public override BlockBase GetBlock(IBlockComponent component)
        {
            BlockMethod res = new();
            res.SetBinding(BlockMethod.InputProperty, GetBinding("Input", component));
            res.SetBinding(BlockMethod.OutputProperty, GetBinding("Output", component));
            return res;
        }
    }

    abstract class BlockMultiChildrenFactory : BlockFactoryBase
    {
        protected BlockPanel CreateChildPanel()
        {
            BlockPanel panel = new();
            panel.Children.Add(new BlockEmpty());
            return panel;
        }
    }

    class BlockCycleFactory : BlockMultiChildrenFactory
    {
        /// <summary>
        /// добавляет блок
        /// </summary>
        /// <param name="component">Связанный компонент из модели</param>
        /// <param name="root">Блок, после которого вставлять</param>
        /// <param name="panel">куда вставлять. Если равно null то берётся родительская панель root</param>
        public override BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel panel = null)
        {
            return base.AddBlock(component, root, panel);
        }

        public override BlockBase GetBlock(IBlockComponent component)
        {
            BlockCycle block = new();
            block.TrueSource = CreateChildPanel();
            block.TrueText = Resources.Resources.TrueTextBlock;
            block.FalseText = Resources.Resources.FalseTextBlock;
            return block;
        }

    }


    class BlockCustomCycleFactory:BlockCycleFactory
    {
        private BlockCycleTypes Type { get; init; }
        public BlockCustomCycleFactory(BlockCycleTypes type) => Type = type;

        public override BlockBase GetBlock(IBlockComponent component)
        {
            BlockCycle block =  (BlockCycle)base.GetBlock(component);
            block.TypeName = Type;
            return block;
        }

    }

    class BlockIfFactory : BlockCycleFactory
    {
        /// <summary>
        /// добавляет блок
        /// </summary>
        /// <param name="component">Связанный компонент из модели</param>
        /// <param name="root">Блок, после которого вставлять</param>
        /// <param name="panel">куда вставлять. Если равно null то берётся родительская панель root</param>
        public override BlockBase AddBlock(IBlockComponent component, BlockBase root, BlockPanel panel = null)
        {
            if (!(component is BlockIfComponent))
                throw new ArgumentException("ожидался компонент условия");
            return base.AddBlock(component, root, panel);

        }
        public override BlockBase GetBlock(IBlockComponent component)
        {
            BlockIf block = new BlockIf();
            block.TrueSource = CreateChildPanel();
            block.FalseSource = CreateChildPanel();
            block.TrueText = Resources.Resources.TrueTextBlock;
            block.FalseText = Resources.Resources.FalseTextBlock;
            return block;
        }
    }

    class BlockSwitchFactory : BlockFactoryBase
    {

        public override BlockBase GetBlock(IBlockComponent component)
        {
            SwitchBlock block = new();
            BlockSwitchComponent bsc = (BlockSwitchComponent)component;
            var dates = bsc.GetAllPossibleMetadates();
            foreach (var data in dates)
            {
                if (BlockSwitchComponentMetadata.keyRegex.IsMatch(data.GetProperty()))
                {
                    var item = bsc.GetComponentItem(data);
                    if (block.AddItem.CanExecute(item.Key))
                        block.AddItem.Execute(item.Key);
                }
            }
            block.Items.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    BlockSwitchComponent sw = (BlockSwitchComponent)component;

                    foreach (SwitchBlockItem item in e.NewItems)
                    {
                        BlockSwitchComponentMetadata metaData = new(item.Key);
                        component.Add(null, metaData);
                        BindingOperations.SetBinding(item, SwitchBlockItem.KeyProperty, GetBinding("Key", sw.GetComponentItem(metaData)));
                    }
                }
            };
            return block;
        }
    }

}
