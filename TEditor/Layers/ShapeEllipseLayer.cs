using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TEditor.Models;

namespace TEditor.Layers
{
    public class ShapeEllipseLayer : ShapeLayerBase<ShapeLayerBaseModel>
    {
        public override string Type { get; } = "椭圆";
        public override string Key { get; } = LayerType.Ellipse;

        public ShapeEllipseLayer(Canvas canvasLayout, Layer canvasContent, ShapeLayerBaseModel model)
            : base(canvasLayout, canvasContent, model)
        {
        }
        public ShapeEllipseLayer(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new ShapeLayerBaseModel())
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(new SolidColorBrush(FillColor), BorderPen, new System.Windows.Point(Width / 2, Height / 2), Width / 2, Height / 2);
        }
    }
}
