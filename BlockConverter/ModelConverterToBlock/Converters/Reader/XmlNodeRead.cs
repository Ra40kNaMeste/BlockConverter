using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ModelConverterToBlock.Converters.Reader
{
    abstract class Singletone<T> where T : Singletone<T>
    {
        private static Lazy<T> type = new(() => Activator.CreateInstance(typeof(T), true) as T);
        public static T GetObject() => type.Value;
    }
    //Семейство для чтения свойств нодов контекста
    interface IContextNodeReaderable
    {
        public void Read(XmlNode node, PropertyContext context);
    }

    class PropertyContextNodeReader : Singletone<PropertyContextNodeReader>, IContextNodeReaderable
    {
        private PropertyContextNodeReader() { }
        public void Read(XmlNode node, PropertyContext context)
        {
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new PropertyData(name));
        }
    }
    class StretchPropertyContextNodeReader : Singletone<StretchPropertyContextNodeReader>, IContextNodeReaderable
    {
        public void Read(XmlNode node, PropertyContext context)
        {
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new StretchProptertyData(name));
        }
    }
    class OptionalContextNodeReader : Singletone<OptionalContextNodeReader>, IContextNodeReaderable
    {
        private OptionalContextNodeReader() { }
        public void Read(XmlNode node, PropertyContext context)
        {
            XmlNode props = node.GetChildNode("Properties");
            XmlNode chaines = node.GetChildNode("Chaines");
            List<CustomRegexByPattern> regexes = new();
            List<string> propsList = new();
            foreach (XmlNode item in props)
                if(item.Name == "Property")
                    propsList.Add(item.GetValueAttribute("Name"));
            foreach (XmlNode item in chaines)
            {
                if(item.Name == "Chain")
                {
                    Dictionary<string, string> findProps = new();
                    XmlNode findProperty = item.GetChildNode("Properties");
                    foreach (XmlNode prop in findProperty)
                    {
                        findProps.Add(prop.GetValueAttribute("Name"),
                           prop.GetValueAttribute("Value"));
                    }
                    regexes.Add(new CustomRegexByPattern(item.GetValueAttribute("Template"), context, findProps));
                }
            }
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name,
                new OptionalLink(name, regexes, Convert.ToBoolean(node.GetValueAttribute("IsOnlyOne"))));
        }
    }
    class BodyContextNodeReader : Singletone<BodyContextNodeReader>, IContextNodeReaderable
    {
        private BodyContextNodeReader() { }
        public void Read(XmlNode node, PropertyContext context)
        {

            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new BodyReader(name,
                NodeReaderOperations.ReadNodeWord(node, "Start"), NodeReaderOperations.ReadNodeWord(node, "End"), NodeReaderOperations.ReadNodeLinks(node, context, "Links", false)));
        }

    }
    class BlockTextToWordNodeReader : Singletone<BlockTextToWordNodeReader>, IContextNodeReaderable
    {
        private BlockTextToWordNodeReader() { }
        public void Read(XmlNode node, PropertyContext context)
        {
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new BlockTextToWordReader(name, NodeReaderOperations.ReadNodeWord(node, "EndWord"), 
                Convert.ToBoolean(node.GetValueAttribute("IsIncludeNextWord")), NodeReaderOperations.ReadNodeLinks(node, context, "Links", false)));
        }
    }
    class ByPatternNodeReader : Singletone<ByPatternNodeReader>, IContextNodeReaderable
    {
        public void Read(XmlNode node, PropertyContext context)
        {
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new ByPatternReader(name, node.GetValueAttribute("Pattern")));
        }
    }
    class ArrayParamsContextNodeReader : Singletone<ArrayParamsContextNodeReader>, IContextNodeReaderable
    {
        private ArrayParamsContextNodeReader() { }
        public void Read(XmlNode node, PropertyContext context)
        {
            string name = node.GetValueAttribute("Name");
            context.Context.Add(name, new ArrayParamsReader(name,                
                NodeReaderOperations.ReadNodeLinks(node, context, "Links"), node.GetValueAttribute("Separator")));
        }
    }
    internal static class NodeReaderOperations
    {
        public static LinkChainCollection ReadNodeLinks(XmlNode target, PropertyContext context, string name, bool isExceptionGenerate = true)
        {
            LinkChainCollection links = new();
            var content = target.GetValueAttribute(name, isExceptionGenerate);
            if(content == null)
                return null;
            ICollection<string> strs = StringInterprets.FindBlockLink(content);
            foreach (var item in strs)
                links.PushBack(StringInterprets.ConvertToLinkReader(item, context));
            return links;
        }
        public static WordFromLinksGetter ReadNodeWord(XmlNode target, string name)
        {
            XmlNode node;
            if ((node = target.GetChildNode(name, false)) != null)
                return new RegexWordFromLinksGetter(node.GetValueAttribute("Regex"));
            else
                return new CommonWordFromLinksGetter(target.GetValueAttribute(name));
        }
    }

}
