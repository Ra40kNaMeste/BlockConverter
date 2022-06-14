using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlocksLib.Blocks;
using BlocksLib.Blocks.Tools;
using ModelConverterToBlock.Blocks;
using BlockConverter.BlockFactorys;
using System.Windows.Controls;
using BlockConverter.ViewModels;

namespace BlockConverter.Tools
{
    /// <summary>
    /// Набор методов для визуализирования модели
    /// </summary>
    static class BlockModelToViewConverter
    {
        /// <summary>
        /// Рисует схему
        /// </summary>
        /// <param name="root">Модель схемы</param>
        /// <param name="scheme">Место для рисования</param>
        /// <returns></returns>
        public static ICollection<BlockBase> ConvertToView(IBlockComponent root, BlockSchemeFromViewItem scheme)
        {
            List<BlockBase> res = new();
            BlockPanel result;
            if (!(root is BlockComponent))
                throw new ArgumentException("головной блок не является точкой входа в программу");

            Type type = typeof(BeginBlockComponent);
            BlockFactoryBase factory = scheme.Factories.Where((i)=>i.Value.Item2.GetType() == type).First().Value.Item1;

            factory.SetSelectDictionary(scheme.SelectDictionary);
            BlockBase block = factory.AddBlock(root, null);
            res.Add(block);
            result = (BlockPanel)block.Parent;
            ConvertToViewRecurs(root.GetComponent(new BlockComponentMetadata()), block, result, scheme ,res);
            return res;
        }
        /// <summary>
        /// Вставляет элемент/группу элементов в схему
        /// </summary>
        /// <param name="component">Элемент, который необходимо вставить</param>
        /// <param name="targetPanel">Панель, куда вставлять</param>
        /// <param name="scheme">Схема для визуализации</param>
        /// <param name="parentBlock">После какого блока вставить</param>
        /// <returns></returns>
        public static ICollection<BlockBase> InsertBlockComponentToView(IBlockComponent component, BlockPanel targetPanel, BlockSchemeFromViewItem scheme, BlockBase parentBlock)
        {
            List<BlockBase> res = new();
            ConvertToViewRecurs(component, parentBlock, targetPanel, scheme, res);
            return res;
        }
        private static void ConvertToViewRecurs(IBlockComponent next, BlockBase block, BlockPanel panel, BlockSchemeFromViewItem targetScheme, ICollection<BlockBase> res)
        {
            if (next == null) return;
            BlockFactoryBase factory = ConvertToViewBlockFactory(next, targetScheme);
            factory.SetSelectDictionary(targetScheme.SelectDictionary);
            BlockBase nextBlock = factory.AddBlock(next, block, panel);
            res.Add(nextBlock);
            var dates = next.GetAllPossibleMetadates();
            foreach (var data in dates)
            {
                BlockPanel nextPanel = FindPanelByMetadata(nextBlock, data);
                if(nextPanel != null)
                    ConvertToViewRecurs(next.GetComponent(data), block, nextPanel, targetScheme, res);
                else
                    ConvertToViewRecurs(next.GetComponent(data), nextBlock, panel, targetScheme, res);
            }
        }

        /// <summary>
        /// Дочерняя панель по метаданным
        /// </summary>
        /// <param name="block">Блок, в котором ищем</param>
        /// <param name="metaData">Метаданные</param>
        /// <returns></returns>
        internal static BlockPanel FindPanelByMetadata(BlockBase block, BlockComponentMetadata metaData)
        {
            string prop = metaData.GetProperty();
            if (prop == BlockProperty.Next.ToString())
                return null;
            if (prop == BlockProperty.ChildBlocks.ToString() || prop == BlockIfChilds.TrueBlock.ToString())
                return ((BlockCycleBase)block).TrueSource;
            if (prop == BlockIfChilds.FalseBlock.ToString())
                return ((BlockIf)block).FalseSource;
            if (BlockSwitchComponentMetadata.keyRegex.IsMatch(prop))
                return ((SwitchBlock)block).Items.FirstOrDefault((i) => i.Key == BlockSwitchComponentMetadata.GetKey(prop)).Value;
            throw new ArgumentException("Не удалось найти панель в блоке");
        }

        private static BlockFactoryBase ConvertToViewBlockFactory(IBlockComponent component, BlockSchemeFromViewItem targetScheme)
        {
            try
            {
                Type type = component.GetType();
                VisualBlockTypes key = ParametersAllBlocksManager.FactoryStyles
                    .Where((i) => i.BlockComponent.GetType() == type).First().BlockType;
                return targetScheme.GetFactory(key).Item1;;
            }
            catch (Exception)
            {

                throw new ArgumentException("Тип блока не значится в реестре типов");
            }
        }

    }
}
