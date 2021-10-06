﻿using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.WebSockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using TEditor.Converters;
using TEditor.Layers;
using TEditor.Models;
using TEditor.Views;
using ToolGood.Algorithm;

namespace TEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        private LayerManager _layerManager;
        private DocModel model;
        private DocModel Model
        {
            get => model;
            set
            {
                model = value;
                canvasContent.Width = model.Width;
                canvasContentBackground.Width = model.Width;
                canvasContent.Height = model.Height;
                canvasContentBackground.Height = model.Height;
            }
        }

        private string currentFileName;
        private string CurrentFileName
        {
            get => currentFileName;
            set
            {
                currentFileName = value;
                this.Title = currentFileName + " - TEditor by 四季天书 技术预览版 0.0.4";
            }
        }

        private FormatConditionModel CurrentFormatConditoinModel 
            => listBoxCondition.SelectedItem as FormatConditionModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalCache.Init();

            _layerManager = new LayerManager(canvasLayout, canvasContent);
            listBoxLayers.ItemsSource = _layerManager.Layers;
            listBoxLayers.LayerManager = _layerManager;
            _layerManager.OnSelectionChanged += _layerManager_OnSelectionChanged;
            _layerManager.LayerVisableChanged += _layerManager_LayerVisableChanged;

            Model = new DocModel();
            CurrentFileName = "未命名-1.ted";
            Scale = 100;
            //TODO: 替身图层没有一起调整顺序
            SwitchToDocControl();
            Model.PropertyChanged += Model_PropertyChanged;
            listBoxCondition.ItemsSource = Model.FormatConditions;
            listBoxCondition.SelectedIndex = 0;
        }

        private void _layerManager_LayerVisableChanged(object sender, Layer layer)
        {
            CurrentFormatConditoinModel.LayersVisable[layer.Id] = layer.Visible;
            //TODO: 清除已删除图层
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Model = Model;
        }

        private void _layerManager_OnSelectionChanged(object sender, LayerInner e)
        {
            placeLayerControl.Children.Clear();
            if (e != null)
            {
                LayerCommonControl lc = new LayerCommonControl();
                lc.DataContext = e;
                placeLayerControl.Children.Add(lc);
                placeLayerControl.Children.Add(e.LayerControl);
            }
            else
            {
                SwitchToDocControl();
            }
        }

        private void SwitchToDocControl()
        {
            var docControl = new DocControl();
            docControl.DataContext = Model;
            placeLayerControl.Children.Add(docControl);
        }

        private void ArrangeControl()
        {
            _layerManager.Arrage();
        }

        private void MoveCC(double dx, double dy)
        {
            Canvas.SetLeft(canvasContent, Canvas.GetLeft(canvasContent) + dx);
            Canvas.SetTop(canvasContent, Canvas.GetTop(canvasContent) + dy);
            ArrangeControl();
        }

        private void textboxCanvasScale_LostFocus(object sender, RoutedEventArgs e)
        {
            var text = (sender as TextBox).Text;
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
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "PNG 图片|*.png",
                FileName = CurrentFileName.Substring(0, CurrentFileName.LastIndexOf("."))
            };
            if (dlg.ShowDialog() == true)
            {
                SaveContent(dlg.FileName);
            }
        }

        private void SaveContent(string filename)
        {
            canvasContentBackground.Visibility = Visibility.Hidden;
            //wait for render complete
            canvasContent.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new Action(() => { }));
            WriteToPng(canvasContent, filename);
            canvasContentBackground.Visibility = Visibility.Visible;
        }

        public void WriteToPng(UIElement element, string filename)
        {
            var rect = new Rect(element.RenderSize);
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            var bitmap = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            //TODO: 是否能添加BitmapMetadata
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var file = File.OpenWrite(filename))
            {
                encoder.Save(file);
            }
        }

        private DataTable currentDataTable;
        private void buttonImportTable_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("请连同标题行复制到剪贴板后，点击确定\r\n支持Excel、CSV文件直接复制", "锵锵锵") != MessageBoxResult.OK)
            {
                return;
            }
            var dataobject = Clipboard.GetDataObject();
            string data_csv = (string)dataobject.GetData(DataFormats.CommaSeparatedValue);
            TextReader sr = new StringReader(data_csv);
            var reader = new CsvReader(sr, CultureInfo.InvariantCulture);
            IEnumerable<dynamic> records = reader.GetRecords<dynamic>();
            currentDataTable = ToDataTable(records);
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
                    Dictionary<string, string> ret = new Dictionary<string, string>();
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

        private void CheckFormatCondition(int index)
        {
            AlgorithmEngine engine = new AlgorithmEngine();
            foreach (DataColumn column in currentDataTable.Columns)
            {
                string value = currentDataTable.Rows[index][column.ColumnName].ToString();
                if (double.TryParse(value, out double dvalue))
                {
                    engine.AddParameter(column.ColumnName, dvalue);
                }
                else
                {
                    engine.AddParameter(column.ColumnName, value);
                }
            }
            FormatConditionModel defaultCondition = null;
            foreach (var condition in Model.FormatConditions)
            {
                if (condition.Name == FormatConditionModel.DEFAULT_NAME)
                {
                    defaultCondition = condition;
                    continue;
                }
                bool isMatch = engine.TryEvaluate(condition.Condition, false);
                if (isMatch)
                {
                    listBoxCondition.SelectedItem = condition;
                    return;
                }
            }
            if (defaultCondition != null)
            {
                listBoxCondition.SelectedItem = defaultCondition;
            }
        }

        private void dataGridMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridMain.SelectedIndex < currentDataTable.Rows.Count && dataGridMain.SelectedIndex >= 0)
            {
                UpdateVar(dataGridMain.SelectedIndex);
                CheckFormatCondition(dataGridMain.SelectedIndex);
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "TEditor文件|*.ted";
            sfd.FileName = CurrentFileName;
            if (sfd.ShowDialog() == true)
            {
                TEditorFile file = new TEditorFile
                {
                    DocModel = Model,
                    Layers = _layerManager.GetLayerModels()
                };

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                };
                options.Converters.Add(new TypeConverterJsonAdapter());

                string json = JsonSerializer.Serialize(file, options);
                File.WriteAllText(sfd.FileName, json);

                CurrentFileName = sfd.SafeFileName;
            }
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = $"TEditor文件|*.ted|所有文件|*.*";
            if (dlg.ShowDialog() == true)
            {
                string json = File.ReadAllText(dlg.FileName);
                var file = JsonSerializer.Deserialize<TEditorFile>(json);
                //TODO: 错误处理
                CurrentFileName = dlg.SafeFileName;
                Model = file.DocModel;
                _layerManager.SetLayerModels(file.Layers);
                listBoxCondition.ItemsSource = Model.FormatConditions;
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
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            beView = new BatchExportView();
            beView.buttonOK.Click += ButtonStartBatchExport_Click;
            beView.buttonCancel.Click += ButtonCancel_Click;
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
            for (int i = 0; i <= currentDataTable.Rows.Count - 1; i++)
            {
                dataGridMain.SelectedIndex = i;
                string name = ReplaceVarTemplate(model.FileNameTemplate, i);
                name = name.Replace("{index}", (i + 1).ToString());
                SaveContent(System.IO.Path.Combine(model.ExportFolder, name + ".png"));
            }
        }

        private void buttonConditionAdd_Click(object sender, RoutedEventArgs e)
        {
            var fcModel = new FormatConditionModel();
            editCondition(fcModel);

            Model.FormatConditions.Add(fcModel);
        }

        private void listBoxCondition_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxCondition.SelectedItem != null)
            {
                editCondition(listBoxCondition.SelectedItem as FormatConditionModel);
            }
            listBoxCondition.Items.Refresh();
        }

        private void editCondition(FormatConditionModel model)
        {
            var windowCondition = new Window()
            {
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
            };
            var conditionControl = new FormatConditionControl();
            conditionControl.DataContext = model;
            conditionControl.buttonOK.Click += (ss, ee) =>
            {
                windowCondition.Close();
            };
            windowCondition.Content = conditionControl;
            windowCondition.ShowDialog();

            //TODO: 增加删除
        }

        private void listBoxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fcModel = CurrentFormatConditoinModel;
            if (fcModel == null) { return; } //ItemSource刚设置后没有选中的

            foreach (var layer in _layerManager.Layers)
            {
                if (fcModel.LayersVisable.ContainsKey(layer.Id))
                {
                    layer.Visible = fcModel.LayersVisable[layer.Id];
                }
                else
                {
                    layer.Visible = true;
                }
            }
        }

        private void buttonConditionRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCondition.Items.Count > 1)
            {
                Model.FormatConditions.Remove(CurrentFormatConditoinModel);
            }
        }

        #region 添加图层
        private void buttonAddText_Click(object sender, RoutedEventArgs e)
        {
            _layerManager.AddWithKey(LayerType.Text);
        }

        private void buttonAddImage_Click(object sender, RoutedEventArgs e)
        {
            _layerManager.AddWithKey(LayerType.Image);
        }

        private void buttonAddEllipse_Click(object sender, RoutedEventArgs e)
        {
            _layerManager.AddWithKey(LayerType.Ellipse);
            //var vb = new VisualBrush(le);
            //lr.OpacityMask = vb;

        }

        private void buttonAddRectangle_Click(object sender, RoutedEventArgs e)
        {
            _layerManager.AddWithKey(LayerType.Rectangle);
        }
        #endregion
    }
}
