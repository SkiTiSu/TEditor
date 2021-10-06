using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TEditor.Models
{
    public class ShapeLayerBaseModel
    {
        public Color FillColor { get; set; } = Colors.Black;
        public Color BorderColor { get; set; } = Colors.White;
        public double BorderWidth { get; set; } = 0;
        public double Width { get; set; } = 100;
        public double Height { get; set; } = 100;
    }
}
