using Microsoft.Toolkit.Mvvm.ComponentModel;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Recording;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEditor.Models;

namespace TEditor.ViewModels
{
    //[DoNotNotify]
    [Recordable]
    [NotifyPropertyChanged]
    public partial class DocViewModel
    {
        //public DocViewModel(DocModel model)
        //{
        //    Model = model;
        //}

        [Child]
        [NotRecorded]
        private DocModel model;
        [NotRecorded]
        public DocModel Model 
        { 
            get => model;
            set
            {
                model = value;
                //FormatConditionGroupsVm = new(model.FormatConditionGroups);
            }
        }

        [Reference]
        //[ObservableProperty]
        private FormatConditionGroupsViewModel formatConditionGroupsVm;

        [RecordingScope("设置画布宽度")]
        public double Width { get => model.Width; set => model.Width = value; }

        //private double width = 1000;
        //public double Width
        //{
        //    get => width;
        //    set
        //    {
        //        Debug.WriteLine("out:" + value);
        //        width = value;
        //    }
        //}
        //public double Width
        //{
        //    get => model.Width;
        //    set
        //    {
        //        Debug.WriteLine(value);
        //        model.Width = value;
        //        //SetProperty(model.Width, value, model, (u, n) => u.Width = n); 
        //    }
        //}

        public double Height { get => model.Height; set => model.Height = value; }
        //public double Height
        //{
        //    get => model.Height;
        //    set => SetProperty(model.Height, value, model, (u, n) => u.Height = n);
        //}
    }
}
