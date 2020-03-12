using System;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Layer indices of Unity built-in layers.
    /// </summary>
    public static class BuiltinLayers
    {
        public static int Default => _default.Value;
        public static int TransparentFX => _transparentFX.Value;
        public static int IgnoreRaycast => _ignoreRaycast.Value;
        public static int Water => _water.Value;
        public static int UI => _UI.Value;

        private static readonly Lazy<int> _default = new Lazy<int>(() => LayerMask.NameToLayer("Default"));
        private static readonly Lazy<int> _transparentFX = new Lazy<int>(() => LayerMask.NameToLayer("TransparentFX"));
        private static readonly Lazy<int> _ignoreRaycast = new Lazy<int>(() => LayerMask.NameToLayer("Ignore Raycast"));
        private static readonly Lazy<int> _water = new Lazy<int>(() => LayerMask.NameToLayer("Water"));
        private static readonly Lazy<int> _UI = new Lazy<int>(() => LayerMask.NameToLayer("UI"));
    }

    /// <summary>
    ///     Mask values of Unity built-in layers.
    /// </summary>
    public static class BuiltinLayerMasks
    {
        public static int Default => 1 << BuiltinLayers.Default;
        public static int TransparentFX => 1 << BuiltinLayers.TransparentFX;
        public static int IgnoreRaycast => 1 << BuiltinLayers.IgnoreRaycast;
        public static int Water => 1 << BuiltinLayers.Water;
        public static int UI => 1 << BuiltinLayers.UI;
    }

    /// <summary>
    ///     Values of Unity built-in sorting layers.
    /// </summary>
    public static class BuiltinSortingLayers
    {
        public static int Default => _default.Value;

        private static readonly Lazy<int> _default = new Lazy<int>(() => SortingLayer.GetLayerValueFromName("Default"));
    }
}
