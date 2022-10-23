using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverterToBlock.Converters.Reader
{
    /// <summary>
    /// задаётся приоритет конструкций
    /// </summary>
    public enum BlockConstruction
    {
        Switch = 7,
        Function = 6,
        If = 5,
        ForCycle = 4,
        PostCycle = 3,
        Cycle = 2,
        Return = 1,
        Input = 0,
        Output = -1,
        Method = -2,
        Operation = -3,
        None = -4
    }

}
