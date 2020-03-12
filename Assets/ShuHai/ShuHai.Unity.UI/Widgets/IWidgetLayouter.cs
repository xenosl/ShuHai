namespace ShuHai.Unity.UI
{
    public interface IWidgetLayouter
    {
        bool Dirty { get; }

        void MarkDirty();

        void RebuildLayout();
    }
}