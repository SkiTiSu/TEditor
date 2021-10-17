using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{
    public class LayerModel
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public int ZIndex { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public bool Visible { get; set; }
        public string LayerNameCustom { get; set; }
        public bool ClippingMaskEnable { get; set; }
        public bool ClippingMaskBottom { get; set; }
        public object Data { get; set; }
    }
}
