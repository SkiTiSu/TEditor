using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Messages
{
    public class DataSelectedRowChangedMessage : ValueChangedMessage<DataSelectedRowChangedMessageArgs>
    {
        public DataSelectedRowChangedMessage(DataSelectedRowChangedMessageArgs data) : base(data)
        { }
    }

    public class DataSelectedRowChangedMessageArgs
    {
        public DataTable Data { get; set; }
        public int SelectedIndex { get; set; }
    }
}
