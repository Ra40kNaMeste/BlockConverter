using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class SpaceSkiperTests : LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "SpaceSkiperTests.xml";

        protected override string Path => "SpaceSkiperTests.txt";

        protected override string Result => "a";
        
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnMultiSpace() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnNotOneSpace() => Assert.IsTrue(OnTest(2));

        [TestMethod()]
        public void OnNotMultiSpace() => Assert.IsTrue(OnTest(4));
        [TestMethod()]
        public void OnSymbol() => Assert.IsTrue(OnTest(5));
    }
}
