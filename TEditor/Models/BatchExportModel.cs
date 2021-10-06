using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TEditor.Common;

namespace TEditor.Models
{
    public class BatchExportModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ExportFolder { get; set; }
        public string FileNameTemplate { get; set; } = "Rank_{index}";
    }
}
