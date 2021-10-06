using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;
using CsvHelper.Configuration.Attributes;
using TEditor.Layers;

namespace TEditor.Models
{
    public class TextLayerModel : BaseLayerModel
    {
        public string Text { get; set; } = "请修改文字";
        public string FontFamilyName { get; set; } = "微软雅黑";
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public double FontSize { get; set; } = 32;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
        public double LineHeight { get; set; } = 0;
        public uint TextSpaceNumber { get; set; } = 0;
        public Color Color { get; set; } = Colors.Black;

        public bool StrokeEnable { get; set; } = false;
        public StrokePosition StrokePosition { get; set; } = StrokePosition.Outside;
        public double StrokeThickness { get; set; } = 1;
        public Color StrokeColor { get; set; } = Colors.Black;

        public bool ShadowEnable { get; set; } = false;
        public double ShadowDepth { get; set; } = 10;
        public double ShadowDirection { get; set; } = 315;
        public Color ShadowColor { get; set; } = Colors.Black;
        public double ShadowOpacity { get; set; } = 0.5;
        public double ShadowBlurRadius { get; set; } = 10;

        public bool VariableEnable { get; set; } = false;
        public string VariableTemplate { get; set; }
    }
}
