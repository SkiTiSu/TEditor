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
    public class ShapeRectangleLayer : ShapeLayerBase
    {
        public override string Type { get; } = "矩形";
        public override string Key { get; } = LayerType.Rectangle;


        public ShapeRectangleLayer(Canvas canvasLayout, Layer canvasContent, ShapeLayerBaseModel model)
            : base(canvasLayout, canvasContent, model)
        {
        }
        public ShapeRectangleLayer(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new ShapeLayerBaseModel())
        {

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(FillColor), BorderPen, new System.Windows.Rect(0, 0, Width, Height));
        }
    }
}
