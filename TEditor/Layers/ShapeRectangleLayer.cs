using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TEditor.Models;

namespace TEditor.Layers
{
    public class ShapeRectangleLayer : ShapeLayerBase<ShapeRectangleLayerModel>
    {
        public override string Type { get; } = "矩形";
        public override string Key { get; } = LayerType.Rectangle;
        public override FrameworkElement LayerControl { get; protected set; } = new ShapeRectangleLayerControl();

        public ShapeRectangleLayer(Canvas canvasLayout, Layer canvasContent, ShapeRectangleLayerModel model)
            : base(canvasLayout, canvasContent, model)
        {
        }
        public ShapeRectangleLayer(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new ShapeRectangleLayerModel())
        {
        }

        public bool RadiusLink { get => model.RadiusLink; set => model.RadiusLink = value; }
        private bool radiusLinking = false;
        public double RadiusTopLeft
        {
            get => model.RadiusTopLeft;
            set
            {
                if (RadiusLink && !radiusLinking)
                {
                    radiusLinking = true;
                    double diff = value - model.RadiusTopLeft;
                    RadiusTopRight += diff;
                    RadiusBottomRight += diff;
                    RadiusBottomLeft += diff;
                    radiusLinking = false;
                }
                model.RadiusTopLeft = value >= 0 ? value : 0;
                ReInit();
            }
        }
        public double RadiusTopRight 
        { 
            get => model.RadiusTopRight; 
            set
            {
                if (RadiusLink && !radiusLinking)
                {
                    radiusLinking = true;
                    double diff = value - model.RadiusTopRight;
                    RadiusTopLeft += diff;
                    RadiusBottomRight += diff;
                    RadiusBottomLeft += diff;
                    radiusLinking = false;
                }
                model.RadiusTopRight = value >= 0 ? value : 0;
                ReInit();
            }
        }
        public double RadiusBottomRight 
        { 
            get => model.RadiusBottomRight; 
            set
            {
                if (RadiusLink && !radiusLinking)
                {
                    radiusLinking = true;
                    double diff = value - model.RadiusBottomRight;
                    RadiusTopLeft += diff;
                    RadiusTopRight += diff;
                    RadiusBottomLeft += diff;
                    radiusLinking = false;
                }
                model.RadiusBottomRight = value >= 0 ? value : 0;
                ReInit();
            }
        }
        public double RadiusBottomLeft 
        { 
            get => model.RadiusBottomLeft;
            set
            {
                if (RadiusLink && !radiusLinking)
                {
                    radiusLinking = true;
                    double diff = value - model.RadiusBottomLeft;
                    RadiusTopLeft += diff;
                    RadiusTopRight += diff;
                    RadiusBottomRight += diff;
                    radiusLinking = false;
                }
                model.RadiusBottomLeft = value >= 0 ? value : 0;
                ReInit();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawRoundedRectangleEx(drawingContext, new SolidColorBrush(FillColor), BorderPen, new Rect(0, 0, Width, Height),
                new CornerRadius(RadiusTopLeft, RadiusTopRight, RadiusBottomRight, RadiusBottomLeft));
        }

        public static void DrawRoundedRectangleEx(DrawingContext dc, Brush brush,
            Pen pen, Rect rect, CornerRadius cornerRadius)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                bool isStroked = pen != null;
                const bool isSmoothJoin = true;

                context.BeginFigure(rect.TopLeft + new Vector(0, cornerRadius.TopLeft), brush != null, true);
                context.ArcTo(new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y),
                    new Size(cornerRadius.TopLeft, cornerRadius.TopLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.TopRight - new Vector(cornerRadius.TopRight, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.TopRight.X, rect.TopRight.Y + cornerRadius.TopRight),
                    new Size(cornerRadius.TopRight, cornerRadius.TopRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomRight - new Vector(0, cornerRadius.BottomRight), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y),
                    new Size(cornerRadius.BottomRight, cornerRadius.BottomRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);
                context.LineTo(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0), isStroked, isSmoothJoin);
                context.ArcTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                    new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.Close();
            }
            dc.DrawGeometry(brush, pen, geometry);
        }
    }
}
