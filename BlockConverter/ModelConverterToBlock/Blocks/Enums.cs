using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Blocks
{
    /// <summary>
    /// Свойства блока
    /// </summary>
    public enum BlockProperty
    {
        Next, ChildBlocks, Content, None, EndBlockContent, BlockName
    }
    /// <summary>
    /// Дочерние ветви блока If
    /// </summary>
    public enum BlockIfChilds
    {
        TrueBlock, FalseBlock
    }

}
