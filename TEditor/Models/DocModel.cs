using PostSharp.Patterns.Model;
using PostSharp.Patterns.Recording;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{
    [Recordable]
    [NotifyPropertyChanged]
    public class DocModel //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public DocModel()
        {
            FormatConditionGroups.Add(new FormatConditionGroupModel()
            {
                Name = "默认条件组",
            });
        }

        private double width = 1920;
        public double Width 
        {
            get => width;
            set
            {
                Debug.WriteLine("inner:" + value);
                width = value;
            }
        }
        public double Height { get; set; } = 1080;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; }

        [Reference]
        public ObservableCollection<FormatConditionGroupModel> FormatConditionGroups { get; set; } = new ObservableCollection<FormatConditionGroupModel>();
    }
}
