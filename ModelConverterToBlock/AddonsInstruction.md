<h1>Добавления нового шаблона</h1>
<p>
1)  Унаследовать класс от LinkReaderDataBase или его потомков. 
Например PropertyData - позволяет создавать свойства а реализация IFindWordable - искать ключевое слово.
Реализовать Clone()
2) создать класс - читатель из файла. Унаследовать интерфейс IContextNodeReaderable.
3) войти в stringInterpret и отформатировать ConvertToNodeReader</p>

<h1>Добавления нового блока</h1>
<p>1) зайти в Blocks/Tree и создать новый класс унаследованный от IBlockComponent или от его производных.
добавить синхронизцию Json
2)В Converters/Reader/Enums добавить конструкцию
3) Добавляем обработчик в ConvertToBlock/Converter унаследованный от ConverterLinkToBlockBase 
и переопределить метод Convert
4) там же добавить в метод ConvertToLink</p>
