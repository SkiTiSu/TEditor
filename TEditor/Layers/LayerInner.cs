﻿using System;
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
using TEditor.Utils.Undo;

namespace TEditor
{
    [Serializable]
    public class LayerInner : Canvas, INotifyPropertyChanged, IUndoHasIgnore
    {
        public double GridSubdivisions = 0.5;
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangeDetailEventHandler(object sender, string propertyName, object before, object after);
        public event PropertyChangeDetailEventHandler PropertyChangeDetail;

        public string LayerNameDisplay => string.IsNullOrWhiteSpace(LayerNameCustom) ? LayerName : LayerNameCustom;
        [AlsoNotifyFor(nameof(LayerNameDisplay))]
        public virtual string LayerName { get; set; } = "图层";
        [AlsoNotifyFor(nameof(LayerNameDisplay))]
        public string LayerNameCustom { get; set; }
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

        public Layer ParentCanvas;
        public Substitute _substitute;
        public ResizeAdorner resizeAdorner;

        double ContentScale { get => ParentCanvas.ContentScale; }
        double ContentDisLeft { get => ParentCanvas.ContentDisLeft; }
        double ContentDisTop { get => ParentCanvas.ContentDisTop; }

        public event MouseButtonEventHandler SubstituteMouseLeftButtonDown;

        public LayerInner(Layer canvasContent)
            : base()
        {
            ParentCanvas = canvasContent;

            _substitute = new Substitute
            {
                Visibility = Visibility.Collapsed,
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
            };
            _substitute.MouseLeftButtonDown += Substitute_MouseLeftButtonDown;

            if (LayerControl is not DefaultLayerControl)
            {
                LayerControl.DataContext = this;
            }
        }

        private void Substitute_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubstituteMouseLeftButtonDown(this, e);
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

        // Binding无法识别隐藏基类的new属性
        [AlsoNotifyFor(nameof(Size))]
        public double WidthBinding
        {
            get => Width;
            set 
            {
                Width = value;
                ReArrangeInner();
            }
        }

        public new virtual double Height
        {
            get => base.Height;
            set => base.Height = value;
        }

        [AlsoNotifyFor(nameof(Size))]
        public double HeightBinding
        {
            get => Height;
            set
            {
                Height = value;
                ReArrangeInner();
            }
        }

        public Size Size
        {
            get => new(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
                ReArrangeInner();
            }
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
            get => new(LayoutLeft, LayoutTop);
            set
            {
                LayoutLeft = value.X;
                LayoutTop = value.Y;
            }
        }

        [DoNotNotify]
        public double LayoutLeft
        {
            get => ContentDisLeft + ContentLeft * ContentScale;
            set
            {
                ContentLeft = (value - ContentDisLeft) / ContentScale;
            }
        }

        [DoNotNotify]
        public double LayoutTop
        {
            get => ContentDisTop + ContentTop * ContentScale;
            set
            {
                ContentTop = (value - ContentDisTop) / ContentScale;
            }
        }

        [DoNotNotify]
        public double ContentLeftOffset { get; set; } = 0;
        [DoNotNotify]
        public double ContentTopOffset { get; set; } = 0;

        public Point ContentPosition
        {
            get => new(ContentLeft, ContentTop);
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
            LayerInner layer = model.Key switch
            {
                LayerType.Text => new TextLayer(canvasLayout, canvasContent, ToObject<TextLayerModel>((JsonElement)model.Data)),
                LayerType.Image => new ImageLayer(canvasLayout, canvasContent, ToObject<ImageLayerModel>((JsonElement)model.Data)),
                LayerType.Ellipse => new ShapeEllipseLayer(canvasLayout, canvasContent, ToObject<ShapeLayerBaseModel>((JsonElement)model.Data)),
                LayerType.Rectangle => new ShapeRectangleLayer(canvasLayout, canvasContent, ToObject<ShapeRectangleLayerModel>((JsonElement)model.Data)),
                _ => new LayerInner(canvasContent),
            };
            //layer.Model = model.Data;
            layer.LayerNameCustom = model.LayerNameCustom;
            layer.ContentLeft = model.Left;
            layer.ContentTop = model.Top;
            return layer;
        }

        public static LayerInner FromKey(string key, Canvas canvasLayout, Layer canvasContent)
        {
            return key switch
            {
                LayerType.Text => new TextLayer(canvasLayout, canvasContent),
                LayerType.Image => new ImageLayer(canvasLayout, canvasContent),
                LayerType.Ellipse => new ShapeEllipseLayer(canvasLayout, canvasContent),
                LayerType.Rectangle => new ShapeRectangleLayer(canvasLayout, canvasContent),
                _ => new LayerInner(canvasContent),
            };
        }

        public static T ToObject<T>(JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, GlobalConfig.Instance.JsonOptions);
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

        // 此方法由Fody自动调用
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            PropertyChangeDetail?.Invoke(this, propertyName, before, after);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<string> UndoIgnore => new[]
        {
            nameof(ContentLeft),
            nameof(ContentTop),
            nameof(IsResizing),
            nameof(LayerName),
            nameof(LayerNameDisplay),
            nameof(LayoutHeight),
            nameof(LayoutWidth),
            nameof(HeightBinding),
            nameof(WidthBinding),
            nameof(Height),
            nameof(Width),

        };
    }
}
