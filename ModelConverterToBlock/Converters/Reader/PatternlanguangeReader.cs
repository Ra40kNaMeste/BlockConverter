using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ModelConverterToBlock.Blocks;

namespace ModelConverterToBlock.Converters.Reader
{
    public static class PatternLaguangeList
    {
        static PatternLaguangeList()
        {
            Instructions = new();
        }
        public static void ReadInstruactions()
        {
            string[] allPaths = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataCode"));
            Instructions = new();
            foreach (var item in allPaths)
            {
                FileInfo file = new FileInfo(item);
                if (file.Exists && file.Extension == ".xml")
                    Instructions.Add(new FileInfo(item));
            }

        }
        public static List<FileInfo> Instructions { get; set; }
    }
    /// <summary>
    /// Читает из файла интсрукции
    /// </summary>
    public class PatternlanguangeReader
    {
        //прочитать из файла
        public void Read(string path)
        {
            XmlDocument document = new();
            document.Load(path);
            PropertyContext context = Interpret(document);

            Context = context;
        }
        public void Read(Uri path)
        {
            Read(path.ToString());
        }
        private static PropertyContext Interpret(XmlDocument document)
        {
            PropertyContext context = new();
            XmlNodeList dates = document.DocumentElement.ChildNodes;
            List<XmlNode> constructions = new();
            foreach (XmlNode item in dates)
            {
                string name = item.GetValueAttribute("Name");
                if (name == "Context")
                    FillContext(item, context);
                else
                    constructions.Add(item);
            }
            foreach (var item in constructions)
                FillConstruction(item, context);
            return context;
        }
        private static void FillConstruction(XmlNode constructionNode, PropertyContext context)
        {
            BlockConstruction construction = ConvertToBlockConstruction(constructionNode.GetValueAttribute("Name"));
            XmlNodeList nodes = constructionNode.GetChildNode("Realizations").ChildNodes;
            if (!context.Constructions.ContainsKey(construction))
                context.Constructions.Add(construction, new List<CustomRegexByPattern>());
            List<CustomRegexByPattern> regexes = context.Constructions[construction];
            foreach (XmlNode item in nodes)
            {
                if (item.Name != "Realization")
                    throw new XmlFileReadException(item.Name + " не является реализацией");
                Dictionary<string, string> AlteredPattern = GetBlockProperties(item);
                Dictionary<string, string> pattern = AlteredPattern.Concat(GetBlockProperties(constructionNode)
                    .Where((i) => !AlteredPattern.ContainsKey(i.Key))).ToDictionary((i) => i.Key.ToString(), (i) => i.Value);
                regexes.Add(new CustomRegexByPattern(item.GetValueAttribute("Template"), context, pattern));
            }
        }
        private static void FillContext(XmlNode contextNode, PropertyContext context)
        {
            XmlNodeList nodes = contextNode.ChildNodes;
            List<XmlNode> listNodes = new();
            foreach (XmlNode item in nodes)
                listNodes.Add(item);
            IEnumerable<XmlNode> listNodesSort = listNodes.OrderBy((i) => Convert.ToInt32(i.GetValueAttribute("InizLevel", false)));

            foreach (var item in listNodesSort)
                StringInterprets.ConvertToNodeReader(item.Name).Read(item, context);
        }
        private static Dictionary<string, string> GetBlockProperties(XmlNode node)
        {
            XmlAttributeCollection attrs = node.Attributes;
            Dictionary<string, string> res = new();
            foreach (XmlAttribute attr in attrs)
            {
                if(attr.Name!="Name" && attr.Name!="Template")
                    res.Add(attr.Name, attr.Value);
            }
            return res;
        }
        private static BlockConstruction ConvertToBlockConstruction(string str)
        {
            try
            {
                return (BlockConstruction)Enum.Parse(typeof(BlockConstruction), str);
            }
            catch
            {
                return BlockConstruction.None;
            }
        }
        public PropertyContext Context
        {
            get;
            private set;
        }
    }

    public class PropertyContext
    {
        public PropertyContext()
        {
            Context = new();
            Constructions = new();
        }
        internal Dictionary<string, LinkReaderDataBase> Context { get; init; }
        internal Dictionary<BlockConstruction, List<CustomRegexByPattern>> Constructions { get; init; }

    }

    static class XmlNodeExtension
    {
        public static string GetValueAttribute(this XmlNode node, string attr, bool isExceptionGeneration = true)
        {
            var res = node.Attributes.GetNamedItem(attr);
            if (res == null)
                if (isExceptionGeneration)
                    throw new XmlFileReadException("Не удалось найти свойство в " + node.Name, attr);
                else
                    return null;
            return res.Value;
        }
        public static XmlNode GetChildNode(this XmlNode target, string name, bool isExceptionGeneration = true)
        {
            var childs = target.ChildNodes;
            foreach (XmlNode item in childs)
                if (item.Name == name)
                    return item;
            return isExceptionGeneration ? throw new XmlFileReadException("Не удалось найти дочерний нод в " + target.Name, name) : null;
        }
    }
    public class XmlFileReadException : Exception
    {
        public XmlFileReadException() { }
        public XmlFileReadException(string message) : base(message) { }
        public string NameProperty { get; init; }
        public XmlFileReadException(string message, string nameProperty) : this(message) => NameProperty = nameProperty;
    }
}
