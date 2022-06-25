using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Utils.Undo
{
    // TODO 避免反射用的，最好还是用特性+编译时分析，还没研究怎么实现
    public interface IUndoHasIgnore
    {
        IEnumerable<string> UndoIgnore { get; }
    }
}
