using System;

namespace TEditor.Utils.Undo
{
    public interface IChange
    {
        DateTime Time { get; set; }
        void Undo();
        void Redo();
        bool IsSameFinalTarget(IChange change);
        void Update(IChange change);
    }
}