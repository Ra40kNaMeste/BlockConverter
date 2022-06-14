using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    //некий компонент, который читает строки
    internal class CustomRegexComponent
    {
        public CustomRegexComponent()
        {
            IsFill = false;
            RightLink = new();
            LeftLink = new();
        }
        /// <summary>
        /// Интерпретировать строку
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="startIndex">начало чтения</param>
        /// <param name="lenght">длина</param>
        /// <returns>найденные свойства</returns>
        internal CustomRegexLinkData Start(string str, int startIndex, int lenght)
        {
            //нахождение индекса ключевого слова если оно отсутвует, то берётся конец строки
            var indexMatchs = RootLiksGetter == null ? GetEndWords(lenght) : FindWords(str, startIndex, lenght);
            foreach (RootWord indexMatch in indexMatchs)
            {
                List<CustomRegexLinkData> globalDates = new();
                int index = indexMatch.Index + startIndex;
                int oldIndex = index + 1;

                //интерпретация вправо и влево от ключегого индекса

                CustomRegexLinkData leftData = new LeftLinkData(str, index, index - startIndex);
                globalDates.Add(leftData);
                CustomRegexLinkData rightData = new RightLinkData(str, index, startIndex + lenght - index);
                globalDates.Add(rightData);
                LeftLink.Interpret(leftData);
                RightLink.Interpret(rightData);

                //объединение всех найденных свойств

                CustomRegexLinkData resData = new RightLinkData(str, startIndex, 0, "Properties");
                resData.Length = indexMatch.Length;

                int min = leftData.Index;
                int max = rightData.Index;

                int startIndexData, endIndexData;
                RegexPropertyArray Properties = resData.Properties;
                foreach (var data in globalDates)
                {
                    endIndexData = data.Index;
                    startIndexData = data.GetStartIndex(data.Length);
                    if (startIndexData > endIndexData)
                        Swap<int>(ref startIndexData, ref endIndexData);
                    resData.IsSuccuss &= data.IsSuccuss;
                    Properties.Properties = Properties.Properties.Concat(data.Properties.Properties).ToList();
                }
                if (resData.IsSuccuss)
                {
                    resData.Index = min;
                    resData.Length = max - min;
                    resData.IsSuccuss = true;
                    return resData;
                }

            }
            return new RightLinkData(str, startIndex, 0, "Properties") { IsSuccuss = false };
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        internal IEnumerable<RootWord> FindWords(string str, int startIndex, int lenght)
        {
            MatchCollection indexMatchs = RootLiksGetter.GetRegex(RegexOptions.None).Matches(str.Substring(startIndex, lenght));
            return indexMatchs.Select(i => new RootWord(i.Index, i.Length));
        }
        internal IEnumerable<RootWord> GetEndWords(int lenght)
        {
            return new List<RootWord>() { new RootWord(lenght, 0) };
        }

        public WordFromLinksGetter RootLiksGetter { get; set; }

        public LinkChainCollection RightLink { get; set; }
        public LinkChainCollection LeftLink { get; set; }
        protected internal ICustomRegexComponentStateFillable StateFill = LeftComponentFillState.GetObject();
        public bool IsFill { get; set; }
        public void Add(LinkReaderDataBase link) => StateFill?.AddLink(link, this);

    }

    internal struct RootWord
    {
        public RootWord(int index, int length)
        {
            Index = index;
            Length = length;
        }
        public int Index;
        public int Length;
    }
    //его состояния. Прикрепляет сначала к левой цепочки, а, когда находит элемент с ключевым свойством, меняет на правый
    //правый же пикрепляет к правой цепочки, а, когда находит Skiper, меняет значение isFill=false. Теперь нельзя добавлять элементы 
    interface ICustomRegexComponentStateFillable
    {
        public void AddLink(LinkReaderDataBase link, CustomRegexComponent targetComponent);
    }
    class LeftComponentFillState : Singletone<LeftComponentFillState>, ICustomRegexComponentStateFillable
    {
        private LeftComponentFillState() { }
        public void AddLink(LinkReaderDataBase link, CustomRegexComponent targetComponent)
        {
            if (link is Skiper)
                if (targetComponent.LeftLink.Lenght == 0)
                    return;
                else
                    throw new ArgumentOutOfRangeException("link", "Не возможно создать цепочку");
            if (link is IFindRootWordable)
            {
                IFindRootWordable findRoot = (IFindRootWordable)link;
                targetComponent.RootLiksGetter = findRoot.GetRootWord();
                targetComponent.StateFill = RightComponentFillState.GetObject();
                switch (findRoot.DirectionFromWord)
                {
                    case Direction.Right:
                        targetComponent.RightLink.PushBack(link);
                        break;
                    case Direction.Left:
                        targetComponent.LeftLink.PushFront(link);
                        targetComponent.RightLink.PushBack(new WordReader(findRoot.GetRootWord()));
                        break;
                    default:
                        break;
                }

            }
            else
                targetComponent.LeftLink.PushFront(link);

        }
    }
    class RightComponentFillState : Singletone<RightComponentFillState>, ICustomRegexComponentStateFillable
    {
        private RightComponentFillState() { }
        public void AddLink(LinkReaderDataBase link, CustomRegexComponent targetComponent)
        {
            if (link is Skiper)
            {
                targetComponent.IsFill = true;
                targetComponent.StateFill = null;
            }
            else
                targetComponent.RightLink.PushBack(link);
        }
    }
}
