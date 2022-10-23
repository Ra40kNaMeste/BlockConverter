using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{

    [TestClass()]
    public class ByPatternReaderTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "ByPatternReaderTests.xml";

        protected override string Path => "ByPatternReaderTests.txt";

        protected override string Result => "8-9";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnNotPattern() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnWordAndNextPattern() => Assert.IsTrue(OnTest(2));
    }

}
