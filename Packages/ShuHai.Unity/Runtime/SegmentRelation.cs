namespace ShuHai.Unity
{
    public enum SegmentRelation
    {
        CollinearOverlap,
        CollinearDisjoint,
        Parallel, // parallel and non-intersect
        Intersect, // intersect at a point and not collinear
        Disjoint // disjoint and not parallel
    }
}