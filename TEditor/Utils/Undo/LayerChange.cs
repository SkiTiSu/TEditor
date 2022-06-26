using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEditor.Layers;

namespace TEditor.Utils.Undo
{
    public class LayerChange : IChange
    {
        public DateTime Time { get; set; }
        LayerManager layerManager;
        Layer layer;
        LayerChangeAction action;

        public LayerChange(LayerManager layerManager, Layer layer, LayerChangeAction action)
        {
            Time = DateTime.Now;
            this.layerManager = layerManager;
            this.layer = layer;
            this.action = action;
        }

        public bool IsSameFinalTarget(IChange change)
            => false; // 目前只会有一个LayerManager实例
        public void Undo()
        {
            switch (action)
            {
                case LayerChangeAction.Add:
                    layerManager.Remove(layer);
                    break;
                case LayerChangeAction.Remove:
                    layerManager.Add(layer);
                    break;
            }
        }

        public void Redo()
        {
            switch (action)
            {
                case LayerChangeAction.Add:
                    layerManager.Add(layer);
                    break;
                case LayerChangeAction.Remove:
                    layerManager.Remove(layer);
                    break;
            }
        }

        public void Update(IChange change)
        {
        }

        public override string ToString()
        {
            string actionString = action switch
            {
                LayerChangeAction.Add => "新增",
                LayerChangeAction.Remove => "删除",
                _ => "",
            };
            return actionString + "图层: " + layer.Inner.LayerNameDisplay;
        }
    }

    public enum LayerChangeAction
    {
        Add,
        Remove,
    }
}
