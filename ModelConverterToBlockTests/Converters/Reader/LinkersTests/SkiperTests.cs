using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelConverterToBlock.Blocks;
using ModelConverterToBlock.Converters.ConvertToBlock;
using ModelConverterToBlock.Converters.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlockTests.Converters.Reader.LinkersTests
{
    public abstract class LinkersTestsBase
    {
        protected abstract string PathFolder { get; }
        protected abstract string PathInstructions { get; }
        protected abstract string Path { get; }
        protected abstract string Result { get; }
        public LinkersTestsBase()
        {
            SetStandartConverterSettings();
            converter.Read(new Uri(PathFolder + "/" + PathInstructions, UriKind.Relative));
            FillTestsDic();
        }
        private void FillTestsDic()
        {
            string str = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + PathFolder + "/" + Path);
            var strs = str.Split("next");
            foreach (var item in strs)
            {
                var temp = item.Split("result:");
                testsDic.Add(new(Convert.ToBoolean(temp[1]), temp[0]));
            }
        }
        protected ConverterToBlock converter = new();
        protected List<(bool, string)> testsDic = new();
        private static void SetStandartConverterSettings()
        {
            foreach (BlockConstruction item in Enum.GetValues(typeof(BlockConstruction)))
            {
                ConverterSetting.PropertyConverters[item] = new PropertyGenerateValueConverterTemlplate();
            }
        }

        protected bool OnTest(int number)
        {
            var target = testsDic[number];
            BeginBlockComponent begin = converter.Convert(target.Item2).First();
            return (begin.GetComponent(new BlockComponentMetadata()).Content == Result) == target.Item1;
        }

    }
    [TestClass()]
    public class SkiperTests: LinkersTestsBase
    {
        protected override string PathFolder => "Converters/Reader/LinkersTests/LinkersTestsDates";

        protected override string PathInstructions => "SkiperTests.xml";

        protected override string Path => "SkiperTests.txt";

        protected override string Result => "a";

        [TestMethod()]
        public void OnDefault() => Assert.IsTrue(OnTest(0));
        [TestMethod()]
        public void OnNotWord() => Assert.IsTrue(OnTest(1));
        [TestMethod()]
        public void OnTwoWord() => Assert.IsTrue(OnTest(2));
    }
}
