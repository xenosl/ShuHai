using System;
using System.Collections.Generic;

namespace ShuHai
{
    /// <summary>
    ///     A generic object pool to avoid heap allocation.
    /// </summary>
    /// <typeparam name="T">Type of object to be pooled.</typeparam>
    public static class Pool<T>
        where T : class, new()
    {
        /// <summary>
        ///     Get a pooled object of type <see cref="T" />. Heap allocation is performed if there's no free object.
        /// </summary>
        public static Pooled<T> Get() { return _objects.Count == 0 ? new Pooled<T>() : _objects.Pop(); }

        internal static void Dispose(Pooled<T> element) { _objects.Push(element); }

        private static readonly Stack<Pooled<T>> _objects = new Stack<Pooled<T>>();
    }

    public sealed class Pooled<T> : IDisposable
        where T : class, new()
    {
        public readonly T Value = new T();

        public bool Free { get; private set; } = true;

        public void Dispose()
        {
            if (Free)
                return;
            Pool<T>.Dispose(this);
            Free = true;
        }
    }
}