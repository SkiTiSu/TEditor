using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Media.Converters;
using TEditor.Models;
using System.Security.RightsManagement;
using System.Text.Json;
using System.Text.Encodings.Web;
using TEditor.Converters;
using PropertyChanged;
using TEditor.Layers;

namespace TEditor
{
    [Serializable]
    public class LayerInner : Canvas, INotifyPropertyChanged
    {
        public double GridSubdivisions = 0.5;
        public event PropertyChangedEventHandler PropertyChanged;

        public string LayerName { get; set; } = "图层";
        public virtual string Type { get; } = "Default";
        public virtual string Key { get; } = "DefaultLayer";
        public virtual object Model { get; set; }
        public virtual FrameworkElement LayerControl { get; protected set; } = new DefaultLayerControl();
        private bool visible = true;
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                if (visible)
                {
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Visibility = Visibility.Hidden;
                }
            }
        }

        Canvas _canvasLayout;
        public Layer ParentCanvas;
        AdornerLayer _adornerLayer;
        public Substitute _substitute;

        double ContentScale { get => ParentCanvas.ContentScale; }
        double ContentDisLeft { get => ParentCanvas.ContentDisLeft; }
        double ContentDisTop { get => ParentCanvas.ContentDisTop; }

        public event MouseButtonEventHandler SubstituteMouseLeftButtonDown;

        public LayerInner(Canvas canvasLayout, Layer canvasContent)
            : base()
        {
            _canvasLayout = canvasLayout;
            ParentCanvas = canvasContent;
            _adornerLayer = AdornerLayer.GetAdornerLayer(_canvasLayout);

            _substitute = new Substitute();
            _substitute.Visibility = Visibility.Collapsed;
            _substitute.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            _substitute.MouseLeftButtonDown += Substitute_MouseLeftButtonDown;
            //ArrangeControl(layer, substitute);
            ResizeAdorner ra = new ResizeAdorner(_substitute);
            ra.OnChanged += Ra_OnChanged;

            _canvasLayout.Children.Add(_substitute);

            _adornerLayer.Add(ra);

            if (!(LayerControl is DefaultLayerControl))
            {
                LayerControl.DataContext = this;
            }       
        }

        private void Substitute_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubstituteMouseLeftButtonDown(this, e);
        }

        private void Ra_OnChanged(UIElement element, CanProp prop)
        {
            ApplyCanProp(prop);
        }

        public void ApplyCanProp(CanProp prop)
        {
            LayoutWidth = prop.Width;
            LayoutHeight = prop.Height;
            if (prop.ChangeLeft)
            {
                LayoutLeft = prop.Left;
            }
            if (prop.ChangeTop)
            {
                LayoutTop = prop.Top;
            }
        }

        public new virtual double Width
        {
            get => base.Width;
            set => base.Width = value;
        }

        public new virtual double Height
        {
            get => base.Height;
            set => base.Height = value;
        }

        public double LayoutWidth
        {
            get => this.Width * ContentScale;
            set
            {
                this.Width = value / ContentScale;
            }
        }

        public double LayoutHeight
        {
            get => this.Height * ContentScale;
            set
            {
                this.Height = value / ContentScale;
            }
        }

        public Point LayoutPosition
        {
            get => new Point(LayoutLeft, LayoutTop);
            set
            {
                LayoutLeft = value.X;
                LayoutTop = value.Y;
            }
        }

        public double LayoutLeft
        {
            get => ContentDisLeft + ContentLeft * ContentScale;
            set
            {
                ContentLeft = (value - ContentDisLeft) / ContentScale;
            }
        }
        public double LayoutTop
        {
            get => ContentDisTop + ContentTop * ContentScale;
            set
            {
                ContentTop = (value - ContentDisTop) / ContentScale;
            }
        }

        public double ContentLeftOffset { get; set; } = 0;
        public double ContentTopOffset { get; set; } = 0;

        public Point ContentPosition
        {
            get => new Point(ContentLeft, ContentTop);
            set
            {
                ContentLeft = value.X;
                ContentTop = value.Y;
            }
        }

        public double ContentLeft
        {
            get => (!double.IsNaN(Canvas.GetLeft(this))) ? Canvas.GetLeft(this) : 0;
            set
            {
                Canvas.SetLeft(this, RoundUpToNearest(value, GridSubdivisions));
                PositionChanged();
            }
        }

        public double ContentTop
        {
            get => (!double.IsNaN(Canvas.GetTop(this))) ? Canvas.GetTop(this) : 0;
            set
            {
                Canvas.SetTop(this, RoundUpToNearest(value, GridSubdivisions));
                PositionChanged();
            }
        }

        private void PositionChanged()
        {
            ReArrangeInner();
        }

        private void SubstituteArrange()
        {
            _substitute.Width = LayoutWidth;
            _substitute.Height = LayoutHeight;
            //TODO: 厘清位置关系，把Offset放到更合适的位置
            Canvas.SetLeft(_substitute, LayoutLeft + ContentLeftOffset * ContentScale);
            Canvas.SetTop(_substitute, LayoutTop + ContentTopOffset * ContentScale);
        }

        public void ReArrangeInner()
        {
            if (IsResizing)
            {
                SubstituteArrange();
            }
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    SubstituteArrange();

        //    return base.MeasureOverride(constraint);
        //}

        //protected override Size ArrangeOverride(Size arrangeSize)
        //{
        //    //SubstituteArrange();

        //    return base.ArrangeOverride(arrangeSize);
        //}

        public bool IsResizing
        {
            get => _substitute.Visibility == Visibility.Visible;
            set
            {
                SubstituteArrange();
                _substitute.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void Move(double offsetX, double offsetY)
        {
            //TODO: 减少一次PositonChanged的次数
            ContentLeft += offsetX;
            ContentTop += offsetY;
        }

        public static LayerInner FromLayerModel(LayerModel model, Canvas canvasLayout, Layer canvasContent)
        {
            LayerInner layer;
            switch (model.Key)
            {
                case LayerType.Text:
                    layer = new TextLayer(canvasLayout, canvasContent, ToObject<TextLayerModel>((JsonElement)model.Data));
                    break;
                case LayerType.Image:
                    layer = new ImageLayer(canvasLayout, canvasContent, ToObject<ImageLayerModel>((JsonElement)model.Data));
                    break;
                case LayerType.Ellipse:
                    layer = new ShapeEllipseLayer(canvasLayout, canvasContent, ToObject<ShapeLayerBaseModel>((JsonElement)model.Data));
                    break;
                case LayerType.Rectangle:
                    layer = new ShapeRectangleLayer(canvasLayout, canvasContent, ToObject<ShapeLayerBaseModel>((JsonElement)model.Data));
                    break;
                default:
                    layer = new LayerInner(canvasLayout, canvasContent);
                    break;
            }
            //layer.Model = model.Data;
            layer.ContentLeft = model.Left;
            layer.ContentTop = model.Top;
            return layer;
        }

        public static LayerInner FromKey(string key, Canvas canvasLayout, Layer canvasContent)
        {
            switch (key)
            {
                case LayerType.Text:
                    return new TextLayer(canvasLayout, canvasContent);
                case LayerType.Image:
                    return new ImageLayer(canvasLayout, canvasContent);
                case LayerType.Ellipse:
                    return new ShapeEllipseLayer(canvasLayout, canvasContent);
                case LayerType.Rectangle:
                    return new ShapeRectangleLayer(canvasLayout, canvasContent);
                default:
                    return new LayerInner(canvasLayout, canvasContent);
            }
        }

        public static T ToObject<T>(JsonElement element)
        {
            var json = element.GetRawText();
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            options.Converters.Add(new TypeConverterJsonAdapter());
            return JsonSerializer.Deserialize<T>(json, options);
        }

        //protected virtual void OnSizeChanged(double width, double height)
        //{

        //}
        public static double RoundUpToNearest(double passednumber, double roundto)
        {

            // 105.5 up to nearest 1 = 106
            // 105.5 up to nearest 10 = 110
            // 105.5 up to nearest 7 = 112
            // 105.5 up to nearest 100 = 200
            // 105.5 up to nearest 0.2 = 105.6
            // 105.5 up to nearest 0.3 = 105.6

            //if no rounto then just pass original number back
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return Math.Ceiling(passednumber / roundto) * roundto;
            }
        }
        public static double RoundDownToNearest(double passednumber, double roundto)
        {

            // 105.5 down to nearest 1 = 105
            // 105.5 down to nearest 10 = 100
            // 105.5 down to nearest 7 = 105
            // 105.5 down to nearest 100 = 100
            // 105.5 down to nearest 0.2 = 105.4
            // 105.5 down to nearest 0.3 = 105.3

            //if no rounto then just pass original number back
            if (roundto == 0)
            {
                return passednumber;
            }
            else
            {
                return Math.Floor(passednumber / roundto) * roundto;
            }
        }
    }
}
