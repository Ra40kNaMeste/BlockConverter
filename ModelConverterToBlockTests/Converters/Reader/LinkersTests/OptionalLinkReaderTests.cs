using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class OptionalLinkReaderTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "OptionalLinkReaderTests.xml";

        protected override string Path => "OptionalLinkReaderTests.txt";

        protected override string Result => "ab";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public  void OnDefaultOne() => Assert.IsTrue(OnTest(1));

        [TestMethod()]
        public void OnNotOption1() => Assert.IsTrue(OnTest(2));
        [TestMethod()]
        public void OnNotOption2() => Assert.IsTrue(OnTest(3));
    }
}
