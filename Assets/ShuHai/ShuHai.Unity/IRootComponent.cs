namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a logical component that attached to <see cref="Root" />
    /// </summary>
    /// <remarks>
    ///     A default constructor is required for all classes that implemented the <see langword="interface" /> since the
    ///     <see cref="Root" /> class instantiate the component instance using the default constructor.
    /// </remarks>
    public interface IRootComponent
    {
        void Initialize();
        void Deinitialize();

        void FixedUpdate();
        void Update();
        void LateUpdate();
    }
}