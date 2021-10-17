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
            switch (Placement)
            {
                case PlacementAlignmentEnum.TopLeft:
                case PlacementAlignmentEnum.BottomRight:
                    return Cursors.SizeNWSE;
                    break;
                case PlacementAlignmentEnum.TopRight:
                case PlacementAlignmentEnum.BottomLeft:
                    return Cursors.SizeNESW;
                    break;
                case PlacementAlignmentEnum.Top:
                case PlacementAlignmentEnum.Bottom:
                    return Cursors.SizeNS;
                    break;
                case PlacementAlignmentEnum.Left:
                case PlacementAlignmentEnum.Right:
                    return Cursors.SizeWE;
                    break;
                case PlacementAlignmentEnum.Center:
                    return Cursors.Cross;
                    break;
                default:
                    return Cursors.Arrow;
                    break;
            }
        }

        /// <summary>Top left</summary>
        public static readonly PlacementAlignment TopLeft = new PlacementAlignment(PlacementAlignmentEnum.TopLeft);
        /// <summary>Top center</summary>
        public static readonly PlacementAlignment Top = new PlacementAlignment(PlacementAlignmentEnum.Top);
        /// <summary>Top right</summary>
        public static readonly PlacementAlignment TopRight = new PlacementAlignment(PlacementAlignmentEnum.TopRight);
        /// <summary>Center left</summary>
        public static readonly PlacementAlignment Left = new PlacementAlignment(PlacementAlignmentEnum.Left);
        /// <summary>Center center</summary>
        public static readonly PlacementAlignment Center = new PlacementAlignment(PlacementAlignmentEnum.Center);
        /// <summary>Center right</summary>
        public static readonly PlacementAlignment Right = new PlacementAlignment(PlacementAlignmentEnum.Right);
        /// <summary>Bottom left</summary>
        public static readonly PlacementAlignment BottomLeft = new PlacementAlignment(PlacementAlignmentEnum.BottomLeft);
        /// <summary>Bottom center</summary>
        public static readonly PlacementAlignment Bottom = new PlacementAlignment(PlacementAlignmentEnum.Bottom);
        /// <summary>Bottom right</summary>
        public static readonly PlacementAlignment BottomRight = new PlacementAlignment(PlacementAlignmentEnum.BottomRight);
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
