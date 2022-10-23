using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class StretchPropertyDataTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "StretchPropertyDataTests.xml";

        protected override string Path => "StretchPropertyDataTests.txt";

        protected override string Result => "ab";

        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnTwoProperty() => Assert.IsTrue(OnTest(0));
    }
}
