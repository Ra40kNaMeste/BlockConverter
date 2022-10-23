using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    //Все найденные свойства CustomRegex`ом
    public interface IRegexProperty
    {
        public int StartIndex { get; }
        public int Length { get; }
        public int EndIndex { get; }
        public string Name { get; }
        public ICollection<IRegexProperty> GetProperties();
    }

    public class RegexPropertyValue : IRegexProperty
    {
        public RegexPropertyValue(string name, int startIndex)
        {
            Name = name;
            StartIndex = startIndex;
        }
        public string Name { get; init; }
        public int StartIndex { get; init; }
        public int Length { get { return Name.Length; } }

        public int EndIndex { get { return StartIndex + Name.Length; } }

        public ICollection<IRegexProperty> GetProperties()
        {
            return new List<IRegexProperty>();
        }
    }
    public class RegexProperty : IRegexProperty
    {
        public RegexProperty(string name, int startIndex, string property)
        {
            Name = name;
            Property = new RegexPropertyValue(property, startIndex);
        }
        public RegexProperty(string name, IRegexProperty property)
        {
            Name = name;
            Property = property;
        }
        public IRegexProperty Property { get; init; }

        public int StartIndex
        {
            get { return Property.StartIndex; }
        }

        public int Length { get { return Property.Length; } }

        public string Name { get; init; }

        public int EndIndex { get { return Property.EndIndex; } }

        public ICollection<IRegexProperty> GetProperties()
        {
            return new List<IRegexProperty>() { Property };
        }
    }
    public class RegexPropertyArray : IRegexProperty, IEnumerable<IRegexProperty>
    {
        public RegexPropertyArray(string name, ICollection<IRegexProperty> properties)
        {
            Name = name;
            Properties = properties;
        }
        private int startCoor;
        public int StartCorr
        {
            get { return startCoor; }
            set { startCoor = Math.Abs(value); UpdateSection(); }
        }
        private int endCorr;
        public int EndCorr
        {
            get { return endCorr; }
            set { endCorr = Math.Abs(value); UpdateSection(); }
        }
        public void UpdateSection()
        {
            if (Properties.Count != 0)
            {
                var coll = Properties.Where((i) => i.StartIndex >= 0 && i.Length >= 0);
                if (coll.Count() == 0)
                {
                    StartIndex = -1 + StartCorr;
                    Length = -1 + EndCorr - StartIndex;
                    return;
                }
                    
                int minIndex = coll.Where((i)=>i.StartIndex>=0).Min((i) => i.StartIndex) - StartCorr,
                    maxIndex = Properties.Max((i) => i.StartIndex + i.Length) + EndCorr;
                StartIndex = minIndex;
                Length = maxIndex - minIndex;
            }
            else
            {
                StartIndex = -1 + StartCorr;
                Length = -1 + EndCorr - StartIndex;
            }

        }
        private ICollection<IRegexProperty> properties;
        public ICollection<IRegexProperty> Properties 
        {
            get { return properties; }
            set { properties = value; UpdateSection(); }
        }

        public int StartIndex { get; private set; }

        public int Length { get; private set; }

        public string Name { get; init; }

        public int EndIndex { get { return StartIndex + Length; } }

        public ICollection<IRegexProperty> GetProperties()
        {
            return Properties;
        }
        public RegexPropertyArray AbjustStartCorr(int currentStart)
        {
            StartCorr = 0;
            UpdateSection();
            if (currentStart > StartIndex && StartIndex >= 0)
                throw new ArgumentOutOfRangeException("положение для корректировки не может превышать минимальный стартовый индекс свойств");
            StartCorr = StartIndex == -1 ? currentStart + 1 : StartIndex - currentStart;
            return this;
        }
        public RegexPropertyArray AbjustEndCorr(int currentEnd)
        {
            EndCorr = 0;
            UpdateSection();
            if (currentEnd < EndIndex)
                throw new ArgumentOutOfRangeException("положение для корректировки не может быть меньше максимального конца свойств");
            EndCorr = EndIndex - currentEnd;
            return this;
        }
        public RegexPropertyArray Union(RegexPropertyArray array)
        {
            RegexPropertyArray leftArray = StartIndex < array.StartIndex ? this : array;
            RegexPropertyArray rightArray = EndIndex < array.EndIndex? this : array;
            StartCorr = leftArray.StartCorr;
            EndCorr = rightArray.EndCorr;
            Properties = Properties.Union(array.Properties).ToList();
            UpdateSection();
            return this;
        }

        public IEnumerator<IRegexProperty> GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Properties.GetEnumerator();
        }
    }
    public class InvalidSkriptException : Exception
    {
        public InvalidSkriptException(string message, int index) : base(message) { Index = index; }
        public int Index { get; init; }
    }
    //классы - входные данные для CustomRegexLinkData и производных
    abstract class CustomRegexLinkData : ICloneable
    {
        public CustomRegexLinkData(string root, int index, int length, string name)
        {
            Root = root;
            this.index = index;
            Properties = new(name, new List<IRegexProperty>());
            Length = length;
            IsSuccuss = true;
        }
        public string Root { get; init; }

        public string TargetString => Root.Substring(GetStartIndex(Length), Length);

        private int index;
        public int Index 
        {
            get { return index; }
            internal set
            {
                Length -= Math.Abs(index - value);
                index = value;
            }
        }
        private int length;
        public int Length 
        {
            get { return length; }
            set
            {
                if (value < 0)
                    IsSuccuss = false;
                length = value;
            }
        }
        public RegexPropertyArray Properties { get; set; }
        public bool IsSuccuss { get; set; }
        public abstract RegexOptions GetOptions();
        public abstract void AddMatch(Match match);
        public abstract void Addition(int val);
        public abstract object Clone();
        public abstract int GetStartIndex(int length);
        public abstract int GetOffset(int length);

        public abstract RightLinkData ConvertToRightLinkData();
    }
    class LeftLinkData : CustomRegexLinkData
    {
        public LeftLinkData(string root, int index, int length, string name = "") : base(root, index, length, name) { }

        public override void AddMatch(Match match)
        {
            if (match.Index + match.Length != Index)
            {
                IsSuccuss = false;
                return;
            }
            Index = match.Index;
        }
        public override void Addition(int val)
        {
            Index -= val;

        }
        public override RegexOptions GetOptions()
        {
            return RegexOptions.RightToLeft;
        }
        public override object Clone()
        {
            return new LeftLinkData(Root, Index, Length, Properties.Name);
        }

        public override int GetStartIndex(int length)
        {
            return Index - length;
        }

        public override int GetOffset(int length)
        {
            return length;
        }

        public override RightLinkData ConvertToRightLinkData()
        {
            return new RightLinkData(Root, GetStartIndex(Length), Length, Properties.Name);
        }
    }
    class RightLinkData : CustomRegexLinkData
    {
        public RightLinkData(string root, int index, int length, string name = "") : base(root, index, length, name) { }

        public override void Addition(int val)
        {
            Index += val;
        }

        public override void AddMatch(Match match)
        {
            if (match.Index != Index)
            {
                IsSuccuss = false;
                return;
            }
            Index = match.Index + match.Length;
        }

        public override object Clone()
        {
            return new RightLinkData(Root, Index, Length);
        }

        public override int GetOffset(int length)
        {
            return -length;
        }

        public override RegexOptions GetOptions()
        {
            return RegexOptions.None;
        }

        public override int GetStartIndex(int length)
        {
            return Index;
        }

        public override RightLinkData ConvertToRightLinkData()
        {
            return (RightLinkData)Clone();
        }
    }

}
