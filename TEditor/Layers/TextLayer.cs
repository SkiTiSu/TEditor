﻿using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TEditor.Models;

namespace TEditor.Layers
{
    public class TextLayer : LayerInner, ILayerWithVar
    {
        public override FrameworkElement LayerControl { get; protected set; } = new TextLayerControl();
        public new string LayerName { get => Text.Substring(0, Math.Min(Text.Length, 8)); }
        public override string Type { get; } = "文字";
        public override string Key { get; } = LayerType.Text;
        public override object Model
        {
            get => model;
            set
            {
                model = value as TextLayerModel;
                if (model != null)
                {
                    ReInit();
                }
            }
        }

        private TextLayerModel model = new TextLayerModel();

        FormattedText formattedText;

        public string Text
        {
            get => model.Text;
            set
            {
                model.Text = value;
                ReInit();
            }
        }

        private void ReInit()
        {
            formattedText = new FormattedText(
                GetTextAddSpace(),
                CultureInfo.GetCultureInfo("zh-cn"),
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal),
                FontSize,
                new SolidColorBrush(Color),
                1.0);
            formattedText.MaxLineCount = 999;
            formattedText.Trimming = TextTrimming.CharacterEllipsis; //注意这里要暴露给用户设置为妙
            formattedText.TextAlignment = TextAlignment;
            formattedText.LineHeight = LineHeight;
        }

        private string GetTextAddSpace()
        {
            string space = "";
            for (int j = 0; j < TextSpaceNumber; j++)
            {
                space += "\u200A"; // Hair Space，约为1/8 em
            }
            char[] textArray = Text.ToCharArray();
            return string.Join(space, textArray);
            
        }

        public new double ActualWidth { get => formattedText?.Width ?? 0; }
        public new double ActualHeight { get => formattedText?.Height ?? 0; }
        public TextLayer(Canvas canvasLayout, Layer canvasContent, object model)
            : base(canvasLayout, canvasContent)
        {
            Model = model;
            ReInit();
            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            //formattedText.MaxTextWidth = 300;
            //formattedText.MaxTextHeight = 240;

            // Use a Bold font weight beginning at the 6th character and continuing for 11 characters.
            //formattedText.SetFontWeight(FontWeights.Bold, 6, 11);

            // Use a linear gradient brush beginning at the 6th character and continuing for 11 characters.
            //formattedText.SetForegroundBrush(
            //                        new LinearGradientBrush(
            //                        Colors.Orange,
            //                        Colors.Teal,
            //                        90.0),
            //                        6, 11);
            this.PropertyChanged += TextLayer_PropertyChanged;
        }

        public TextLayer(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new TextLayerModel())
        {

        }

        private void TextLayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Render();
        }

        //Typeface typeface;
        //public Typeface Typeface
        //{
        //    get => typeface;
        //    set
        //    {
        //        typeface = value;
        //        formattedText.SetFontTypeface(typeface);
        //        Render();
        //    }
        //}

        public FontFamily FontFamily
        {
            get => new FontFamily(model.FontFamilyName);
            set
            {
                model.FontFamilyName = value.ToString();
                formattedText.SetFontFamily(value);
            }
        }

        public FontWeight FontWeight
        {
            get => model.FontWeight;
            set
            {
                model.FontWeight = value;
                formattedText.SetFontWeight(value);
            }
        }

        public double FontSize
        {
            get => model.FontSize;
            set
            {
                model.FontSize = value;
                formattedText.SetFontSize(model.FontSize);
            }
        }

        public FontStyle FontStyle
        {
            get => model.FontStyle;
            set
            {
                model.FontStyle = value;
                formattedText.SetFontStyle(model.FontStyle);
            }
        }
        
        //TODO: 文本框模式
        public TextAlignment TextAlignment
        {
            get => model.TextAlignment;
            set
            {
                model.TextAlignment = value;
                formattedText.TextAlignment = value;
            }
        }

        public double LineHeight
        {
            get => model.LineHeight;
            set
            {
                model.LineHeight = value;
                formattedText.LineHeight = value;
            }
        }

        public uint TextSpaceNumber
        {
            get => model.TextSpaceNumber;
            set 
            {
                model.TextSpaceNumber = value;
                ReInit();
            }
        }

        public Color Color
        {
            get => model.Color;
            set
            {
                model.Color = value;
                formattedText.SetForegroundBrush(new SolidColorBrush(model.Color));
            }
        }

        public bool StrokeEnable 
        { 
            get => model.StrokeEnable;
            set => model.StrokeEnable = value;
        }
        public StrokePosition StrokePosition 
        { 
            get => model.StrokePosition;
            set => model.StrokePosition = value;
        }
        public double StrokeThickness 
        { 
            get => model.StrokeThickness;
            set => model.StrokeThickness = value;
        }
        public Color StrokeColor 
        { 
            get => model.StrokeColor;
            set => model.StrokeColor = value;
        }

        public bool ShadowEnable 
        { 
            get => model.ShadowEnable;
            set => model.ShadowEnable = value;
        }
        public double ShadowDepth
        {
            get => model.ShadowDepth;
            set => model.ShadowDepth = value;
        }
        public double ShadowDirection
        {
            get => model.ShadowDirection;
            set => model.ShadowDirection = value;
        }
        public Color ShadowColor
        {
            get => model.ShadowColor;
            set => model.ShadowColor = value;
        }
        public double ShadowOpacity
        {
            get => model.ShadowOpacity;
            set => model.ShadowOpacity = value;
        }
        public double ShadowBlurRadius
        {
            get => model.ShadowBlurRadius;
            set => model.ShadowBlurRadius = value;
        }

        public bool VariableEnable 
        { 
            get => model.VariableEnable;
            set => model.VariableEnable = value;
        }
        public bool VariableEnableInverse { get => !VariableEnable; }
        public string VariableTemplate
        {
            get => model.VariableTemplate;
            set => model.VariableTemplate = value;
        }

        private void Render()
        {
            this.InvalidateVisual();
        }

        public override double Width
        {
            get => formattedText.Width;
            set
            {
                formattedText.MaxTextWidth = value;
            }
        }

        public override double Height
        {
            get => formattedText.Height;
            set
            {
                formattedText.MaxTextHeight = value;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //drawingContext.DrawText(formattedText, new Point(0, 0));
            var _textGeometry = formattedText.BuildGeometry(new Point(0, 0));
 
            drawingContext.DrawGeometry(
                new SolidColorBrush(Color),
                null,
                _textGeometry
            );

            if (StrokeEnable)
            {
                if (StrokePosition == StrokePosition.Outside)
                {
                    var boundsGeo = new RectangleGeometry(new Rect(0, 0, Width, Height));
                    var _clipGeometry = Geometry.Combine(boundsGeo, _textGeometry, GeometryCombineMode.Exclude, null);
                    drawingContext.PushClip(_clipGeometry);
                }
                else if (StrokePosition == StrokePosition.Inside)
                {
                    drawingContext.PushClip(_textGeometry);
                }

                if (StrokePosition == StrokePosition.Outside || StrokePosition == StrokePosition.Inside)
                {
                    drawingContext.DrawGeometry(
                        null,
                        new Pen(new SolidColorBrush(StrokeColor), StrokeThickness * 2),
                        _textGeometry
                    );
                    drawingContext.Pop();
                }
                else
                {
                    drawingContext.DrawGeometry(
                        null,
                        new Pen(new SolidColorBrush(StrokeColor), StrokeThickness),
                        _textGeometry
                    );
                }
            }

            //阴影如果导出的时候背景是透明则无法显示 bug
            if (ShadowEnable)
            {
                DropShadowEffect dropShadowEffect = new DropShadowEffect()
                {
                    ShadowDepth = ShadowDepth,
                    Direction = ShadowDirection,
                    Color = ShadowColor,
                    Opacity = ShadowOpacity,
                    BlurRadius = ShadowBlurRadius
                };
                this.Effect = dropShadowEffect;
            }
            else if (this.Effect != null)
            {
                this.Effect = null;
            }

            switch (TextAlignment)
            {
                case TextAlignment.Center:
                    ContentLeftOffset = -Width / 2;
                    break;
                case TextAlignment.Right:
                    ContentLeftOffset = -Width;
                    break;
                default:
                    ContentLeftOffset = 0;
                    break;
            }

            this.ReArrangeInner();
            //this.Width = formattedText.Width;
            //this.Height = formattedText.Height;
        }

        protected sealed override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            Rect r = new Rect(0, 0, Width, Height);

            if (r.Contains(hitTestParameters.HitPoint))
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
            return null;
        }

        public void UpdateVar(Dictionary<string, string> keyValues)
        {
            if (VariableEnable)
            {
                string realText = VariableTemplate;
                foreach (var keyValue in keyValues)
                {
                    realText = realText.Replace("{" + keyValue.Key + "}", keyValue.Value);
                }
                Text = realText;
            }
        }
    }

    public enum StrokePosition
    {
        Center,
        Outside,
        Inside
    }
}
