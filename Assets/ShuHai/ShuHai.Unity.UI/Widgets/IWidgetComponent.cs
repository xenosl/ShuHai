namespace ShuHai.Unity.UI
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