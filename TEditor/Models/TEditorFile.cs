using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEditor.Models;

namespace TEditor
{
    [Serializable]
    public class TEditorFile
    {
        public int VersionCode { get; set; } = 0;
        public DocModel DocModel { get; set; } = new();
        public LayerModel[] Layers { get; set; } = new LayerModel[0];
    }
}
