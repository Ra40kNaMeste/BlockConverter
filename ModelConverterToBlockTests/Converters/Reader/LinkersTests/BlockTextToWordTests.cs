using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class BlockTextToWordTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "BlockTextToWordTests.xml";

        protected override string Path => "BlockTextToWordTests.txt";

        protected override string Result => "ab+ab";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnNotStopWord() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnWithLinks() => Assert.IsTrue(OnTest(2));
    }
}
