using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    [TestClass()]
    public class ArrayParamsReaderTests:BodyReaderTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "ArrayParamsReaderTests.xml";

        protected override string Path => "ArrayParamsReaderTests.txt";

        protected override string Result => "ab";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnNotTwoProperty() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnTwoProperty() => Assert.IsTrue(OnTest(2));
        [TestMethod()]
        public void OnNotOneProperty() => Assert.IsTrue(OnTest(3));
        
        [TestMethod()]
        public void OnTwoStartWord() => OnSkriptExceptionTest(5);
        [TestMethod()]
        public void OnTwoEndWord() => Assert.IsTrue(OnTest(4));
    }
}
