using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.ViewModels
{
    public partial class BatchExportViewModel : ObservableObject
    {
        [ObservableProperty]
        private string exportFolder;
        [ObservableProperty]
        private string fileNameTemplate = "Rank_{index}";
        [ObservableProperty]
        private int startAt;
        [ObservableProperty]
        private int endAt;
        [ObservableProperty]
        private int repeatTimes = 0;
        [ObservableProperty]
        private int deltaX = 0;
        [ObservableProperty]
        private int deltaY = 0;
    }
}
