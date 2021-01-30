using System;

namespace ShuHai.Unity
{
    public enum Axis { X, Y, Z };

    public enum AxisPlane { XY, YZ, ZX };

    public static class AxisExtensions
    {
        public static Axis GetNormalAxis(this AxisPlane plane)
        {
            switch (plane)
            {
                case AxisPlane.XY: return Axis.Z;
                case AxisPlane.YZ: return Axis.X;
                case AxisPlane.ZX: return Axis.Y;
                default: throw new ArgumentOutOfRangeException(nameof(plane), plane, null);
            }
        }

        public static AxisPlane GetOrthogonalPlane(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return AxisPlane.YZ;
                case Axis.Y: return AxisPlane.ZX;
                case Axis.Z: return AxisPlane.XY;
                default: throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }
    }
}
