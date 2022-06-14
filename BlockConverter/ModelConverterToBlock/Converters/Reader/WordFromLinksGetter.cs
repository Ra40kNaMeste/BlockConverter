using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    abstract class WordFromLinksGetter
    {
        public WordFromLinksGetter(string word) => regex = word;
        protected string regex;
        public abstract Regex GetRegex(RegexOptions options);

    }
    class CommonWordFromLinksGetter: WordFromLinksGetter
    {
        public CommonWordFromLinksGetter(string word) : base(word) { }
        public override string ToString()
        {
            return regex;
        }

        public override Regex GetRegex(RegexOptions options)
        {
            return new Regex(Regex.Escape(regex), options);
        }
    }
    class RegexWordFromLinksGetter : WordFromLinksGetter
    {
        public RegexWordFromLinksGetter(string regexStr) : base(regexStr) { }

        public override Regex GetRegex(RegexOptions options)
        {
            return new Regex(regex, options);
        }

        public override string ToString()
        {
            return Regex.Unescape(regex);
        }
    }

}
