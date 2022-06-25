using System;

namespace TEditor.Utils.Undo
{
    public interface IChange
    {
        DateTime Time { get; set; }

        void Redo();
        void Undo();

        bool IsSameFinalTarget(IChange change);
        void Update(IChange change);
    }
}