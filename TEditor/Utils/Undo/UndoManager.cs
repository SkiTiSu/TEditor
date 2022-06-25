using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Utils.Undo
{
    public class UndoManager
    {
        public int MaxUndoSteps { get; set; } = 20;
        // TODO 后续可以增加全局的鼠标按下、按键反复按下的检测，光用时间还是有点暴力
        public TimeSpan DebounceTime { get; set; } = TimeSpan.FromMilliseconds(400);
        public event EventHandler StatusChanged;
        public bool CanAddChange { get; set; } = true;

        LinkedList<IChange> UndoStack = new();
        LinkedList<IChange> RedoStack = new();
        bool haveUncompleteChange = false;
        DateTime lastUndoRedoTime = DateTime.Now;

        public IReadOnlyCollection<IChange> UndoList => UndoStack;

        public void AddChange(IChange change)
        {
            if (!CanAddChange) { return; }

            // 忽略Undo和Redo操作带来的重复
            if (haveUncompleteChange) 
            {
                haveUncompleteChange = false;
                return;
            }
            // 防止Undo和Redo操作带来的重复（会出发多次事件的变更）
            if (change.Time - lastUndoRedoTime < DebounceTime)
            {
                return;
            }

            int compareToIndex = UndoStack.Count > 5 ? 5 : UndoStack.Count;

            if (compareToIndex > 0)
            {
                for (int i = 0 ; i < compareToIndex; i++)
                {
                    var lastChange = UndoStack.Skip(i).First();
                    if (lastChange.IsSameFinalTarget(change))
                    {
                        // 防止移动鼠标的操作造成的大量变更事件
                        if (change.Time - lastChange.Time < DebounceTime)
                        {
                            lastChange.Update(change);
                            return;
                        }
                    }
                }
            }

            UndoStack.AddFirst(change);
            if (UndoStack.Count > MaxUndoSteps)
            {
                UndoStack.RemoveLast();
            }
            // 当有重做操作后，如果新增的变更不是UndoRedo带来的，清空Redo栈（无法再重做）
            RedoStack.Clear();

            StatusChanged?.Invoke(this, null);
        }

        public bool CanUndo() => UndoStack.Count > 0;
        public void Undo()
        {
            if (!CanUndo()) { return; }
            IChange change = UndoStack.First();
            UndoStack.RemoveFirst();
            RedoStack.AddFirst(change);
            lastUndoRedoTime = DateTime.Now;
            haveUncompleteChange = true;
            change.Undo();
            StatusChanged?.Invoke(this, null);
        }

        public bool CanRedo() => RedoStack.Count > 0;
        public void Redo()
        {
            if (!CanRedo()) { return; }
            IChange change = RedoStack.First();
            RedoStack.RemoveFirst();
            UndoStack.AddFirst(change);
            lastUndoRedoTime = DateTime.Now;
            haveUncompleteChange = true;
            change.Redo();
            StatusChanged?.Invoke(this, null);
        }
    }
}
