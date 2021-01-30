namespace ShuHai.Unity.UI.Widgets
{
    public interface IWidgetComponent
    {
        Widget Widget { get; }
    }

    public interface IWidgetComponent<out T> : IWidgetComponent
        where T : Widget
    {
        new T Widget { get; }
    }
}