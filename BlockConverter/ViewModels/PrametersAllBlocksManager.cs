using BlocksLib.Blocks;
using ModelConverterToBlock.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlockConverter.ViewModels
{
    public enum VisualBlockTypes
    {
        Begin = BaseBlockTypes.BeginBlock,
        Operation = BaseBlockTypes.Block,
        Input = BaseBlockTypes.InputBlock,
        Output = BaseBlockTypes.OutputBlock,
        End = BaseBlockTypes.EndBlock,

        Cycle = BlockCycleTypes.BlockCycle,
        PostCycle = BlockCycleTypes.BlockPostCycle,
        For = BlockCycleTypes.BlockForCycle,

        Method,
        If,
        Switch
    }

    //Параметры для создания фабрики
    public sealed class FactoryParameters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя фабрики</param>
        /// <param name="block">Тип блока, создаваемого фабрикой</param>
        /// <param name="component">Тип компонента, для фабрики</param>
        /// <param name="customStyle">Имя стиля блока</param>
        /// <param name="isOnlyStyle">Обязателен ли стиль</param>
        internal FactoryParameters(VisualBlockTypes blockType, BlockBase block,
            IBlockComponent component, string customStyle, string defaultStyle, bool isOnlyStyle = false)
        {
            BlockType = blockType;
            BlockPrototype = block;
            BlockComponent = component;
            IsOnlyStyle = isOnlyStyle;
            CustomStyle = customStyle;
            DefaultStyle = defaultStyle;
        }

        public string DefaultStyle { get; set; }
        //Имя фабрики
        public VisualBlockTypes BlockType { get; init; }

        //Компонент, который фабрика создаёт
        public IBlockComponent BlockComponent { get; init; }

        //Обязан ли блок иметь стиль
        public bool IsOnlyStyle { get; set; }

        /// <summary>
        /// Создаёт прототип блока без привязок
        /// </summary>
        /// <returns></returns>
        public BlockBase GetPrototype()
        {
            BlockBase res = (BlockBase)BlockPrototype.Clone();
            res.SetResourceReference(FrameworkElement.StyleProperty, IsOnlyStyle ? CustomStyle : DefaultStyle);
            return res;
        }

        public BlockBase BlockPrototype { get; init; }

        //Имя кастомного стиля
        public string CustomStyle { get; init; }
    }


    public static class ParametersAllBlocksManager
    {
        private static Dictionary<Type, VisualBlockTypes> dicForConverter = new()
        {
            { typeof(BlockMethod), VisualBlockTypes.Method },
            { typeof(BlockIf), VisualBlockTypes.If },
            { typeof(SwitchBlock), VisualBlockTypes.Switch }
        };

        static ParametersAllBlocksManager() => FillFactoryStyles();


        //Заполняет библиотеку шаблонов фабрик
        private static void FillFactoryStyles()
        {
            string defaultStyleBlock = "BlockCustomStyle";
            string defaultStyleMultiChildBlock = "multiChildBlockBaseStyle";

            FactoryStyles = new()
            {
                new(VisualBlockTypes.Begin, new Block() { TypeName = BaseBlockTypes.BeginBlock }, new BeginBlockComponent(), "BeginCustomStyle", defaultStyleBlock, true),
                new(VisualBlockTypes.Operation, new Block() { TypeName = BaseBlockTypes.Block }, new BlockComponent(), "BlockCustomStyle", defaultStyleBlock),
                new(VisualBlockTypes.End, new Block() { TypeName = BaseBlockTypes.EndBlock }, new BlockEndComponent(), "EndCustomStyle", defaultStyleBlock, true),
                new(VisualBlockTypes.Input, new Block() { TypeName = BaseBlockTypes.InputBlock }, new BlockInputComponent(), "InputCustomStyle", defaultStyleBlock, true),
                new(VisualBlockTypes.Output, new Block() { TypeName = BaseBlockTypes.OutputBlock }, new BlockOutputComponent(), "OutputCustomStyle", defaultStyleBlock, true),

                new(VisualBlockTypes.Method, new BlockMethod(), new BlockMethodComponent(), "MethodBlockCustomStyle", defaultStyleBlock),
                new(VisualBlockTypes.If, new BlockIf(), new BlockIfComponent(), "IfBlockCustomStyle", defaultStyleMultiChildBlock),
                new(VisualBlockTypes.Switch, new SwitchBlock(), new BlockSwitchComponent(), "SwitchBlockCustomStyle", defaultStyleMultiChildBlock),

                new(VisualBlockTypes.Cycle, new BlockCycle() { TypeName = BlockCycleTypes.BlockCycle }, new BlockCycleComponent(), "CycleBlockCustomStyle", defaultStyleMultiChildBlock),
                new(VisualBlockTypes.PostCycle, new BlockCycle() { TypeName = BlockCycleTypes.BlockPostCycle }, new BlockPostCycleComponent(), "PostCycleBlockCustomStyle", defaultStyleMultiChildBlock, true),
                new(VisualBlockTypes.For, new BlockCycle() { TypeName = BlockCycleTypes.BlockForCycle }, new BlockForComponent(), "ForCycleBlockCustomStyle", defaultStyleMultiChildBlock, true),
            };
        }

        //Библиотека шаблонов фабрик
        public static List<FactoryParameters> FactoryStyles { get; private set; }

        public static VisualBlockTypes GetVisualBlockTypesByBlock(BlockBase block)
        {
            if (block is IBlockMultiType multiType)
            {
                object VisualTypes;
                string typeBlock = multiType.GetBlockType();
                if (Enum.TryParse(typeof(BaseBlockTypes), typeBlock, out VisualTypes))
                    return (VisualBlockTypes)VisualTypes;
                if (Enum.TryParse(typeof(BlockCycleTypes), typeBlock, out VisualTypes))
                    return (VisualBlockTypes)VisualTypes;
                throw new ArgumentException();
            }

            Type type = block.GetType();
            if (dicForConverter.ContainsKey(type))
                return dicForConverter[type];
            throw new ArgumentException();
        }
        public static VisualBlockTypes GetVisualBlockTypesByString(string str)
        {
            object result;
            if (Enum.TryParse(typeof(VisualBlockTypes), str, out result))
                return (VisualBlockTypes)result;
            throw new ArgumentException();
        }
        public static FactoryParameters GetElement(BlockBase block) => GetElement(GetVisualBlockTypesByBlock(block));

        public static FactoryParameters GetElement(VisualBlockTypes type) => FactoryStyles.Where(i => i.BlockType == type).FirstOrDefault();


        //Преобразует имя блока в имя стиля - динамического ресурса
        //public static string ConvertNameBlockToNameStyle(string name) => ConvertNameBlockToNameStyle(GetVisualBlockTypesByBlock())

        public static string ConvertNameBlockToNameStyle(VisualBlockTypes name) => FactoryStyles.Where((i) => i.BlockType == name).FirstOrDefault()?.CustomStyle;
    }
}
