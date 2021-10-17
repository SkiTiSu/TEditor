using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TEditor
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection visualChilderns;
        Canvas ParentCanvas;
        public FrameworkElement Substitute { get; set; }
        public double SubstituteScale { get; set; }

        public delegate void ChangedHandler(UIElement element, CanProp prop);
        public event ChangedHandler OnChanged;

        public ResizeAdorner(UIElement element)
            : base(element)
        {
            visualChilderns = new VisualCollection(this);
            //ParentCanvas = parent;

            CreateThumb(PlacementAlignment.Top);
            CreateThumb(PlacementAlignment.TopLeft);
            CreateThumb(PlacementAlignment.TopRight);
            CreateThumb(PlacementAlignment.Left);
            CreateThumb(PlacementAlignment.Right);
            CreateThumb(PlacementAlignment.Bottom);
            CreateThumb(PlacementAlignment.BottomLeft);
            CreateThumb(PlacementAlignment.BottomRight);

            Rectangle border = new Rectangle()
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 })
            };
            visualChilderns.Add(border);

            this.Loaded += ResizeAdorner_Loaded;
        }

        private void ResizeAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            IsClipEnabled = true;
        }

        ResizeThumb CreateThumb(PlacementAlignment alignment)
        {
            ResizeThumb thumb = new ResizeThumb();
            thumb.Alignment = alignment;
            thumb.Cursor = alignment.GetCursor();

            thumb.DragDelta += Thumb_DragDelta;

            visualChilderns.Add(thumb);

            return thumb;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;

            ResizeThumb thumb = sender as ResizeThumb;
            var alignment = thumb.Alignment;

            double dx = 0;
            double dy = 0;

            if (alignment.Horizontal == HorizontalAlignment.Left) dx = -e.HorizontalChange;
            if (alignment.Horizontal == HorizontalAlignment.Right) dx = e.HorizontalChange;
            if (alignment.Vertical == VerticalAlignment.Top) dy = -e.VerticalChange;
            if (alignment.Vertical == VerticalAlignment.Bottom) dy = e.VerticalChange;

            var newWidth = Math.Max(1, adornedElement.Width + dx);
            var newHeight = Math.Max(1, adornedElement.Height + dy);
            var newLeft = Canvas.GetLeft(adornedElement) - dx;
            var newTop = Canvas.GetTop(adornedElement) - dy;

            CanProp prop = new CanProp()
            {
                Width = newWidth,
                Height = newHeight,
                Left = newLeft,
                Top = newTop,
                ChangeLeft = false,
                ChangeTop = false
            };

            if (alignment.Horizontal == HorizontalAlignment.Left)
            {
                Canvas.SetLeft(adornedElement, newLeft);
                prop.ChangeLeft = true;
            }
            if (alignment.Vertical == VerticalAlignment.Top)
            {
                Canvas.SetTop(adornedElement, newTop);
                prop.ChangeTop = true;
            }

            adornedElement.Width = newWidth;
            adornedElement.Height = newHeight;

            OnChanged?.Invoke(adornedElement, prop);
        }

        private MouseAction _currentMouseAction;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //Point point = Mouse.GetPosition(AdornedElement);
            //_currentMouseAction = GetMouseAction(point);
            //UpdateCursor();
        }


        private enum MouseAction
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Top,
            Bottom,
            Left,
            Right,
            Move,
            Rotate
        }

        private MouseAction GetMouseAction(Point point)
        {
            double h2 = ActualHeight / 2;
            double w2 = ActualWidth / 2;
            if (point.X < w2 && point.Y < h2)
                return MouseAction.TopLeft;
            else if (point.X > w2 && point.Y > h2)
                return MouseAction.BottomRight;
            else if (point.X > w2 && point.Y < h2)
                return MouseAction.TopRight;
            else
                return MouseAction.BottomLeft;
        }

        private void UpdateCursor()
        {
            switch (_currentMouseAction)
            {
                case MouseAction.TopLeft:
                case MouseAction.BottomRight:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case MouseAction.TopRight:
                case MouseAction.BottomLeft:
                    Cursor = Cursors.SizeNESW;
                    break;
                case MouseAction.Top:
                case MouseAction.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                case MouseAction.Left:
                case MouseAction.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case MouseAction.Move:
                    Cursor = Cursors.Cross;
                    break;
                case MouseAction.Rotate:
                    Cursor = Cursors.Hand; //TODO
                    break;
                default:
                    Cursor = Cursors.Arrow;
                    break;

            }
        }

        protected override int VisualChildrenCount { get { return visualChilderns.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChilderns[index]; }


        protected override Size ArrangeOverride(Size finalSize) //Should use ArrangeCore instead?
        {
            base.ArrangeOverride(finalSize);

            double desireWidth = AdornedElement.DesiredSize.Width;
            double desireHeight = AdornedElement.DesiredSize.Height;

            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            foreach (var visual in visualChilderns)
            {
                if (visual is Rectangle)
                {
                    Rectangle border = visual as Rectangle;
                    border.Width = adornerWidth;
                    border.Height = adornerHeight;
                    border.Arrange(new Rect(-1, -1, adornerWidth + 2, adornerHeight + 2));
                    continue;
                }
                ResizeThumb thumb = visual as ResizeThumb;
                if (thumb == null) continue;
                double x = 0;
                double y = 0;
                switch (thumb.Alignment.Horizontal)
                {
                    case HorizontalAlignment.Left:
                        x = -adornerWidth / 2;
                        break;
                    case HorizontalAlignment.Center:
                        x = 0;
                        break;
                    case HorizontalAlignment.Right:
                        x = desireWidth - adornerWidth / 2;
                        break;
                }
                switch (thumb.Alignment.Vertical)
                {
                    case VerticalAlignment.Top:
                        y = -adornerHeight / 2;
                        break;
                    case VerticalAlignment.Center:
                        y = 0;
                        break;
                    case VerticalAlignment.Bottom:
                        y = desireHeight - adornerHeight / 2;
                        break;
                }
                thumb.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            }

            return finalSize;
        }

        double _renderRadius = 5.0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            //Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            //SolidColorBrush renderBrush = new SolidColorBrush(Colors.Black);
            //renderBrush.Opacity = 0.3;
            //Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, _renderRadius, _renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, _renderRadius, _renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, _renderRadius, _renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, _renderRadius, _renderRadius);

            //drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(adornedElementRect.TopLeft + new Vector(-5, 0), adornedElementRect.BottomLeft));
            //drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(adornedElementRect.TopLeft + new Vector(0, -5), adornedElementRect.TopRight));
            //drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(adornedElementRect.TopRight, adornedElementRect.BottomRight + new Vector(5, 0)));
            //drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(adornedElementRect.BottomLeft, adornedElementRect.BottomRight + new Vector(0, 5)));

        }
    }
}
