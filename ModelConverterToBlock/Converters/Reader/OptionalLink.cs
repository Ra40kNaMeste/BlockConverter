using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    class OptionalLink : PropertyData
    {
        public OptionalLink(string name, List<CustomRegexByPattern> regexes, bool isOnlyOne) : base(name)
        {
            this.regexes = regexes;
            this.isOnlyOne = isOnlyOne;
        }
        private readonly bool isOnlyOne;
        private readonly List<CustomRegexByPattern> regexes;
        protected internal override Regex GetRegex(CustomRegexLinkData data)
        {
            throw new NotImplementedException();
        }

        protected internal override void Interpret(CustomRegexLinkData data)
        {
            RegexPropertyArray array = null, temp;
            foreach (var item in regexes)
            {
                int startIndex = data.GetStartIndex(data.Length);
                temp = item.FindProperties(data.Root, startIndex, data.Length);
                if (temp != null && temp.StartIndex == startIndex &&
                    (array == null || temp.StartIndex < array.StartIndex))
                {
                    array = temp;
                    break;
                }
            }
            if (array == null)
            {
                if (isOnlyOne)
                {
                    data.IsSuccuss = false;
                    return;
                }
            }
            else
            {
                data.Properties.Properties.Add(new RegexPropertyArray(Name, array.Properties));
                data.Addition(array.Length + data.GetStartIndex(array.Length) - array.StartIndex);
            }
            if (data.IsSuccuss)
                Succesor?.Interpret(data);
        }
        public override object Clone()
        {
            return new OptionalLink(Name, regexes, isOnlyOne) { Succesor = Succesor };
        }
    }
}

