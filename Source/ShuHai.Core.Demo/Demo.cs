namespace ShuHai.Core.Demo
{
    public abstract class Demo
    {
        public string Name => GetType().Name;

        public abstract void Run();
    }
}
