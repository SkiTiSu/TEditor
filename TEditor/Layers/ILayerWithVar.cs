using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Layers
{
    interface ILayerWithVar
    {
        void UpdateVar(Dictionary<string, string> keyValues);
    }
}
