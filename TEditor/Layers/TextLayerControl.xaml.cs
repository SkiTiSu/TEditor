using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Controls.Primitives;
using TEditor.Layers;

namespace TEditor
{
    /// <summary>
    /// TextLayerControl.xaml 的交互逻辑
    /// </summary>
    public partial class TextLayerControl : UserControl
    {
        private TextLayer textLayer;

        public TextLayerControl()
        {
            InitializeComponent();
            this.DataContextChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                textLayer = e.NewValue as TextLayer;
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxFont.ItemsSource = GlobalCache.LocalizedFontFamily;
            comboBoxFont.SelectedItem = textLayer.FontFamily;
            comboBoxFontSize.ItemsSource = new int[] { 6, 8, 9, 10, 11, 12, 14, 16, 18, 24, 30, 36, 48, 60, 72 };
            comboBoxStrokePosition.ItemsSource = Enum.GetValues(typeof(StrokePosition)).Cast<StrokePosition>();
        }

        private void comboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily currentFont = comboBoxFont.SelectedItem as FontFamily;
            if (currentFont == null)
            {
                return;
            }
            textLayer.FontFamily = currentFont;
            List<FontWeight> list = new List<FontWeight>();
            foreach (var typeface in currentFont.FamilyTypefaces)
            {
                if (typeface.Style == FontStyles.Normal)
                {
                    list.Add(typeface.Weight);
                }
                //list.Add(typeface.AdjustedFaceNames.Values.First().ToString());
            }
            
            comboBoxFontWeight.ItemsSource = list;
            if (comboBoxFontWeight.SelectedIndex < 0)
            {
                comboBoxFontWeight.SelectedIndex = 0;
            }
        }

        private void toggleButtonBold_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;
            
            if (toggle.IsChecked == true)
            {
                textLayer.FontWeight = FontWeights.Bold;
            }
            else
            {
                textLayer.FontWeight = FontWeights.Normal;
            }
        }

        private void toggleButtonItalic_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;

            if (toggle.IsChecked == true)
            {
                textLayer.FontStyle = FontStyles.Italic;
            }
            else
            {
                textLayer.FontStyle = FontStyles.Normal;
            }
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)parameter == value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
