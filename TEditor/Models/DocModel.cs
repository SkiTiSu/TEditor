using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{
    public class DocModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DocModel()
        {
            FormatConditions.Add(new FormatConditionModel()
            {
                Name = FormatConditionModel.DEFAULT_NAME,
            });
        }

        public double Width { get; set; } = 1920;
        public double Height { get; set; } = 1080;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; }

        public ObservableCollection<FormatConditionModel> FormatConditions { get; set; } = new ObservableCollection<FormatConditionModel>();
    }
}
