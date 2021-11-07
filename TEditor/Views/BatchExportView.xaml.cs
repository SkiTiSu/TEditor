using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TEditor.ViewModels;

namespace TEditor.Views
{
    /// <summary>
    /// BatchExportView.xaml 的交互逻辑
    /// </summary>
    public partial class BatchExportView : UserControl
    {
        internal BatchExportViewModel model = new();
        public BatchExportView()
        {
            InitializeComponent();

            this.DataContext = model;
        }

        private void buttonSelectExportFolder_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                FileName = "保存在此处",
                Filter = "文件夹|文件夹"
            };

            if (dlg.ShowDialog() == true)
            {
                model.ExportFolder = Path.GetDirectoryName(dlg.FileName);
            }
        }
    }
}
