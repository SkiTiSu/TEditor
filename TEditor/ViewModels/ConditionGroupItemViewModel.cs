using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TEditor.Messages;
using TEditor.Models;
using TEditor.Views;

namespace TEditor.ViewModels
{
    public partial class ConditionGroupItemViewModel : ObservableRecipient
    {
        public ConditionGroupItemViewModel(FormatConditionGroupModel model)
        {
            Model = model;
            this.PropertyChanged += ConditionGroupItemViewModel_PropertyChanged;
        }

        private void ConditionGroupItemViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedCondition):
                    SelectedConditionChanged();
                    break;
                default:
                    break;
            }
        }

        public FormatConditionGroupModel Model { get; set; }

        public ObservableCollection<FormatConditionModel> Conditions => Model?.FormatConditionModels;
        public string Name { get => Model.Name; set => Model.Name = value; }
        public SolidColorBrush ColorBrush => new(Model.Color);

        public event EventHandler RemoveThis;

        [ObservableProperty]
        private FormatConditionModel selectedCondition;

        [ObservableProperty]
        private bool isEditLayersMode;

        [RelayCommand]
        public void DoubleClicked()
        {
            if (SelectedCondition != null)
            {
                EditCondition(SelectedCondition);
            }
            // TODO 是否需要refresh
        }

        public void SelectedConditionChanged()
        {
            if (SelectedCondition == null) { return; } //ItemSource刚设置后没有选中的
            foreach (var layerVisable in SelectedCondition.LayersVisable)
            {
                Messenger.Send(new ChangeLayerVisibleMessage(new LayerVisible()
                {
                    LayerId = layerVisable.Key,
                    Visible = layerVisable.Value,
                }));
            }
        }

        [RelayCommand]
        private void AddCondition()
        {
            var fcModel = new FormatConditionModel();
            EditCondition(fcModel);

            Conditions.Add(fcModel);
        }

        [RelayCommand]
        private void RemoveCondition()
        {
            if (Conditions.Count > 1)
            {
                Conditions.Remove(SelectedCondition);
            }
        }

        [RelayCommand]
        private void RemoveThisGroup()
        {
            RemoveThis?.Invoke(this, EventArgs.Empty);
        }

        private void EditCondition(FormatConditionModel model)
        {
            var windowCondition = new Window()
            {
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                //Owner = this,
            };
            var conditionControl = new FormatConditionControl
            {
                DataContext = model
            };
            conditionControl.buttonOK.Click += (ss, ee) =>
            {
                windowCondition.Close();
            };
            windowCondition.Content = conditionControl;
            windowCondition.ShowDialog();

            //TODO: 增加删除
        }
    }
}
