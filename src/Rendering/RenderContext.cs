using System.Collections.Generic;

namespace WallyMapSpinzor2;

public sealed class RenderContext
{
    public string? AssetDir { get; set; }
    public uint? ExtraStartFrame { get; set; }
    public int? DefaultNumFrames { get; set; }
    public double? DefaultSlowMult { get; set; }

    public double? BackgroundRect_X { get; set; }
    public double? BackgroundRect_Y { get; set; }
    public double? BackgroundRect_W { get; set; }
    public double? BackgroundRect_H { get; set; }

    public Background? CurrentBackground { get; set; } = null;

    public Dictionary<uint, Animation> AnimationByPlatID { get; init; } = [];
    public Dictionary<string, List<MovingPlatform>> MovingPlatformByPlatID { get; init; } = [];
    public Dictionary<MovingPlatform, (double, double)> MovingPlatformDynamicOffset { get; init; } = [];
    public Dictionary<MovingPlatform, Transform> MovingPlatformTransform { get; init; } = [];
    /*
    DynamicCollision (and no other dynamics) have a "bug",
    where only the last DynamicCollision will actually get offset by the moving platform.
    This emulates that behavior.
    */
    public Dictionary<string, DynamicCollision> DynamicCollisionPlatIDOwner { get; init; } = [];

    public Dictionary<uint, (double, double)> NavIDDictionary { get; init; } = [];
}