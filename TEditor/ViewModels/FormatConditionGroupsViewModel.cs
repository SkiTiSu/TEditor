using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEditor.Messages;
using TEditor.Models;
using ToolGood.Algorithm;

namespace TEditor.ViewModels
{
    public partial class FormatConditionGroupsViewModel : ObservableObject, 
        IRecipient<DataSelectedRowChangedMessage>, IRecipient<LayerVisibleChangedMessage>
    {
        //[AlsoNotifyChangeFor(nameof(Vms))]
        [ObservableProperty]
        public ObservableCollection<FormatConditionGroupModel> groups;

        public FormatConditionGroupsViewModel(ObservableCollection<FormatConditionGroupModel> model)
        {
            Groups = model;
            Vms = new ObservableCollection<ConditionGroupItemViewModel>(Groups?.Select((model) => new ConditionGroupItemViewModel(model)));
            foreach (var vm in Vms)
            {
                vm.RemoveThis += RemoveGroup;
            }
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            var group = sender as ConditionGroupItemViewModel;
            Groups.Remove(group.Model);
            Vms.Remove(group);
        }

        public ObservableCollection<ConditionGroupItemViewModel> Vms { get; set; }

        [RelayCommand]
        private void AddGroup()
        {
            FormatConditionGroupModel model = new()
            {
                Name = "新建条件组"
            };
            Groups.Add(model);
            ConditionGroupItemViewModel vm = new(model);
            vm.RemoveThis += RemoveGroup;
            Vms.Add(vm);
        }

        public void Receive(DataSelectedRowChangedMessage message)
        {
            var data = message.Value.Data;
            var index = message.Value.SelectedIndex;

            AlgorithmEngine engine = new AlgorithmEngine();
            foreach (DataColumn column in data.Columns)
            {
                string value = data.Rows[index][column.ColumnName].ToString();
                if (double.TryParse(value, out double dvalue))
                {
                    engine.AddParameter(column.ColumnName, dvalue);
                }
                else
                {
                    engine.AddParameter(column.ColumnName, value);
                }
            }

            foreach (var vm in Vms)
            {
                FormatConditionModel defaultCondition = null;
                bool isMatched = false;
                foreach (var condition in vm.Conditions)
                {
                    if (condition.Name == FormatConditionModel.DEFAULT_NAME)
                    {
                        defaultCondition = condition;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(condition.Condition))
                    {
                        continue;
                    }
                    bool isMatch = engine.TryEvaluate(condition.Condition, false);
                    if (isMatch)
                    {
                        vm.SelectedCondition = condition;
                        isMatched = true;
                        break;
                    }
                }
                if (defaultCondition != null && !isMatched)
                {
                    vm.SelectedCondition = defaultCondition;
                }
            }
        }

        public void Receive(LayerVisibleChangedMessage message)
        {
            var layerId = message.Value.LayerId;
            var visible = message.Value.Visible;

            foreach (var vm in Vms)
            {
                if (vm.IsEditLayersMode)
                {
                    vm.Model.EffctiveLayers.Add(layerId);
                    foreach (var condition in vm.Model.FormatConditionModels)
                    {
                        condition.LayersVisable[layerId] = visible;
                    }
                }
                if (vm.Model.EffctiveLayers.Contains(layerId))
                {
                    vm.SelectedCondition.LayersVisable[layerId] = visible;
                    // 当前仅允许图层被一个条件组控制
                    break;
                }
            }
        }
    }
}
