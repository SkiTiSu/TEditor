using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEditor.Models;

namespace TEditor.ViewModels
{
    public partial class DocViewModel : ObservableObject
    {
        //public DocViewModel(DocModel model)
        //{
        //    Model = model;
        //}

        private DocModel model;
        public DocModel Model 
        { 
            get => model;
            set
            {
                model = value;
                FormatConditionGroupsVm = new(model.FormatConditionGroups);
            }
        }

        [ObservableProperty]
        private FormatConditionGroupsViewModel formatConditionGroupsVm;

        public double Width
        {
            get => model.Width;
            set => SetProperty(model.Width, value, model, (u, n) => u.Width = n);
        }

        public double Height
        {
            get => model.Height;
            set => SetProperty(model.Height, value, model, (u, n) => u.Height = n);
        }
    }
}
