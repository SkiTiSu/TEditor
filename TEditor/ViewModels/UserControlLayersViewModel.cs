﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        [NotifyPropertyChangedFor(nameof(Layers))]
        private LayerManager layerManager;

        public ObservableCollection<Layer> Layers
            => LayerManager?.Layers;

        private Layer SelectedLayer => LayerManager.SelectedLayer;

        [RelayCommand]
        private void MenuDelete() => 
            LayerManager.Remove(SelectedLayer);

        [RelayCommand]
        private void MenuEnableClippingMask() =>
            LayerManager.EnableClippingMask(SelectedLayer);

        [RelayCommand]
        private void MenuDisableClippingMask() =>
            LayerManager.DisableClippingMask(SelectedLayer);

        [RelayCommand]
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

        [RelayCommand]
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
