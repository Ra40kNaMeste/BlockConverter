using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;

namespace ModelConverterToBlock.Blocks
{
    #region Metadates
    public class BlockComponentMetadata
    {
        /// <summary>
        /// Запрашиваемое свойство
        /// </summary>
        protected BlockProperty Property { get; init; }
        public virtual string GetProperty() => Property.ToString();
        public BlockComponentMetadata() => Property = BlockProperty.Next;
        /// <summary>
        /// Можно ли выбрать свойство
        /// </summary>
        /// <param name="property">Проверяемое свойство</param>
        /// <returns></returns>
        protected internal virtual bool IsInvalidProperty(string property) => property != BlockProperty.Next.ToString();
    }
    public class BlockCycleComponentMetadata : BlockComponentMetadata
    {
        public BlockCycleComponentMetadata() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property">Запрашиваемое свойство</param>
        public BlockCycleComponentMetadata(BlockProperty property)
        {
            if (IsInvalidProperty(property.ToString()))
                throw new ArgumentException("Нет такого свойства", property.ToString());
            Property = property;
        }
        /// <summary>
        /// Можно ли выбрать свойство
        /// </summary>
        /// <param name="property">Проверяемое свойство</param>
        /// <returns></returns>
        protected internal override bool IsInvalidProperty(string property) =>
            base.IsInvalidProperty(property) && property != BlockProperty.ChildBlocks.ToString();
    }
    public class BlockIfComponentMetadata : BlockCycleComponentMetadata
    {
        public BlockIfComponentMetadata() : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property">Запрашиваемое свойство</param>
        public BlockIfComponentMetadata(BlockIfChilds child) : base(BlockProperty.ChildBlocks)
        {
            ChildProperty = child;
        }
        public override string GetProperty() =>
            Property == BlockProperty.ChildBlocks ? ChildProperty.ToString() : Property.ToString();
        protected internal BlockIfChilds ChildProperty { get; init; }
        /// <summary>
        /// Можно ли выбрать свойство
        /// </summary>
        /// <param name="property">Проверяемое свойство</param>
        /// <returns></returns>
        protected internal override bool IsInvalidProperty(string property) =>
            base.IsInvalidProperty(property) && !Enum.IsDefined(typeof(BlockIfChilds), property);
    }
    public class BlockSwitchComponentMetadata : BlockCycleComponentMetadata
    {
        public readonly static Regex keyRegex = new(@"^Key[.]");
        public BlockSwitchComponentMetadata() : base() { }
        public BlockSwitchComponentMetadata(string key) : base(BlockProperty.ChildBlocks)
        {
            this.key = key;
        }
        private string key;
        public override string GetProperty()
        {
            return Property == BlockProperty.ChildBlocks ? "Key." + key : Property.ToString();
        }
        protected internal override bool IsInvalidProperty(string property)
        {
            return false;
            //return base.IsInvalidProperty(property) || !keyRegex.IsMatch(property);
        }
        public static string GetKey(string property)
        {
            return property.Substring(4);
        }
    }
    #endregion //Metadates

    #region BlockComponents

    /// <summary>
    /// Интерфейс блочного компонента
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public interface IBlockComponent : ICloneable
    {
        [JsonProperty()]
        public string Content { get; set; }
        /// <summary>
        /// Добавление дочернего компонента
        /// </summary>
        /// <param name="component">дочерний компонент</param>
        /// <param name="metadata">родительского элемента</param>
        public void Add(IBlockComponent component, BlockComponentMetadata metadata);
        /// <summary>
        /// Удаляет дочерний элемент
        /// </summary>
        /// <param name="metadata">параметры родительского элемента</param>
        public void Remove(BlockComponentMetadata metadata);
        /// <summary>
        /// Компонент
        /// </summary>
        /// <param name="metadata">Параметры родительского блока</param>
        /// <returns></returns>
        public IBlockComponent GetComponent(BlockComponentMetadata metadata);
        /// <summary>
        /// Выдаёт все возможные метаданные
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates();
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockEndComponent : IBlockComponent
    {
        public BlockEndComponent()
        {
            Content = "";
        }
        [JsonProperty()]
        public string Content { get; set; }
        /// <summary>
        /// Нету
        /// </summary>
        /// <param name="component"></param>
        /// <param name="metadata"></param>
        public void Add(IBlockComponent component, BlockComponentMetadata metadata)
        {
            //throw new NotImplementedException();
        }

        public object Clone()
        {
            return new BlockEndComponent() { Content = Content };
        }

        private static IEnumerable<BlockComponentMetadata> Possible = new List<BlockComponentMetadata>() { new BlockComponentMetadata() };
        public IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates()
        {
            return Possible;
        }



        /// <summary>
        /// нету
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IBlockComponent GetComponent(BlockComponentMetadata metadata)
        {
            return null;
        }
        /// <summary>
        /// нету
        /// </summary>
        /// <param name="metadata"></param>
        public void Remove(BlockComponentMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockComponent : IBlockComponent
    {
        public BlockComponent()
        {
            Content = "";
        }
        [JsonProperty()]
        public string Content { get; set; }
        [JsonProperty()]
        protected IBlockComponent Next { get; set; }
        /// <summary>
        /// Добавление дочернего компонента
        /// </summary>
        /// <param name="component">дочерний компонент</param>
        /// <param name="metadata">родительского элемента</param>
        public virtual void Add(IBlockComponent component, BlockComponentMetadata metadata)
        {
            Next = component;
        }

        public virtual object Clone()
        {
            IBlockComponent block = (IBlockComponent)GetType().GetConstructor(new Type[] { }).Invoke(new object[] { });
            block.Content = Content;
            return block;
        }

        public virtual IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates()
        {
            return new List<BlockComponentMetadata>() { new BlockComponentMetadata() };
        }


        /// <summary>
        /// Конмонент
        /// </summary>
        /// <param name="metadata">Параметры родительского блока</param>
        /// <returns></returns>
        public virtual IBlockComponent GetComponent(BlockComponentMetadata metadata)
        {
            BlockComponentMetadata data = new BlockComponentMetadata();
            if (data.IsInvalidProperty(metadata.GetProperty()))
                throw new ArgumentException("Нету такого свойства");
            return Next;
        }
        /// <summary>
        /// Удаляет дочерний элемент
        /// </summary>
        /// <param name="metadata">Можно не передавать</param>
        public virtual void Remove(BlockComponentMetadata metadata)
        {
            Next = null;
        }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BeginBlockComponent : BlockComponent
    {
        [JsonProperty]
        public string Name { get; set; }
        public override object Clone()
        {
            BeginBlockComponent res = (BeginBlockComponent)base.Clone();
            res.Name = Name;
            return res;
        }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockInputComponent : BlockComponent { }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockOutputComponent : BlockComponent { }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockMethodComponent : BlockComponent
    {
        public BlockMethodComponent() : base()
        {
            Input = "";
            Output = "";
        }
        [JsonProperty]
        public string Input { get; set; }
        [JsonProperty]
        public string Output { get; set; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockCycleComponent : BlockComponent
    {
        public BlockCycleComponent() : base() { }
        [JsonProperty()]
        protected IBlockComponent TrueBlock { get; set; }


        /// <summary>
        /// Выдаёт имя свойства для обращения
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected virtual string ConvertToPropertyName(BlockComponentMetadata metadata)
        {
            string property = metadata.GetProperty();
            if (property == BlockProperty.ChildBlocks.ToString())
                return "TrueBlock";
            return property;
        }
        protected PropertyInfo ConvertToProperty(BlockComponentMetadata metaData)
        {
            return GetType().GetProperty(ConvertToPropertyName(metaData), BindingFlags.Instance | BindingFlags.NonPublic) ??
                throw new ArgumentException("Не удалось прочитать свойство");
        }

        /// <summary>
        /// Добавление дочернего компонента
        /// </summary>
        /// <param name="component">дочерний компонент</param>
        /// <param name="metadata">родительского элемента</param>
        public override void Add(IBlockComponent component, BlockComponentMetadata metadata)
        {
            ConvertToProperty(metadata).SetValue(this, component);
        }
        /// <summary>
        /// Конмонент
        /// </summary>
        /// <param name="metadata">Параметры родительского блока</param>
        /// <returns></returns>
        public override IBlockComponent GetComponent(BlockComponentMetadata metadata)
        {
            return (IBlockComponent)ConvertToProperty(metadata).GetValue(this);
        }

        /// <summary>
        /// Удаляет дочерний элемент
        /// </summary>
        /// <param name="metadata">параметры родительского элемента</param>
        public override void Remove(BlockComponentMetadata metadata)
        {
            PropertyInfo property = ConvertToProperty(metadata);
            property.SetValue(this, null);
        }
        public override IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates()
        {
            return new List<BlockComponentMetadata>()
            {
                new BlockComponentMetadata(),
                new BlockCycleComponentMetadata(BlockProperty.ChildBlocks)
            };
        }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockPostCycleComponent : BlockCycleComponent
    {
        public BlockPostCycleComponent() : base() { }
    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockForComponent : BlockCycleComponent
    {
        public BlockForComponent() : base() { }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockIfComponent : BlockCycleComponent
    {

        public BlockIfComponent() : base() { }
        [JsonProperty()]
        protected IBlockComponent FalseBlock { get; set; }
        public override IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates()
        {
            return new List<BlockComponentMetadata>()
            {
                new BlockComponentMetadata(),
                new BlockCycleComponentMetadata(BlockProperty.ChildBlocks),
                new BlockIfComponentMetadata(BlockIfChilds.FalseBlock)
            };
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockSwitchComponent : BlockComponent
    {
        public BlockSwitchComponent() : base()
        {
            Childs = new();
        }

        [JsonProperty()]
        protected List<BlockSwitchComponentItem> Childs { get; set; }

        public override void Add(IBlockComponent component, BlockComponentMetadata metadata)
        {
            string prop = metadata.GetProperty();

            if (BlockSwitchComponentMetadata.keyRegex.IsMatch(prop))
            {
                BlockSwitchComponentItem item;
                if ((item = GetComponentItem(metadata)) != null)
                    item.Component = component;
                else
                    Childs.Add(new BlockSwitchComponentItem() { Key = BlockSwitchComponentMetadata.GetKey(prop), Component = component });
                return;
            }
            if (prop != BlockProperty.Next.ToString())
                throw new ArgumentException("Нету такого свойства");
            Next = component;
        }


        public override object Clone()
        {
            var res = (BlockSwitchComponent)base.Clone();
            res.Childs = new();
            return res;
        }

        public override IBlockComponent GetComponent(BlockComponentMetadata metadata)
        {
            string prop = metadata.GetProperty();
            BlockSwitchComponentItem item;
            if ((item = GetComponentItem(metadata)) != null)
                return item.Component;
            if (prop != BlockProperty.Next.ToString())
                throw new ArgumentException("Нету такого свойства");
            return Next;
        }

        public BlockSwitchComponentItem GetComponentItem(BlockComponentMetadata metadata)
        {
            string prop = metadata.GetProperty();
            if (BlockSwitchComponentMetadata.keyRegex.IsMatch(prop))
            {
                prop = BlockSwitchComponentMetadata.GetKey(prop);
                return Childs.Find((i) => i.Key == prop);
            }
            return null;
        }

        public override void Remove(BlockComponentMetadata metadata)
        {
            string prop = metadata.GetProperty();

            if (BlockSwitchComponentMetadata.keyRegex.IsMatch(prop))
            {
                prop = BlockSwitchComponentMetadata.GetKey(prop);
            }
            if (prop != BlockProperty.Next.ToString())
                throw new ArgumentException("Нету такого свойства");
            Next = null;
        }
        public override IEnumerable<BlockComponentMetadata> GetAllPossibleMetadates()
        {
            var res = base.GetAllPossibleMetadates().ToList();
            foreach (var item in Childs)
                res.Add(new BlockSwitchComponentMetadata(item.Key));
            return res;
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockSwitchComponentItem
    {
        [JsonProperty()]
        public string Key { get; set; }
        [JsonProperty()]
        public IBlockComponent Component { get; set; }
    }
    #endregion //BlockComponents
    /// <summary>
    /// Операции над блоками
    /// </summary>
    public static class BlockComponentOperations
    {
        /// <summary>
        /// вырезает следующий блок
        /// </summary>
        /// <param name="parent">блок после которого необходимо вырезать</param>
        /// <param name="metaData">параметры для вырезания</param>
        public static void RemoveBlock(IBlockComponent parent, BlockComponentMetadata metaData)
        {
            IBlockComponent next = parent.GetComponent(metaData)?.GetComponent(new BlockComponentMetadata());
            parent.Add(next, metaData);
        }
        /// <summary>
        /// Вставляет блок или группу блоков
        /// </summary>
        /// <param name="parent">после этого блока вставляет</param>
        /// <param name="child">что вставляет</param>
        /// <param name="metaData">параметры вставки</param>
        public static void InsertBlock(IBlockComponent parent, IBlockComponent child, BlockComponentMetadata metaData)
        {
            IBlockComponent lasted = child, temp;
            while ((temp = lasted.GetComponent(new BlockComponentMetadata())) != null)
                lasted = temp;
            lasted.Add(parent.GetComponent(metaData), new BlockComponentMetadata());
            parent.Add(child, metaData);
        }
        public const string format = ".blocks";
        /// <summary>
        /// Сохраняет блоки по указанному адресу
        /// </summary>
        /// <param name="path">адрес</param>
        /// <param name="component">Имя сохраняемого компонента</param>
        public static void Save(string path, IBlockComponent component)
        {
            string objects = JsonConvert.SerializeObject(component, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            if (!path.EndsWith(format))
                path += format;
            using StreamWriter sw = new StreamWriter(path, false);
            sw.WriteLine(objects);
        }
        /// <summary>
        /// Читает блок-схему из файла
        /// </summary>
        /// <param name="path">адрес</param>
        /// <returns>Блок-схема</returns>
        public static IBlockComponent Read(string path)
        {
            string res;
            using (StreamReader sr = new StreamReader(path))
            {
                res = sr.ReadToEnd();
                sr.Close();
            }
            return Deserialize(res);
        }
        public static IBlockComponent Deserialize(string obj)
        {
            return (IBlockComponent)JsonConvert.DeserializeObject(obj, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Клонирует участок блок-схемы
        /// </summary>
        /// <param name="start">Начальный элемент</param>
        /// <returns>Клон</returns>
        public static IBlockComponent CloneTree(IBlockComponent start) => CloneTree(start, null);
        /// <summary>
        /// Клонирует участок блок-схемы
        /// </summary>
        /// <param name="startComponent">Начальный элемент</param>
        /// <param name="endComponent">Конечный элемент</param>
        /// <returns>Клон</returns>
        public static IBlockComponent CloneTree(IBlockComponent startComponent, IBlockComponent endComponent)
        {
            IBlockComponent res = CloneWithChilds(startComponent);
            IBlockComponent current, oldCloneChild, newCloneChild;
            oldCloneChild = res;
            current = startComponent;
            while (current != endComponent && (current = current.GetComponent(new BlockComponentMetadata())) != null)
            {
                newCloneChild = CloneWithChilds(current);
                oldCloneChild.Add(newCloneChild, new());
                oldCloneChild = newCloneChild;
            }
            return res;
        }
        /// <summary>
        /// Клонирует 1 блок со всеми ответвлениями
        /// </summary>
        /// <param name="component">Блок</param>
        /// <returns>Копия</returns>
        public static IBlockComponent CloneWithChilds(IBlockComponent component)
        {
            IBlockComponent res = (IBlockComponent)component.Clone();
            var childs = component.GetAllPossibleMetadates();
            string nextPropertyString = new BlockComponentMetadata().GetProperty();
            foreach (var child in childs)
            {
                if (child.GetProperty() == nextPropertyString)
                    continue;
                IBlockComponent childComponent;
                if ((childComponent = component.GetComponent(child)) != null)
                    res.Add(CloneWithChildsRecurs(childComponent), child);
            }
            return res;
        }
        private static IBlockComponent CloneWithChildsRecurs(IBlockComponent component)
        {
            IBlockComponent res = (IBlockComponent)component.Clone();
            var childs = component.GetAllPossibleMetadates();
            foreach (var child in childs)
            {
                IBlockComponent childComponent;
                if ((childComponent = component.GetComponent(child)) != null)
                    res.Add(CloneWithChildsRecurs(childComponent), child);
            }
            return res;
        }
    }
    
}
