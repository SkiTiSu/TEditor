using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TEditor.Layers;

namespace TEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly LayerManager layerManager;

        public DocViewModel DocVm { get; private set; }

        private TEditorFile model;
        public TEditorFile Model { 
            get => model;
            set
            {
                model = value;
                layerManager.SetLayerModels(Model.Layers);
                DocVm.Model = model.DocModel;
                //OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }
        }

        public MainViewModel()
        {
            DocVm = new();
        }
        public MainViewModel(LayerManager layerManager) : this()
        {
            this.layerManager = layerManager;
        }

        [ObservableProperty]
        private string title;

        private string currentFileName;
        public string CurrentFileName
        {
            get => currentFileName;
            set
            {
                currentFileName = value;
                Title = currentFileName + " - TEditor by 四季天书 " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            }
        }

        [ICommand]
        public void Save()
        {
            SaveFileDialog sfd = new()
            {
                Filter = "TEditor文件|*.ted",
                FileName = CurrentFileName,
            };
            if (sfd.ShowDialog() == true)
            {
                DocVm.Model.UpdatedAt = DateTime.Now;

                TEditorFile file = ToTEditorFile();

                string json = JsonSerializer.Serialize(file, GlobalConfig.Instance.JsonOptions);
                File.WriteAllText(sfd.FileName, json);

                CurrentFileName = sfd.SafeFileName;
            }
        }

        [ICommand]
        public void Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = $"TEditor文件|*.ted|所有文件|*.*";
            if (dlg.ShowDialog() == true)
            {
                string json = File.ReadAllText(dlg.FileName);
                var file = JsonSerializer.Deserialize<TEditorFile>(json, GlobalConfig.Instance.JsonOptions);
                //TODO: 错误处理
                CurrentFileName = dlg.SafeFileName;
                Model = file;

                layerManager.RefreshClippingMask();
            }
        }

        [ICommand]
        public void AddText()
        {
            layerManager.AddWithKey(LayerType.Text);
        }

        [ICommand]
        public void AddImage()
        {
            layerManager.AddWithKey(LayerType.Image);
        }

        [ICommand]
        public void AddEllipse()
        {
            layerManager.AddWithKey(LayerType.Ellipse);
        }

        [ICommand]
        public void AddRectangle()
        {
            layerManager.AddWithKey(LayerType.Rectangle);
        }

        public TEditorFile ToTEditorFile()
        {
            return new TEditorFile()
            {
                DocModel = DocVm.Model,
                Layers = layerManager.GetLayerModels(),
            };
        }
    }
}
