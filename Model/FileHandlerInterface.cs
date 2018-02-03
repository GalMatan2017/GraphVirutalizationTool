using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVirtualizationTool
{
    interface FileHandlerInterface
    {
         List<List<T>> ParseFile<T>(string filename);
    }
}
