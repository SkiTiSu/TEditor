﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TEditor.Models;

namespace TEditor.Layers
{
    public abstract class ShapeLayerBase<T> : LayerInner 
        where T : ShapeLayerBaseModel, new()
    {
        public ShapeLayerBase(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new T())
        {

        }

        public ShapeLayerBase(Canvas canvasLayout, Layer canvasContent, T model)
            : base(canvasContent)
        {
            Model = model;
        }

        internal T model;
        public override object Model 
        { 
            get => model;
            set
            {
                model = value as T;
                Width = model.Width;
                Height = model.Height;
                ReInit();
            }
        }

        public override FrameworkElement LayerControl { get; protected set; } = new ShapeLayerControl();

        public Color FillColor 
        { 
            get => model.FillColor;
            set
            {
                model.FillColor = value;
                ReInit();
            }
        }

        public Color BorderColor
        {
            get => model.BorderColor;
            set
            {
                model.BorderColor = value;
                ReInit();
            }
        }

        public double BorderWidth
        {
            get => model.BorderWidth;
            set
            {
                model.BorderWidth = value;
                ReInit();
            }
        }

        public override double Width
        {
            get => model.Width;
            set
            {
                base.Width = value;
                model.Width = value;
            }
        }

        public override double Height
        {
            get => model.Height;
            set
            {
                base.Height = value;
                model.Height = value;
            }
        }

        public Pen BorderPen
            => (BorderWidth > 0) ? new Pen(new SolidColorBrush(BorderColor), BorderWidth) : null;

        public void ReInit()
        {
            InvalidateVisual();
        }
    }
}