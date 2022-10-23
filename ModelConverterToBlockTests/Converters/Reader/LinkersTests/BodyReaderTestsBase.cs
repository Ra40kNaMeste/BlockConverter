using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelConverterToBlock.Blocks;
using ModelConverterToBlock.Converters.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    public abstract class BodyReaderTestsBase:LinkersTestsBase
    {

        protected void OnSkriptExceptionTest(int number)
        {
            var target = testsDic[number];
            Assert.ThrowsException<AggregateException>(() => converter.Convert(target.Item2).First());
        }
    }
    public class BodyReaderTests:BodyReaderTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "BodyReaderTests.xml";

        protected override string Path => "BodyReaderTests.txt";

        protected override string Result => "a";
        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnTwoStartWord() => OnSkriptExceptionTest(1);
        [TestMethod()]
        public void OnTwoEndWord() => Assert.IsTrue(OnTest(2));
        [TestMethod()]
        public void OnTwoWords() => Assert.IsTrue(OnTest(3));

    }
}
