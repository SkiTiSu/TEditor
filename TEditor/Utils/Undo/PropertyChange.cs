using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Utils.Undo
{
    internal class PropertyChange : IChange
    {
        readonly object target;
        readonly string propertyName;
        readonly object oldValue;
        object newValue;
        public DateTime Time { get; set; }

        public PropertyChange(object target, string propertyName, object oldValue, object newValue)
            : this(target, propertyName, oldValue, newValue, DateTime.Now)
        { }

        public PropertyChange(object target, string propertyName, object oldValue, object newValue, DateTime time)
        {
            this.target = target;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
            Time = time;
        }

        public void Undo()
        {
            target.GetType().GetProperty(propertyName).SetValue(target, oldValue, null);
        }

        public void Redo()
        {
            target.GetType().GetProperty(propertyName).SetValue(target, newValue, null);
        }

        public bool IsSameFinalTarget(IChange change)
        {
            if (change is not PropertyChange chan)
            {
                return false;
            }
            if (chan.target == target && chan.propertyName == propertyName)
            {
                return true;
            }
            return false;
        }

        public void Update(IChange change)
        {
            if (change is not PropertyChange chan)
            {
                return;
            }
            newValue = chan.newValue;
            Time = chan.Time;
        }

        public override string ToString()
        {
            return $"{propertyName}: {oldValue} -> {newValue}";
        }
    }
}
