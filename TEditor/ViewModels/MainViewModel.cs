using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using TEditor.Utils.Undo;

namespace TEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly LayerManager layerManager;
        private readonly UndoManager undoManager;

        public DocViewModel DocVm { get; private set; }

        private TEditorFile model;
        public TEditorFile Model { 
            get => model;
            set
            {
                // 这里相当于状态回归到没有对新文件/打开的文件做任何更改
                model = value;
                layerManager.SetLayerModels(Model.Layers);
                DocVm.Model = model.DocModel;
                //OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
                IsEdited = false;
                RefreshTitle();
            }
        }

        private bool isEdited;
        public bool IsEdited 
        {
            get => isEdited;
            set
            {
                isEdited = value;
                RefreshTitle();
            }
        }

        public MainViewModel()
        {
            DocVm = new();
        }
        public MainViewModel(LayerManager layerManager, UndoManager undoManager) : this()
        {
            this.layerManager = layerManager;
            this.undoManager = undoManager;
            undoManager.StatusChanged += UndoManager_StatusChanged;
        }

        [ObservableProperty]
        private string title;

        public string CurrentFileName { get; set; }

        private void RefreshTitle()
        {
            string editedSign = IsEdited ? "*" : "";
            Title = CurrentFileName + editedSign + " - TEditor by 四季天书 " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        [RelayCommand]
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
                IsEdited = false;
            }
        }

        [RelayCommand]
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

        [RelayCommand]
        public void AddText()
        {
            layerManager.AddWithKey(LayerType.Text);
        }

        [RelayCommand]
        public void AddImage()
        {
            layerManager.AddWithKey(LayerType.Image);
        }

        [RelayCommand]
        public void AddEllipse()
        {
            layerManager.AddWithKey(LayerType.Ellipse);
        }

        [RelayCommand]
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

        [RelayCommand(CanExecute = nameof(CanUndo))]
        public void Undo()
        {
            undoManager.Undo();
        }

        public bool CanUndo()
            => undoManager.CanUndo();

        [RelayCommand(CanExecute = nameof(CanRedo))]
        public void Redo()
        {
            undoManager.Redo();
        }

        public bool CanRedo()
            => undoManager.CanRedo();

        private void UndoManager_StatusChanged(object sender, EventArgs e)
        {
            if (!IsEdited) { IsEdited = true; }
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
            UndoList = null;
            OnPropertyChanged(nameof(UndoList));
            UndoList = undoManager.UndoList;
            OnPropertyChanged(nameof(UndoList));
        }

        public IReadOnlyCollection<IChange> UndoList { get; set; }
    }
}
