using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TEditor
{
    public class AdornerManager
    {
        Canvas _canvasLayout;
        Canvas _canvasContent;
        AdornerLayer _adornerLayer;
        //List<Canvas> _substitute = new List<Canvas>();
        Dictionary<FrameworkElement, Canvas> _substitute = new Dictionary<FrameworkElement, Canvas>();

        public delegate void NewPropHandler(FrameworkElement element, CanProp prop);
        public event NewPropHandler OnNewProp;

        public AdornerManager(Canvas canvasLayout, Canvas canvasContent)
        {
            _canvasLayout = canvasLayout;
            _canvasContent = canvasContent;
            _adornerLayer = AdornerLayer.GetAdornerLayer(_canvasLayout);
        }

        public void Attach(FrameworkElement layer)
        {
            Canvas substitute = new Canvas();
            ResizeAdorner ra = new ResizeAdorner(substitute);
            ra.OnChanged += Ra_OnChanged;

            _canvasLayout.Children.Add(substitute);
            _substitute.Add(layer, substitute);

            ArrangeControl(layer, substitute);
        }

        public void Deattach(FrameworkElement layer)
        {
            _canvasLayout.Children.Remove(_substitute[layer]);
            _substitute.Remove(layer);
        }

        private void Ra_OnChanged(UIElement element, CanProp prop)
        {
            var key = _substitute.FirstOrDefault(x => x.Value == element).Key;
        }

        private void ArrangeControl(KeyValuePair<FrameworkElement, Canvas> WithSubstitue)
            => ArrangeControl(WithSubstitue.Key, WithSubstitue.Value);
        private void ArrangeControl(FrameworkElement target, Canvas substitue)
        {
            double scale = _canvasContent.LayoutTransform.Value.M11;
            double targetRenderWidth = target.ActualWidth * scale;
            double targetRenderHeight = target.ActualHeight * scale;
            substitue.Width = targetRenderWidth;
            substitue.Height = targetRenderHeight;
            Canvas.SetLeft(substitue, Canvas.GetLeft(_canvasContent) + Canvas.GetLeft(target) * scale);
            Canvas.SetTop(substitue, Canvas.GetTop(_canvasContent) + Canvas.GetTop(target) * scale);
        }
    }
}
