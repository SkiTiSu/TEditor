using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TEditor.Models;

namespace TEditor.Layers
{
    public class Layer : Canvas, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public LayerInner Inner { get; set; }
        Canvas _canvasLayout;
        Canvas _canvasContent;
        public Layer(string key, Canvas canvasLayout, Canvas canvasContent)
        {
            Init(canvasLayout, canvasContent);

            Inner = LayerInner.FromKey(key, _canvasLayout, this);
            this.Children.Add(Inner);
        }

        public Layer(LayerModel model, Canvas canvasLayout, Canvas canvasContent)
        {
            Init(canvasLayout, canvasContent);

            this.Id = model.Id;
            this.Visible = model.Visible;
            this.ZIndex = model.ZIndex;
            Inner = LayerInner.FromLayerModel(model, canvasLayout, this);
            this.Children.Add(Inner);
        }

        private void Init(Canvas canvasLayout, Canvas canvasContent)
        {
            this.Background = Brushes.Transparent;
            _canvasLayout = canvasLayout;
            _canvasContent = canvasContent;
            this.Width = _canvasContent.Width;
            this.Height = _canvasContent.Height;
            // TODO 一起刷新
        }

        // 背景不是null时，会被检测点击导致无法点击后面的内容，所以重写HitTest
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }

        public double ContentScale { get => _canvasContent.LayoutTransform.Value.M11; }

        public double ContentDisLeft { get => Canvas.GetLeft(_canvasContent); }
        public double ContentDisTop { get => Canvas.GetTop(_canvasContent); }
        public int ZIndex
        {
            get => Canvas.GetZIndex(this);
            set => Canvas.SetZIndex(this, value);
        }

        private bool visible = true;
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                if (visible)
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Hidden;

                }
            }
        }
        
        
        public bool ClippingMaskEnable { get; set; }
        // TODO: 这两个参数进Model
        /// <summary>
        /// 是否是剪贴蒙版提供蒙版的一层
        /// </summary>
        public bool ClippingMaskBottom { get; set; }

        public LayerModel ToLayerModel()
        {
            return new LayerModel()
            {
                Id = Id,
                Key = Inner.Key,
                ZIndex = ZIndex,
                Left = Inner.ContentLeft,
                Top = Inner.ContentTop,
                Visible = Visible,
                Data = Inner.Model,
            };
        }
    }
}