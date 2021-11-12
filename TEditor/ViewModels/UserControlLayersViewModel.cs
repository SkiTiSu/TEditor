using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TEditor.Layers;
using TEditor.Models;

namespace TEditor.ViewModels
{
    public partial class UserControlLayersViewModel : ObservableObject
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(Layers))]
        private LayerManager layerManager;

        public ObservableCollection<Layer> Layers
            => LayerManager?.Layers;

        private Layer SelectedLayer => LayerManager.SelectedLayer;

        [ICommand]
        private void MenuDelete() => 
            LayerManager.Remove(SelectedLayer);

        [ICommand]
        private void MenuEnableClippingMask() =>
            LayerManager.EnableClippingMask(SelectedLayer);

        [ICommand]
        private void MenuDisableClippingMask() =>
            LayerManager.DisableClippingMask(SelectedLayer);

        [ICommand]
        private void DuplicateLayer()
        {
            if (LayerManager.SelectedLayerInner != null)
            {
                var layer = LayerManager.SelectedLayerInner.Parent as Layer;
                var model = layer.ToLayerModel();
                model.Id = Guid.NewGuid().ToString();
                // TODO 改进以避免序列化反序列化
                string json = JsonSerializer.Serialize(model, GlobalConfig.Instance.JsonOptions);
                LayerModel layerModel = JsonSerializer.Deserialize<LayerModel>(json);

                LayerManager.AddFromModel(layerModel);
            }
        }

        [ICommand]
        private void DeleteLayer()
        {
            if (SelectedLayer != null)
            {
                LayerManager.Remove(SelectedLayer);
            }
        }

        [ObservableProperty]
        private bool dragShowTop;
        [ObservableProperty]
        private bool dragShowBottom;
    }
}
