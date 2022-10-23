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
using System.ComponentModel;
using System.Threading;

namespace ModelConverterToBlock.Converters.ConvertToBlock.Tests
{
    [TestClass()]
    public class ConverterToBlockTests
    {
        private static void SetStandartConverterSettings()
        {
            foreach (BlockConstruction item in Enum.GetValues(typeof(BlockConstruction)))
            {
                ConverterSetting.PropertyConverters[item] = new PropertyGenerateValueConverterTemlplate();
            }
        }

        [TestMethod()]
        public void CancelReadTest()
        {
            ConverterToBlock converter = new();
            converter.ReadAsync(new Uri("Converters/ConvertToBlock/c_sharpCorrect.xml", UriKind.Relative));
            converter.CancelRead();
            Assert.ThrowsException<PartNotFoundException>(() => converter.Convert(""));
        }

        [TestMethod()]
        public void ReadAsyncTest()
        {
            ConverterToBlock converter = new();
            converter.ReadAsync(new Uri("Converters/ConvertToBlock/c_sharpCorrect.xml", UriKind.Relative));
            Thread.Sleep(1000);
            Assert.IsTrue(converter.IsRead);
        }

        [TestMethod()]
        public void ReadTest()
        {
            ConverterToBlock converter = new();
            converter.Read(new Uri("Converters/ConvertToBlock/c_sharpCorrect.xml", UriKind.Relative));
            Assert.IsTrue(converter.IsRead);
        }

        [TestMethod()]
        public void ConvertTest()
        {
            ConverterToBlock converter = new();
            converter.Read(new Uri("Converters/ConvertToBlock/c_sharpCorrect.xml", UriKind.Relative));
            SetStandartConverterSettings();
            IBlockComponent res = converter.Convert(File.ReadAllText(Directory.GetCurrentDirectory() + "/Converters/ConvertToBlock/OneSwitch.txt")).FirstOrDefault();
            bool isSuccess = res.Content == "ab";
            Assert.IsTrue(isSuccess);
        }
    }
    [TestClass()]
    public class PropertyValueTreeCollectionTests
    {
        [TestMethod()]
        public void AddElementInEmptyConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyValueTreeCollection array = new(empty);
            PropertyEmptyValueConverter last = new();
            array.Add(last);
            Assert.IsTrue(array.Root.Successor == last & last.Successor == null);
        }

        [TestMethod()]
        public void AddElementConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter last = new();
            empty.Successor = last;
            PropertyValueTreeCollection array = new(empty);
            PropertyEmptyValueConverter two = new();
            array.Add(two);
            Assert.IsTrue(array.Root.Successor == two & two.Successor == last & last.Successor == null);
        }

        [TestMethod()]
        public void AddCoincidentElementConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter last = new();
            empty.Successor = last;
            PropertyValueTreeCollection array = new(empty);

            Assert.ThrowsException<ArgumentException>(() => array.Add(last));
        }

        [TestMethod()]
        public void AddRootElementConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter last = new();
            empty.Successor = last;
            PropertyValueTreeCollection array = new(empty);

            Assert.ThrowsException<ArgumentException>(() => array.Add(empty));
        }

        [TestMethod()]
        public void RemoveFirstElementEmptyConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter two = new();
            PropertyEmptyValueConverter last = new();
            empty.Successor = two;
            two.Successor = last;
            PropertyValueTreeCollection array = new(empty);
            bool res = array.Remove(two);
            Assert.IsTrue(res & array.Root.Successor == last & last.Successor == null);
        }

        [TestMethod()]
        public void RemoveLastElementConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter two = new();
            empty.Successor = two;
            PropertyEmptyValueConverter last = new();
            two.Successor = last;
            PropertyValueTreeCollection array = new(empty);
            bool res = array.Remove(last);
            Assert.IsTrue(res & array.Root.Successor == two & two.Successor == null);
        }

        [TestMethod()]
        public void RemoveElementConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter two = new();
            empty.Successor = two;
            PropertyEmptyValueConverter three = new();
            two.Successor = three;
            PropertyEmptyValueConverter last = new();
            three.Successor = last;
            PropertyValueTreeCollection array = new(empty);
            bool res = array.Remove(three);
            Assert.IsTrue(res & array.Root.Successor == two & two.Successor == last & last.Successor == null);
        }

        [TestMethod()]
        public void RemoveElementNotFromConverterCollection()
        {
            PropertyEmptyValueConverter empty = new();
            PropertyEmptyValueConverter two = new();
            empty.Successor = two;
            PropertyEmptyValueConverter three = new();
            two.Successor = three;
            PropertyEmptyValueConverter last = new();
            PropertyValueTreeCollection array = new(empty);
            bool res = array.Remove(last);
            Assert.IsTrue(!res & array.Root.Successor == two & two.Successor == three & three.Successor == null);
        }

        [TestMethod()]
        public void InsertFirstElementConverterCollection()
        {
            
            PropertyEmptyValueConverter empty = new();

            PropertyEmptyValueConverter two = new();
            empty.Successor = two;
            PropertyValueTreeCollection array = new(empty);
            PropertyEmptyValueConverter three = new();
            array.Insert(0, three);
            Assert.IsTrue(array.Root.Successor == two & two.Successor == three & three.Successor == null);
        }
        [TestMethod()]
        public void InsertLastElementConverterCollection()
        {

            PropertyEmptyValueConverter empty = new();

            PropertyEmptyValueConverter three = new();
            empty.Successor = three;
            PropertyValueTreeCollection array = new(empty);
            PropertyEmptyValueConverter two = new();
            array.Insert(1, two);
            Assert.IsTrue(array.Root.Successor == two & two.Successor == three & three.Successor == null);
        }
    }

}