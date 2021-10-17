using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TEditor.Converters;
using TEditor.Layers;
using TEditor.Models;

namespace TEditor
{
    public class LayerManager
    {
        Canvas _canvasLayout;
        Canvas _canvasContent;
        AdornerLayer _adornerLayer;

        public const int LayersIndexMax = 999;
        public int ZIndexToIndex(int zIndex)
            => LayersIndexMax - zIndex;

        public ObservableCollection<Layer> Layers { get; } = new ObservableCollection<Layer>();

        public event EventHandler<LayerInner> OnSelectionChanged;
        public event EventHandler<Layer> LayerVisableChanged;
        public bool AutoZIndex { get; set; } = true;

        double ContentScale { get => _canvasContent.LayoutTransform.Value.M11; }

        public bool AutoSelective { get; set; } = true;
        private LayerInner _selectedLayer;
        public LayerInner SelectedLayerInner {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                OnSelectionChanged?.Invoke(this, _selectedLayer);
            }
        }

        public LayerManager(Canvas canvasLayout, Canvas canvasContent)
        {
            _canvasLayout = canvasLayout;
            _canvasContent = canvasContent;
            _adornerLayer = AdornerLayer.GetAdornerLayer(_canvasLayout);

            _canvasLayout.MouseMove += _canvasLayout_MouseMove;
            _canvasLayout.MouseLeftButtonDown += _canvasLayout_MouseLeftButtonDown;
            _canvasLayout.MouseLeftButtonUp += _canvasLayout_MouseLeftButtonUp;

            Layers.CollectionChanged += Layers_CollectionChanged;
        }

        private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (AutoZIndex)
            {
                AutoResetZIndex();
            }
        }

        private void _canvasLayout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastPosition = e.GetPosition(_canvasLayout);
        }

        private void _canvasLayout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            DraggingLayerInner = null;
        }

        private void _canvasLayout_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && DraggingLayerInner != null)
            {
                //防止鼠标正好点在Thumb上导致canvasLayout的MouseLeftButtonUp无法触发 还有BUG，点了thumb，但是在其他地方拖拽还是会响应
                if (e.LeftButton == MouseButtonState.Released) 
                {
                    isDragging = false;
                    return;
                }
                var currentPosition = e.GetPosition(_canvasLayout);
                DraggingLayerInner.LayoutLeft = DraggingLayerInner.LayoutLeft + currentPosition.X - lastPosition.X;
                DraggingLayerInner.LayoutTop = DraggingLayerInner.LayoutTop + currentPosition.Y - lastPosition.Y;
                lastPosition = currentPosition;
            }
        }

        public void Add(Layer layer)
        {
            var layerInner = layer.Inner;

            layerInner.MouseDown += Layer_MouseDown;
            layerInner.SubstituteMouseLeftButtonDown += Layer_SubstituteMouseLeftButtonDown;
            layerInner.PropertyChanged += LayerInner_PropertyChanged;
            layer.PropertyChanged += Layer_PropertyChanged;

            _canvasContent.Children.Add(layer);

            Layers.Insert(0, layer);
        }

        public Layer AddWithKey(string key)
        {
            var layer = new Layer(key, _canvasLayout, _canvasContent);
            Add(layer);
            return layer;
        }
        public Layer AddFromModel(LayerModel model)
        {
            var layer = new Layer(model, _canvasLayout, _canvasContent);
            Add(layer);
            return layer;
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LayerInner.Visible):
                    LayerVisableChanged?.Invoke(this, sender as Layer);
                    break;
            }
        }

        private void LayerInner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LayerInner.IsResizing):
                    // 目前只支持单选

                    //var layer = sender as Layer;
                    //if (layer.IsResizing)
                    //{
                    //    OnSelectionChanged?.Invoke(this, layer);
                    //    return;
                    //}
                    //OnSelectionChanged?.Invoke(this, null);

                    foreach (var layer in Layers)
                    {
                        if (layer.Inner.IsResizing)
                        {
                            SelectedLayerInner = layer.Inner;
                            return;
                        }
                    }
                    SelectedLayerInner = null;
                    break;
            }
        }

        private void Layer_SubstituteMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DraggingLayerInner = sender as LayerInner;
        }

        Point lastPosition;
        bool isDragging = false;
        LayerInner DraggingLayerInner;

        public void Remove(Layer layer)
        {
            _canvasContent.Children.Remove(layer);
            //TODO: 清除替身更好的方法
            _canvasLayout.Children.Remove(layer.Inner._substitute);
            Layers.Remove(layer);
        }

        public void RemoveAll()
        {
            //Content有Backgroud层，暂时不能直接清
            foreach (var layer in Layers)
            {
                _canvasContent.Children.Remove(layer);
                _canvasLayout.Children.Remove(layer.Inner._substitute);
            }
            Layers.Clear();
        }

        private void Layer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AutoSelective)
            {
                var layer = sender as LayerInner;
                Select(layer);
                DraggingLayerInner = layer;
            }
        }

        public void Arrage()
        {
            foreach (var layer in Layers)
            {
                layer.Inner.ReArrangeInner();
            }
        }

        //TODO: 更好的正在选择判断方法
        //public Layer SelectingLayer
        //{
        //    get
        //    {
        //        foreach (var layer in Layers)
        //        {
        //            if (layer.IsResizing)
        //            {
        //                return layer;
        //            }
        //        }
        //        return null;
        //    }
        //}

        public void Select(LayerInner layerInner)
        {
            //SelectingLayer = layer;
            layerInner.IsResizing = true;
            foreach (var _layer in Layers)
            {
                var inner = _layer.Inner;
                if (inner != layerInner && inner.IsResizing)
                {
                    inner.IsResizing = false;
                }
            }
        }

        public void CancelSelectionAll()
        {
            //SelectingLayer = null;
            foreach (var layer in Layers)
            {
                var inner = layer.Inner;
                if (inner.IsResizing)
                {
                    inner.IsResizing = false;
                }
            }
        }

        public void EnableClippingMask(Layer layer)
        {
            Brush newOpacityMask = null;
            layer.ClippingMaskEnable = true;
            for (int i = ZIndexToIndex(layer.ZIndex) + 1; i < Layers.Count; i++)
            {
                if (Layers[i].ClippingMaskEnable)
                {
                    continue;
                }
                else
                {
                    // 如果已经是Bottom重新设置一遍也没关系
                    Layers[i].ClippingMaskBottom = true;
                    newOpacityMask = new VisualBrush(Layers[i]);
                    layer.OpacityMask = newOpacityMask;
                    break;
                }
            }
            if (layer.ClippingMaskBottom)
            {
                layer.ClippingMaskBottom = false;
                // 查找上面的图层重新确定OpacityMask
                if (newOpacityMask != null)
                {
                    for (int i = ZIndexToIndex(layer.ZIndex) - 1; i >= 0; i--)
                    {
                        if (Layers[i].ClippingMaskEnable)
                        {
                            Layers[i].OpacityMask = newOpacityMask;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void DisableClippingMask(Layer layer)
        {
            layer.ClippingMaskEnable = false;
            layer.OpacityMask = null;
            for (int i = ZIndexToIndex(layer.ZIndex) + 1; i < Layers.Count; i++)
            {
                if (Layers[i].ClippingMaskEnable)
                {
                    // 下面还有剪贴蒙版图层，不用继续管了
                    break;
                }
                else if (Layers[i].ClippingMaskBottom)
                {
                    Layers[i].ClippingMaskBottom = false;
                    break;
                }
            }
            // 查找上面的图层有没有剪贴图层，如有则取消
            for (int i = ZIndexToIndex(layer.ZIndex) - 1; i >= 0; i--)
            {
                if (Layers[i].ClippingMaskEnable)
                {
                    Layers[i].ClippingMaskEnable = false;
                    Layers[i].OpacityMask = null;
                }
                else
                {
                    break;
                }
            }
        }

        public void RefreshClippingMask()
        {
            List<Layer> LayersNeedMask = new();
            foreach (var layer in Layers)
            {
                if (layer.ClippingMaskEnable)
                {
                    LayersNeedMask.Add(layer);
                }
                else if (layer.ClippingMaskBottom)
                {
                    foreach (var layerNeedMask in LayersNeedMask)
                    {
                        layerNeedMask.OpacityMask = new VisualBrush(layer);
                    }
                    LayersNeedMask.Clear();
                }
            }
        }

        public void AutoResetZIndex()
        {
            int i = LayersIndexMax;
            foreach(var layer in Layers)
            {
                layer.ZIndex = i; //TODO 这样设置ZIndex是否正确
                i--;
            }
        }

        public LayerModel[] GetLayerModels()
        {
            List<LayerModel> layerModels = new List<LayerModel>();
            foreach (var layer in Layers)
            {
                layerModels.Add(layer.ToLayerModel());
            }
            return layerModels.ToArray();
        }

        public void SetLayerModels(IEnumerable<LayerModel> models)
        {
            var layerList = models.OrderBy(x => x.ZIndex);
            this.AutoZIndex = false;
            RemoveAll();
            foreach (var model in layerList)
            {
                this.AddFromModel(model);
            }
            this.AutoZIndex = true;
        }
    }
}
