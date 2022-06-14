using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class PropertyDataTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "PropertyDataTests.xml";

        protected override string Path => "PropertyDataTests.txt";

        protected override string Result => "a";

        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
    }
}
