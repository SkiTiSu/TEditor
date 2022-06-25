using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using TEditor.Layers;
using TEditor.ViewModels;

namespace TEditor
{
    /// <summary>
    /// UserControlLayers.xaml 的交互逻辑
    /// </summary>
    public partial class UserControlLayers : UserControl
    {
        public UserControlLayersViewModel VM { get; } = new();
        public IList<Layer> Layers => VM.Layers;

        public UserControlLayers()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private bool isDragging;
        private Point firstPoint;
        private int draggingIndex;
        private void ListBoxItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                if (!isDragging)
                {
                    isDragging = true;
                    firstPoint = e.GetPosition(this);
                }
                else
                {
                    if ((e.GetPosition(this) - firstPoint).Length > 5)
                    {
                        ListBoxItem draggedItem = sender as ListBoxItem;
                        draggingIndex = listBoxLayers.Items.IndexOf(draggedItem.DataContext as Layer);
                        DragDrop.DoDragDrop(draggedItem, new DataObject("layer", draggedItem.DataContext), DragDropEffects.Move);
                    }
                }
            }
            else
            {
                isDragging = false;
            }
        }

        private void ListBoxItem_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                Layer target = item.DataContext as Layer;
                int targetIndex = listBoxLayers.Items.IndexOf(target);

                if (draggingIndex == targetIndex)
                {
                    VM.DragShowTop = false;
                    VM.DragShowBottom = false;
                }
                else if (draggingIndex > targetIndex)
                {
                    VM.DragShowTop = true;
                    VM.DragShowBottom = false;
                }
                else
                {
                    VM.DragShowTop = false;
                    VM.DragShowBottom = true;
                }
            }
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                Layer droppedData = e.Data.GetData("layer") as Layer;
                Layer target = item.DataContext as Layer;

                int targetIndex = listBoxLayers.Items.IndexOf(target);

                if (draggingIndex < targetIndex)
                {
                    Layers.Insert(targetIndex + 1, droppedData);
                    Layers.RemoveAt(draggingIndex);
                }
                else
                {
                    int remIdx = draggingIndex + 1;
                    if (listBoxLayers.Items.Count + 1 > remIdx)
                    {
                        Layers.Insert(targetIndex, droppedData);
                        Layers.RemoveAt(remIdx);
                    }
                }
            }
        }
    }
}
