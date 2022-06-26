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
        public FrameworkElement Substitute { get; set; }
        public double SubstituteScale { get; set; }

        public delegate void ChangedHandler(UIElement element, CanProp prop);
        public event ChangedHandler OnChanged;

        public ResizeAdorner(UIElement element)
            : base(element)
        {
            visualChilderns = new VisualCollection(this);

            CreateThumb(PlacementAlignment.Top);
            CreateThumb(PlacementAlignment.TopLeft);
            CreateThumb(PlacementAlignment.TopRight);
            CreateThumb(PlacementAlignment.Left);
            CreateThumb(PlacementAlignment.Right);
            CreateThumb(PlacementAlignment.Bottom);
            CreateThumb(PlacementAlignment.BottomLeft);
            CreateThumb(PlacementAlignment.BottomRight);

            Rectangle border = new()
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
            ResizeThumb thumb = new()
            {
                Alignment = alignment,
                Cursor = alignment.GetCursor()
            };

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

            CanProp prop = new()
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
    }
}
