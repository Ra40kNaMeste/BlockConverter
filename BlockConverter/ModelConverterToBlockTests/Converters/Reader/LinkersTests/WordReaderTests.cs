using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class WordReaderTests:LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "WordReaderTests.xml";

        protected override string Path => "WordReaderTests.txt";

        protected override string Result => "a";

        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnSpaceAfterWord() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnSpaceBeforeWord() => Assert.IsTrue(OnTest(2));
    }
}
