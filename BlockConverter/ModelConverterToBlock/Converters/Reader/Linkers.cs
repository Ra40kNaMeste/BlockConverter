using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    //наследники этого класса могут интрепретировать текст. 
    abstract class LinkReaderDataBase : ICloneable
    {
        public LinkReaderDataBase Succesor { get; set; }
        protected internal abstract Regex GetRegex(CustomRegexLinkData data);
        protected internal abstract void Interpret(CustomRegexLinkData data);
        protected Match Replace(CustomRegexLinkData data)
        {
            Regex regex = GetRegex(data);
            Match match = regex.Match(data.Root, data.GetStartIndex(data.Length), data.Length);
            if (match.Index != data.GetStartIndex(match.Value.Length))
            {
                data.IsSuccuss = false;
                return match;
            }
            data.AddMatch(match);
            if (data.IsSuccuss && Succesor != null)
                Succesor.Interpret(data);
            return match;
        }

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }
    class Skiper : LinkReaderDataBase
    {
        public Skiper() { }

        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return new Regex("", data.GetOptions());
        }

        protected internal override void Interpret(CustomRegexLinkData data) { }
    }
    class WordSkiper : LinkReaderDataBase
    {
        public WordSkiper() { }

        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return new Regex(@"\w*", data.GetOptions());
        }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Replace(data);
        }
    }
    class NotWordSkiper : LinkReaderDataBase
    {
        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return new Regex(@"\W*", data.GetOptions());
        }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Replace(data);
        }
    }
    class SpaceSkiper : LinkReaderDataBase
    {
        private bool isSkipOneSpace;
        public SpaceSkiper(bool isSkipOneSpace) => this.isSkipOneSpace = isSkipOneSpace;

        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            string pattern = isSkipOneSpace ? @"\s" : @"\s*";
            return new Regex(pattern, data.GetOptions());
        }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Replace(data);
        }
    }
    //имеет свойство Name. Участвует в составлении прочитанных свойств
    abstract class PropertyDataBase:LinkReaderDataBase
    {
        public PropertyDataBase(string name)
        {
            Name = name;
        }
        protected string Name { get; init; }
    }
    class PropertyData : PropertyDataBase
    {
        public PropertyData(string nameProp) : base(nameProp) { }
        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Match match = Replace(data);
            if (match.Length == 0)
                data.IsSuccuss = false;
            if (data.IsSuccuss)
                data.Properties.Properties.Add(new RegexProperty(Name, match.Index, match.Value));
        }
        public override object Clone()
        {
            return new PropertyData(Name) { Succesor = Succesor };
        }

        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return new Regex(@"(\w|[.])*", data.GetOptions());
        }
    }

    class StretchProptertyData : PropertyDataBase
    {
        public StretchProptertyData(string name) : base(name) { }
        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            throw new NotImplementedException();
        }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            int lenght = data.Length;
            data.Properties.Properties.Add(new RegexProperty(Name,
                data.GetStartIndex(lenght), data.Root.Substring(data.GetStartIndex(lenght), lenght)));
            data.Addition(lenght);
        }
        public override object Clone()
        {
            return new StretchProptertyData(Name) { Succesor = Succesor };
        }
    }
    
    class ByPatternReader : PropertyData
    {
        public ByPatternReader(string name, string pattern) : base(name)
        {
            this.pattern = pattern;
        }
        private string pattern;
        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return new Regex(pattern, data.GetOptions());
        }
        public override object Clone()
        {
            return new ByPatternReader(Name, pattern);
        }
    }

    enum Direction
    {
        Right, Left
    }
    //Те, что реализуют этот интерфейс, могут выдать ключевое слово.
    interface IFindRootWordable
    {
        public WordFromLinksGetter GetRootWord();
        public Direction DirectionFromWord { get; }
    }

    class WordReader : LinkReaderDataBase, IFindRootWordable
    {
        public WordFromLinksGetter Word { get; init; }

        public Direction DirectionFromWord { get { return Direction.Right; } }

        public WordReader(WordFromLinksGetter word) => Word = word;
        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Replace(data);
        }

        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            return Word.GetRegex(data.GetOptions());
        }

        public WordFromLinksGetter GetRootWord()
        {
            return Word;
        }
    }

    abstract class BodyReaderBase : PropertyData
    {
        public BodyReaderBase(string nameProp, LinkChainCollection links = null) : base(nameProp)
        {
            Links = links;
        }
        protected LinkChainCollection Links { get; init; }
        public IRegexProperty InterpretBody(string name, CustomRegexLinkData rootData)
        {
            RightLinkData data = rootData.ConvertToRightLinkData();
            if (Links == null)
                return new RegexProperty(name, data.GetStartIndex(0), data.TargetString);
            Links.Interpret(data);
            rootData.IsSuccuss = data.IsSuccuss;
            return new RegexPropertyArray(name, data.Properties.Properties);
        }
        public override object Clone()
        {
            return base.Clone();
        }
    }

    class BlockTextToWordReader : BodyReaderBase, IFindRootWordable
    {
        public BlockTextToWordReader(string name, WordFromLinksGetter endWord, bool isIncludeNextWord, LinkChainCollection links = null) : base(name, links)
        {
            EndWord = endWord;
            this.isIncludeNextWord = isIncludeNextWord;
        }
        private readonly bool isIncludeNextWord;
        protected internal override void Interpret(CustomRegexLinkData data)
        {
            Regex endRegex = EndWord.GetRegex(data.GetOptions());
            Match match = endRegex.Match(data.Root, data.GetStartIndex(data.Length), data.Length);
            int length = match.Success ? Math.Abs(data.Index - match.Index) : data.Length;
            int startBodyIndex = data.GetStartIndex(data.Length);
            string body = data.Root.Substring(startBodyIndex, length);

            if (data.GetStartIndex(length) == startBodyIndex)
            {
                if (isIncludeNextWord)
                    body += EndWord;
                CustomRegexLinkData dataBody = new RightLinkData(data.Root, startBodyIndex, body.Length);
                data.Properties.Properties.Add(InterpretBody(Name, dataBody));
                data.IsSuccuss = dataBody.IsSuccuss;

                data.Addition(Math.Min(data.Length, length + match.Value.Length));

                if (data.IsSuccuss)
                    Succesor?.Interpret(data);
            }
            else
                data.IsSuccuss = false;

        }
        public override object Clone()
        {
            return new BlockTextToWordReader(Name, EndWord, isIncludeNextWord, Links) { Succesor = Succesor };
        }
        public WordFromLinksGetter EndWord { get; init; }

        public Direction DirectionFromWord { get { return Direction.Left; } }

        public WordFromLinksGetter GetRootWord()
        {
            return EndWord;
        }
    }

    class BodyReader : BodyReaderBase, IFindRootWordable
    {
        public BodyReader(string name, WordFromLinksGetter start, WordFromLinksGetter end, LinkChainCollection links = null) : base(name, links)
        {
            End = end;
            Start = start;
            startRegex = Start.GetRegex(RegexOptions.None);
            endRegex = End.GetRegex(RegexOptions.None);
        }
        public WordFromLinksGetter End { get; init; }
        public WordFromLinksGetter Start { get; init; }

        public Direction DirectionFromWord { get { return Direction.Right; } }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            string body = GetBodyAndReplace(data, out int startIndex);
            RightLinkData dataBody = new(data.Root, startIndex, body.Length);

            data.Properties.Properties.Add(InterpretBody(Name, dataBody));
            data.IsSuccuss &= dataBody.IsSuccuss;
        }
        protected string GetBodyAndReplace(CustomRegexLinkData data, out int startIndex)
        {
            if (Start.GetRegex(data.GetOptions()).Match(data.Root, data.Index).Index != data.GetStartIndex(Start.ToString().Length))
            {
                data.IsSuccuss = false;
                startIndex = data.Index;
                return "";
            }
            startIndex = data.GetStartIndex(Start.ToString().Length) + Start.ToString().Length;
            string body = StringInterprets.GetBody(data, startRegex, endRegex);
            data.Addition(body.Length + Start.ToString().Length + End.ToString().Length);
            if (data.IsSuccuss)
                Succesor?.Interpret(data);
            return body;
        }
        protected Regex endRegex;
        protected Regex startRegex;
        public override object Clone()
        {
            return new BodyReader(Name, Start, End, Links) { Succesor = Succesor };
        }

        public WordFromLinksGetter GetRootWord()
        {
            return Start;
        }
    }

    class ArrayParamsReader : PropertyData
    {
        public ArrayParamsReader(string name, LinkChainCollection links, string separator) : base(name)
        {
            Links = links;
            Separator = separator;
            separatorRegex = new(Regex.Escape(separator));
        }
        public LinkChainCollection Links { get; init; }
        public string Separator { get; init; }
        private Regex separatorRegex;
        protected internal override void Interpret(CustomRegexLinkData data)
        {
            CustomRegexLinkData bodyData = data.ConvertToRightLinkData();
            var items = DivideBySeparator(bodyData, separatorRegex);
            bool isSuccuss = true;
            foreach (var item in items)
            {
                Links.Interpret(item);
                if (!(isSuccuss = item.IsSuccuss))
                    break;
                foreach (var property in item.Properties.Properties)
                    bodyData.Properties.Properties.Add(property);
            }
            bodyData.Properties.UpdateSection();
            data.Addition(data.Length);
            data.IsSuccuss = isSuccuss;
            data.Properties.Properties.Add(new RegexPropertyArray(Name, bodyData.Properties.Properties));
        }
        private static IEnumerable<CustomRegexLinkData> DivideBySeparator(CustomRegexLinkData rootData, Regex separator)
        {
            RightLinkData rightData = rootData.ConvertToRightLinkData();
            MatchCollection matches = separator.Matches(rightData.TargetString);
            int startIndex = 0, endIndex;
            List<RightLinkData> res = new();
            int startRootIndex = rightData.GetStartIndex(0);
            foreach (Match match in matches)
            {
                endIndex = match.Index;
                res.Add(new RightLinkData(rightData.Root, startIndex + startRootIndex, endIndex - startIndex));
                startIndex = endIndex + match.Length;
            }
            endIndex = rightData.Length;
            res.Add(new RightLinkData(rightData.Root, startIndex + startRootIndex, endIndex - startIndex));
            return res;
        }
        public override object Clone()
        {
            return new ArrayParamsReader(Name, Links, Separator) { Succesor = Succesor };
        }
    }

    //представляет собой цепочку из LinkReaderDataBase - элементов
    public class LinkChainCollection
    {
        public int Lenght
        {
            get
            {
                LinkReaderDataBase link = HeadLink;
                int lenght = 0;
                while (link != null)
                {
                    link = link.Succesor;
                    lenght++;
                }
                return lenght;
            }
        }
        private LinkReaderDataBase headLink;
        private LinkReaderDataBase HeadLink
        {
            get { return headLink; }
            set
            {
                headLink = lastLink = value;
                while (lastLink.Succesor != null)
                    lastLink = lastLink.Succesor;
            }
        }
        private LinkReaderDataBase lastLink;
        internal void Interpret(CustomRegexLinkData data) => HeadLink?.Interpret(data);
        internal void PushBack(LinkReaderDataBase link)
        {
            if (HeadLink == null)
                HeadLink = link;
            else
            {
                lastLink.Succesor = link;
                lastLink = link;
            }
        }
        internal void PushFront(LinkReaderDataBase link)
        {
            if (HeadLink == null)
                HeadLink = link;
            else
            {
                link.Succesor = headLink;
                headLink = link;
            }
        }

    }

}
