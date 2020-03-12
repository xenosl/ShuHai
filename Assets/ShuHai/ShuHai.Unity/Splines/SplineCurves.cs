using System;

namespace ShuHai.Unity
{
    [Serializable]
    public sealed class HermiteCurve : SplineCurve<HermiteSpline> { }

    [Serializable]
    public sealed class CatmullRomCurve : SplineCurve<CatmullRomSpline> { }
}