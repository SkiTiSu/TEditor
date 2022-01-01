using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{
    public class ShapeRectangleLayerModel : ShapeLayerBaseModel
    {
        public bool RadiusLink { get; set; } = true;
        public double RadiusTopLeft { get; set; } = 0.0;
        public double RadiusTopRight { get; set; } = 0.0;
        public double RadiusBottomRight { get; set; } = 0.0;
        public double RadiusBottomLeft { get; set; } = 0.0;
    }
}
