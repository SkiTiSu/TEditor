using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TEditor
{
    //借鉴了ICSharpCode.WpfDesign的思想
    public class PlacementAlignment
    {
        public HorizontalAlignment Horizontal { get; private set; }
        public VerticalAlignment Vertical { get; private set; }

        public PlacementAlignmentEnum Placement { get; private set; }

        public PlacementAlignment(PlacementAlignmentEnum placement)
        {
            Placement = placement;
            switch (placement)
            {
                case PlacementAlignmentEnum.Top:
                    Horizontal = HorizontalAlignment.Center;
                    Vertical = VerticalAlignment.Top;
                    break;
                case PlacementAlignmentEnum.TopLeft:
                    Horizontal = HorizontalAlignment.Left;
                    Vertical = VerticalAlignment.Top;
                    break;
                case PlacementAlignmentEnum.TopRight:
                    Horizontal = HorizontalAlignment.Right;
                    Vertical = VerticalAlignment.Top;
                    break;
                case PlacementAlignmentEnum.Center:
                    Horizontal = HorizontalAlignment.Center;
                    Vertical = VerticalAlignment.Center;
                    break;
                case PlacementAlignmentEnum.Left:
                    Horizontal = HorizontalAlignment.Left;
                    Vertical = VerticalAlignment.Center;
                    break;
                case PlacementAlignmentEnum.Right:
                    Horizontal = HorizontalAlignment.Right;
                    Vertical = VerticalAlignment.Center;
                    break;
                case PlacementAlignmentEnum.Bottom:
                    Horizontal = HorizontalAlignment.Center;
                    Vertical = VerticalAlignment.Bottom;
                    break;
                case PlacementAlignmentEnum.BottomLeft:
                    Horizontal = HorizontalAlignment.Left;
                    Vertical = VerticalAlignment.Bottom;
                    break;
                case PlacementAlignmentEnum.BottomRight:
                    Horizontal = HorizontalAlignment.Right;
                    Vertical = VerticalAlignment.Bottom;
                    break;
            }
        }

        public Cursor GetCursor()
        {
            return Placement switch
            {
                PlacementAlignmentEnum.TopLeft or PlacementAlignmentEnum.BottomRight => Cursors.SizeNWSE,
                PlacementAlignmentEnum.TopRight or PlacementAlignmentEnum.BottomLeft => Cursors.SizeNESW,
                PlacementAlignmentEnum.Top or PlacementAlignmentEnum.Bottom => Cursors.SizeNS,
                PlacementAlignmentEnum.Left or PlacementAlignmentEnum.Right => Cursors.SizeWE,
                PlacementAlignmentEnum.Center => Cursors.Cross,
                _ => Cursors.Arrow,
            };
        }

        /// <summary>Top left</summary>
        public static readonly PlacementAlignment TopLeft = new(PlacementAlignmentEnum.TopLeft);
        /// <summary>Top center</summary>
        public static readonly PlacementAlignment Top = new(PlacementAlignmentEnum.Top);
        /// <summary>Top right</summary>
        public static readonly PlacementAlignment TopRight = new(PlacementAlignmentEnum.TopRight);
        /// <summary>Center left</summary>
        public static readonly PlacementAlignment Left = new(PlacementAlignmentEnum.Left);
        /// <summary>Center center</summary>
        public static readonly PlacementAlignment Center = new(PlacementAlignmentEnum.Center);
        /// <summary>Center right</summary>
        public static readonly PlacementAlignment Right = new(PlacementAlignmentEnum.Right);
        /// <summary>Bottom left</summary>
        public static readonly PlacementAlignment BottomLeft = new(PlacementAlignmentEnum.BottomLeft);
        /// <summary>Bottom center</summary>
        public static readonly PlacementAlignment Bottom = new(PlacementAlignmentEnum.Bottom);
        /// <summary>Bottom right</summary>
        public static readonly PlacementAlignment BottomRight = new(PlacementAlignmentEnum.BottomRight);
    }

    public enum PlacementAlignmentEnum
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Top,
        Bottom,
        Left,
        Right,
        Center
    }
}
