using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class NotWordSkiperTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "NotWordSkiperTests.xml";

        protected override string Path => "NotWordSkiperTests.txt";

        protected override string Result => "a";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnOnlySpace() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnTwoWord() => Assert.IsTrue(OnTest(2));
    }
}
