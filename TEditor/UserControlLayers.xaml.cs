using System;
using System.Collections;
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
using System.Collections.ObjectModel;
using TEditor.Layers;

namespace TEditor
{
    /// <summary>
    /// UserControlLayers.xaml 的交互逻辑
    /// </summary>
    public partial class UserControlLayers : UserControl
    {
        private ObservableCollection<Layer> _layers;
        public ObservableCollection<Layer> ItemsSource
        {
            get => _layers; 
            set 
            {
                _layers = value;
                listBoxLayers.ItemsSource = value; 
            } 
        }

        public LayerManager LayerManager { get; set; }
        public UserControlLayers()
        {
            InitializeComponent();
        }


        private bool isDragging;
        private Point firstPoint;
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
                        DragDrop.DoDragDrop(draggedItem, new DataObject("layer", draggedItem.DataContext), DragDropEffects.Move);
                    }
                }
            }
            else
            {
                isDragging = false;
            }
        }

        private void listbox_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                Layer droppedData = e.Data.GetData("layer") as Layer;
                Layer target = ((ListBoxItem)(sender)).DataContext as Layer;

                int removedIdx = listBoxLayers.Items.IndexOf(droppedData);
                int targetIdx = listBoxLayers.Items.IndexOf(target);

                if (removedIdx < targetIdx)
                {
                    _layers.Insert(targetIdx + 1, droppedData);
                    _layers.RemoveAt(removedIdx);
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (listBoxLayers.Items.Count + 1 > remIdx)
                    {
                        _layers.Insert(targetIdx, droppedData);
                        _layers.RemoveAt(remIdx);
                    }
                }
                //ItemsSource = _layers;
                
            }
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            Layer layer = GetLayerFromMenuItem(sender);
            LayerManager.Remove(layer);
        }

        private void MenuEnableClippingMask_Click(object sender, RoutedEventArgs e)
        {
            Layer layer = GetLayerFromMenuItem(sender);
            LayerManager.EnableClippingMask(layer);
        }

        private void MenuDisableClippingMask_Click(object sender, RoutedEventArgs e)
        {
            Layer layer = GetLayerFromMenuItem(sender);
            LayerManager.DisableClippingMask(layer);
        }

        private Layer GetLayerFromMenuItem(object sender)
        {
            return ((((sender as MenuItem).Parent as ContextMenu)
                .Parent as System.Windows.Controls.Primitives.Popup)
                .PlacementTarget as ListBoxItem)
                .DataContext as Layer;
        }
    }
}
