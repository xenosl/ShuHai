namespace ShuHai.Unity.UI.Widgets
{
    public interface IWidgetLayouter
    {
        bool Dirty { get; }

        void MarkDirty();

        void RebuildLayout();
    }
}