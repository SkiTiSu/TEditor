using CsvHelper;
using CsvHelper.Configuration;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TEditor.Layers;
using TEditor.Messages;
using TEditor.ViewModels;
using TEditor.Views;
using HandyControl.Controls;
using Window = HandyControl.Controls.Window;
using MessageBox = HandyControl.Controls.MessageBox;
using TEditor.Utils.Undo;
using TEditor.Models;
using CommandLine;
using TEditor.Common;

namespace TEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : GlowWindow
    {
        private LayerManager _layerManager;
        private UndoManager undoManager;

        private MainViewModel Vm { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalCache.Init();

            undoManager = new();

            _layerManager = new LayerManager(canvasLayout, canvasContent, undoManager);
            listBoxLayers.VM.LayerManager = _layerManager;
            _layerManager.OnSelectionChanged += _layerManager_OnSelectionChanged;
            _layerManager.LayerVisableChanged += _layerManager_LayerVisableChanged;

            Scale = 100;
            //TODO: 替身图层没有一起调整顺序

            // 条件格式

            WeakReferenceMessenger.Default.Register<ChangeLayerVisibleMessage>(this, (r, m) =>
            {
                LayerIdVisibleChangedByFormatCondition = m.Value.LayerId;
                // TODO 这里会不会有效率问题
                _layerManager.Layers.First(x => x.Id == m.Value.LayerId).Visible = m.Value.Visible;
            });


            Vm = new(_layerManager, undoManager)
            {
                CurrentFileName = "未命名-1.ted",
                Model = new()
            };
            this.DataContext = Vm;

            docControl = new()
            {
                DataContext = Vm.DocVm
            };
            SwitchToDocControl();
        }

        private void GlowWindow_ContentRendered(object sender, EventArgs e)
        {
            ArrangeControl();
            Parser.Default.ParseArguments<BatchGenOptions, object>(GlobalCache.Args)
                .WithParsed<BatchGenOptions>(BatchGenCmd)
                .WithNotParsed(HandleParseError);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

        void BatchGenCmd(BatchGenOptions opts)
        {
            ConsoleManager.AllocConsole();
            Console.WriteLine("TEditor by 四季天书 " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            Console.WriteLine("读取文件");
            Vm.OpenFile(opts.TedFile);
            using TextReader sr = File.OpenText(opts.DataSource);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
            };
            var reader = new CsvReader(sr, config);
            IEnumerable<dynamic> records = reader.GetRecords<dynamic>();
            ChangeDataTable(ToDataTable(records));
            Console.WriteLine("开始批量生成");
            BatchExportViewModel model = new()
            {
                ExportFolder = opts.OutputDirectory,
                FileNameTemplate = opts.Filename,
                StartAt = opts.Start,
                EndAt = opts.End,
                RepeatTimes = opts.Repeats,
                DeltaX = opts.OffsetX,
                DeltaY = opts.OffsetY,
            };
            BatchExport(model);
            Console.WriteLine("批量生成完成");
            Environment.Exit(0);
        }

        string LayerIdVisibleChangedByFormatCondition = string.Empty;

        private void _layerManager_LayerVisableChanged(object sender, Layer layer)
        {
            if (layer.Id == LayerIdVisibleChangedByFormatCondition)
            {
                LayerIdVisibleChangedByFormatCondition = "";
                return;
            }
            WeakReferenceMessenger.Default.Send(new LayerVisibleChangedMessage(new LayerVisible()
            {
                LayerId = layer.Id,
                Visible = layer.Visible,
            }));
            //TODO: 清除已删除图层
        }

        private void _layerManager_OnSelectionChanged(object sender, LayerInner e)
        {
            placeLayerControl.Children.Clear();
            if (e != null)
            {
                LayerCommonControl lc = new()
                {
                    DataContext = e
                };
                placeLayerControl.Children.Add(lc);
                placeLayerControl.Children.Add(e.LayerControl);
            }
            else
            {
                SwitchToDocControl();
            }
        }
        DocControl docControl;
        private void SwitchToDocControl()
        {
            placeLayerControl.Children.Add(docControl);
        }

        private int ScrollLeavePx = 80;
        private void ArrangeControl()
        {
            _layerManager.Arrage();
            
            double contentWidth = canvasContent.DesiredSize.Width;
            double contentHeight = canvasContent.DesiredSize.Height;
            if (contentWidth > canvasLayout.ActualWidth || contentHeight > canvasLayout.ActualHeight)
            {
                scrollBarHorizontal.Visibility = Visibility.Visible;
                scrollBarHorizontal.Maximum = canvasLayout.ActualWidth / 2 + contentWidth / 2 - ScrollLeavePx;
                scrollBarHorizontal.Minimum = -scrollBarHorizontal.Maximum;
                scrollBarHorizontal.ViewportSize = canvasLayout.ActualWidth;
                scrollBarHorizontal.Value = canvasLayout.ActualWidth / 2 - canvasContent.DesiredSize.Width / 2 - Canvas.GetLeft(canvasContent);

                scrollBarVertical.Visibility = Visibility.Visible;
                scrollBarVertical.Maximum = canvasLayout.ActualHeight / 2 + contentHeight / 2 - ScrollLeavePx;
                scrollBarVertical.Minimum = -scrollBarVertical.Maximum;
                scrollBarVertical.ViewportSize = canvasLayout.ActualHeight;
                scrollBarVertical.Value = canvasLayout.ActualHeight / 2 - canvasContent.DesiredSize.Height / 2 - Canvas.GetTop(canvasContent);

            }
            else
            {
                scrollBarHorizontal.Visibility = Visibility.Hidden;
                scrollBarVertical.Visibility = Visibility.Hidden;
            }
        }

        private void MoveCC(double dx, double dy)
        {
            Canvas.SetLeft(canvasContent, Canvas.GetLeft(canvasContent) + dx);
            Canvas.SetTop(canvasContent, Canvas.GetTop(canvasContent) + dy);
            ArrangeControl();
        }

        private void textboxCanvasScale_LostFocus(object sender, RoutedEventArgs e)
        {
            var text = (sender as System.Windows.Controls.TextBox).Text;
            text = text.Replace("%", "");
            int newScale;
            if (int.TryParse(text, out newScale))
            {
                Scale = newScale;
            }
            else
            {
                textboxCanvasScale.Text = Scale + "%";
            }
        }

        private int _scale = 100;
        private int Scale
        {
            get => _scale;
            set
            {
                _scale = Math.Max(10, value);
                textboxCanvasScale.Text = _scale + "%";
                double tscale = (double)_scale / 100;
                DpiScale dpi = VisualTreeHelper.GetDpi(canvasContent);
                tscale = tscale / dpi.DpiScaleX;
                canvasContent.LayoutTransform = new ScaleTransform(tscale, tscale);
                ArrangeControl();
            }
        }

        private void scrollBarHorizontal_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            Canvas.SetLeft(canvasContent, canvasLayout.ActualWidth / 2 - canvasContent.DesiredSize.Width / 2 - e.NewValue);
        }

        private void scrollBarVertical_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            Canvas.SetTop(canvasContent, canvasLayout.ActualHeight / 2 - canvasContent.DesiredSize.Height / 2 - e.NewValue);

        }

        #region 鼠标键盘快捷键
        bool isSpaceDown = false;
        bool isAltDown = false;

        bool autoSelectiveBak;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Space && !isSpaceDown)
            {
                isSpaceDown = true;
                canvasLayout.Cursor = Cursors.SizeAll;
                autoSelectiveBak = _layerManager.AutoSelective;
                _layerManager.AutoSelective = false;
                _layerManager.CancelSelectionAll();
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                isAltDown = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (key == Key.Space)
            {
                isSpaceDown = false;
                canvasLayout.Cursor = Cursors.Arrow;
                _layerManager.AutoSelective = autoSelectiveBak;
                e.Handled = true;
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                isAltDown = false;
                e.Handled = true;
            }
            //else if (key == Key.Delete && !(e.OriginalSource is TextBox))
            //{
            //    if (_layerManager.SelectedLayerInner != null)
            //    {
            //        _layerManager.Remove(_layerManager.SelectedLayerInner);
            //    }
            //}
        }

        private void canvasLayout_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isAltDown)
            {
                Scale += (e.Delta > 0) ? 10 : -10;
            }
        }
        #endregion

        bool isCLMouseDown = false;
        Point initPosition;
        private void canvasLayout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvasLayout.Focus();
            Keyboard.Focus(canvasLayout);
            isCLMouseDown = true;
            if (isSpaceDown)
            {
                initPosition = e.GetPosition(this);
            }
        }

        private void canvasLayout_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSpaceDown && isCLMouseDown)
            {
                Point currPosition = e.GetPosition(this);
                MoveCC(currPosition.X - initPosition.X, currPosition.Y - initPosition.Y);
                initPosition = e.GetPosition(this);
            }
        }

        private void canvasLayout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isCLMouseDown = false;
            Cursor = Cursors.Arrow;
        }

        private void canvasContentBackground_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _layerManager.CancelSelectionAll();
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new()
            {
                Filter = "PNG 图片|*.png",
                FileName = Vm.CurrentFileName.Substring(0, Vm.CurrentFileName.LastIndexOf("."))
            };
            if (dlg.ShowDialog() == true)
            {
                ExportMode = true;
                ExportElementToImage(canvasContent, dlg.FileName);
                ExportMode = false;
            }
        }

        private void ExportElementToImage(UIElement element, string filename, int step = 0, int deltaX = 0, int deltaY = 0)
        {
            Rect rect = new(element.RenderSize);
            RenderTargetBitmap bitmap = new(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);

            for (int i = 0; i <= step; i++)
            {
                if (i > 0)
                {
                    bool moveSuccess = moveDataSelection(1);
                    if (!moveSuccess)
                    {
                        break;
                    }
                    rect.X += deltaX;
                    rect.Y += deltaY;
                }
                element.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new Action(() => { }));

                DrawingVisual dVisual = new();
                using (DrawingContext dc = dVisual.RenderOpen())
                {
                    dc.DrawRectangle(new VisualBrush(element), null, rect);
                }
                bitmap.Render(dVisual);
            }

            PngBitmapEncoder encoder = new();
            //TODO: 是否能添加BitmapMetadata
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using FileStream file = File.OpenWrite(filename);
            encoder.Save(file);
        }

        private bool exportMode = false;
        public bool ExportMode
        {
            get => exportMode;
            set
            {
                if (value)
                {
                    canvasContentBackground.Visibility = Visibility.Hidden;
                    //wait for render complete
                    canvasContent.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new Action(() => { }));
                }
                else
                {
                    canvasContentBackground.Visibility = Visibility.Visible;
                }
            }
        }

        private DataTable currentDataTable;
        private void buttonImportTable_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("请从Excel中，连同标题行复制后，点击确定", "锵锵锵") != MessageBoxResult.OK)
            {
                return;
            }
            var dataobject = Clipboard.GetDataObject();
            string data_csv = (string)dataobject.GetData(DataFormats.UnicodeText);
            if (data_csv == null)
            {
                MessageBox.Show("格式有误，请检查");
                return;
            }
            TextReader sr = new StringReader(data_csv);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                Delimiter = "\t",
            };
            var reader = new CsvReader(sr, config);
            IEnumerable<dynamic> records = reader.GetRecords<dynamic>();
            ChangeDataTable(ToDataTable(records));
        }

        private void ChangeDataTable(DataTable table)
        {
            currentDataTable = table;
            dataGridMain.ItemsSource = currentDataTable.DefaultView;
            //UpdateVarText(0);
            dataGridMain.SelectedIndex = 0;
        }

        public static DataTable ToDataTable(IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            if (data.Count() == 0) return null;

            var dt = new DataTable();
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                dt.Columns.Add(key);
            }
            // TODO 如果猜测格式，可能导致不标准的无法成功导入
            // 每列设置Type，供排序使用
            //var firstRow = ((IDictionary<string, object>)data[0]).Values.ToArray();
            //for (int i = 0; i < firstRow.Count(); i++)
            //{
            //    var str = firstRow[i].ToString();
            //    if (!str.Contains(".") && int.TryParse(str, out var rnum))
            //    {
            //        dt.Columns[i].DataType = typeof(int);
            //    }
            //    else if (double.TryParse(str, out var rnumd))
            //    {
            //        dt.Columns[i].DataType = typeof(double);
            //    }
            //    else
            //    {
            //        //就是字符串
            //    }
            //}
            foreach (var d in data)
            {
                var row = ((IDictionary<string, object>)d).Values.ToArray();
                dt.Rows.Add(row);
            }
            return dt;
        }

        private void UpdateVar(int index)
        {
            foreach (var layer in _layerManager.Layers)
            {
                if (layer.Inner is ILayerWithVar layerVar)
                {
                    Dictionary<string, string> ret = new();
                    foreach (DataColumn column in currentDataTable.Columns)
                    {
                        ret.Add(column.ColumnName, currentDataTable.Rows[index][column.ColumnName].ToString());
                    }
                    layerVar.UpdateVar(ret);
                }
            }
        }

        private string ReplaceVarTemplate(string template, int index)
        {
            string re = template;
            foreach (DataColumn column in currentDataTable.Columns)
            {
                re = re.Replace("{" + column.ColumnName + "}", currentDataTable.Rows[index][column.ColumnName].ToString());
            }
            return re;
        }

        private void dataGridMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridMain.SelectedIndex < currentDataTable.Rows.Count && dataGridMain.SelectedIndex >= 0)
            {
                undoManager.CanAddChange = false;
                UpdateVar(dataGridMain.SelectedIndex);
                WeakReferenceMessenger.Default.Send(new DataSelectedRowChangedMessage(new DataSelectedRowChangedMessageArgs() 
                { 
                    Data = currentDataTable,
                    SelectedIndex = dataGridMain.SelectedIndex 
                }));
                undoManager.CanAddChange = true;
            }
        }

        private void canvasLayout_KeyDown(object sender, KeyEventArgs e)
        {
            // 方向键移动
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (_layerManager.SelectedLayerInner != null)
            {
                var layer = _layerManager.SelectedLayerInner;
                e.Handled = true;
                switch (key)
                {
                    case Key.Left:
                        layer.ContentLeft -= 1;
                        break;
                    case Key.Up:
                        layer.ContentTop -= 1;
                        break;
                    case Key.Right:
                        layer.ContentLeft += 1;
                        break;
                    case Key.Down:
                        layer.ContentTop += 1;
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
        }

        Window windowBatchExport;
        BatchExportView beView;
        private void buttonBatchExport_Click(object sender, RoutedEventArgs e)
        {
            windowBatchExport = new Window()
            {
                Width = 400,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            beView = new BatchExportView();
            beView.model.ExportFolder = Environment.CurrentDirectory;
            beView.model.StartAt = 1;
            beView.model.EndAt = dataGridMain.Items.Count;
            beView.buttonOK.Click += ButtonStartBatchExport_Click;
            beView.buttonCancel.Click += ButtonCancel_Click;
            beView.Padding = new Thickness(10);
            windowBatchExport.Content = beView;
            windowBatchExport.ShowDialog();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            windowBatchExport.Close();
        }

        private void ButtonStartBatchExport_Click(object sender, RoutedEventArgs e)
        {
            var model = beView.model;
            windowBatchExport.Close();
            BatchExport(model);
        }

        private void BatchExport(BatchExportViewModel model)
        {
            ExportMode = true;
            if (model.RepeatTimes == 0)
            {
                for (int i = model.StartAt - 1; i <= model.EndAt - 1; i++)
                {
                    dataGridMain.SelectedIndex = i;
                    string name = ReplaceVarTemplate(model.FileNameTemplate, i);
                    name = name.Replace("{index}", (i + 1).ToString());
                    ExportElementToImage(canvasContent, Path.Combine(model.ExportFolder, name + ".png"));
                }
            }
            else
            {
                for (int i = model.StartAt - 1; i <= model.EndAt - 1; i += model.RepeatTimes + 1)
                {
                    dataGridMain.SelectedIndex = i;
                    string name = ReplaceVarTemplate(model.FileNameTemplate, i);
                    name = name.Replace("{index}", $"{i + 1}-{i + 1 + model.RepeatTimes}");
                    ExportElementToImage(canvasContent, Path.Combine(model.ExportFolder, name + ".png"),
                        model.RepeatTimes, model.DeltaX, model.DeltaY);
                }
            }
            ExportMode = false;
        }

        private bool moveDataSelection(int delta)
        {
            // TODO 是用Items.Count还是currentDataTable.Rows.Count
            int newIndex = dataGridMain.SelectedIndex + delta;
            if (newIndex < dataGridMain.Items.Count && newIndex >= 0)
            {
                dataGridMain.SelectedIndex = newIndex;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GlowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Vm.IsEdited)
            {
                return;
            }
            var result = MessageBox.Show(
                this,
                "有未保存的更改！\r\n确认关闭TEditor吗？点击取消返回。",
                "关闭确认",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Cancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
