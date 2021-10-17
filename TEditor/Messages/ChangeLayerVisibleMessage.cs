using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Messages
{
    public class LayerVisible
    {
        public string LayerId { get; set; }
        public bool Visible { get; set; }
    }

    public class ChangeLayerVisibleMessage : ValueChangedMessage<LayerVisible>
    {
        public ChangeLayerVisibleMessage(LayerVisible arg) : base(arg)
        { }
    }

    public class LayerVisibleChangedMessage : ValueChangedMessage<LayerVisible>
    {
        public LayerVisibleChangedMessage(LayerVisible arg) : base(arg)
        { }
    }
}
