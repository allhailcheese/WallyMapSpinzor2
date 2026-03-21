using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class LevelDesc : IDeserializable<LevelDesc>, ISerializable, IDrawable
{
    public const double ANIMATION_FPS = 24;
    public static long GET_ANIM_FRAME(TimeSpan time) => (long)(ANIMATION_FPS * time.TotalSeconds);

    private static readonly (int, int)[] ZOMBIE_SPAWNS = [(230, 390), (180, 900), (-1160, 900), (-1990, 390)];

    public string AssetDir { get; set; } = null!;
    public string LevelName { get; set; } = null!;
    public int NumFrames { get; set; }
    public double SlowMult { get; set; }

    public CameraBounds CameraBounds { get; set; } = null!;
    public SpawnBotBounds SpawnBotBounds { get; set; } = null!;
    public Background[] Backgrounds { get; set; } = null!;
    public LevelSound[] LevelSounds { get; set; } = null!;
    public TeamScoreboard? TeamScoreboard { get; set; }
    public AbstractAsset[] Assets { get; set; } = null!;
    public LevelAnim[] LevelAnims { get; set; } = null!;
    public LevelAnimation[] LevelAnimations { get; set; } = null!;
    public AbstractVolume[] Volumes { get; set; } = null!;
    public AbstractCollision[] Collisions { get; set; } = null!;
    public DynamicCollision[] DynamicCollisions { get; set; } = null!;
    public Respawn[] Respawns { get; set; } = null!;
    public DynamicRespawn[] DynamicRespawns { get; set; } = null!;
    public AbstractItemSpawn[] ItemSpawns { get; set; } = null!;
    public DynamicItemSpawn[] DynamicItemSpawns { get; set; } = null!;
    public NavNode[] NavNodes { get; set; } = null!;
    public DynamicNavNode[] DynamicNavNodes { get; set; } = null!;
    public WaveData[] WaveDatas { get; set; } = null!;
    public AnimatedBackground[] AnimatedBackgrounds { get; set; } = null!;

    public LevelDesc() { }
    private LevelDesc(XElement e)
    {
        AssetDir = e.GetAttribute("AssetDir");
        LevelName = e.GetAttribute("LevelName");
        NumFrames = e.GetIntAttribute("NumFrames", 0);
        SlowMult = e.GetDoubleAttribute("SlowMult", 1);

        CameraBounds = e.DeserializeRequiredChildOfType<CameraBounds>();
        SpawnBotBounds = e.DeserializeRequiredChildOfType<SpawnBotBounds>();
        Backgrounds = e.DeserializeChildrenOfType<Background>();
        LevelSounds = e.DeserializeChildrenOfType<LevelSound>();
        TeamScoreboard = e.DeserializeChildOfType<TeamScoreboard>();
        Assets = e.DeserializeAssetChildren();
        LevelAnims = e.DeserializeChildrenOfType<LevelAnim>();
        LevelAnimations = e.DeserializeChildrenOfType<LevelAnimation>();
        Volumes = e.DeserializeVolumeChildren();
        Collisions = e.DeserializeCollisionChildren();
        DynamicCollisions = e.DeserializeChildrenOfType<DynamicCollision>();
        Respawns = e.DeserializeChildrenOfType<Respawn>();
        DynamicRespawns = e.DeserializeChildrenOfType<DynamicRespawn>();
        ItemSpawns = e.DeserializeItemSpawnChildren();
        DynamicItemSpawns = e.DeserializeChildrenOfType<DynamicItemSpawn>();
        WaveDatas = e.DeserializeChildrenOfType<WaveData>();
        AnimatedBackgrounds = e.DeserializeChildrenOfType<AnimatedBackground>();
        NavNodes = e.DeserializeChildrenOfType<NavNode>();
        DynamicNavNodes = e.DeserializeChildrenOfType<DynamicNavNode>();
    }
    public static LevelDesc Deserialize(XElement e) => new(e);

    public void Serialize(XElement e)
    {
        e.SetAttributeValue("AssetDir", AssetDir);
        e.SetAttributeValue("LevelName", LevelName);
        if (NumFrames != 0)
            e.SetAttributeValue("NumFrames", NumFrames);
        if (SlowMult != 1)
            e.SetAttributeValue("SlowMult", SlowMult);

        e.AddSerialized(CameraBounds);
        e.AddSerialized(SpawnBotBounds);
        e.AddManySerialized(Backgrounds);
        e.AddManySerialized(LevelSounds);
        e.AddSerializedIfNotNull(TeamScoreboard);
        e.AddManySerialized(Assets);
        e.AddManySerialized(LevelAnims);
        e.AddManySerialized(LevelAnimations);
        e.AddManySerialized(Volumes);
        e.AddManySerialized(Collisions);
        e.AddManySerialized(DynamicCollisions);
        e.AddManySerialized(Respawns);
        e.AddManySerialized(DynamicRespawns);
        e.AddManySerialized(ItemSpawns);
        e.AddManySerialized(DynamicItemSpawns);
        e.AddManySerialized(WaveDatas);
        e.AddManySerialized(AnimatedBackgrounds);
        e.AddManySerialized(NavNodes);
        e.AddManySerialized(DynamicNavNodes);
    }

    public void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        if (trans != Transform.IDENTITY)
            throw new ArgumentException("Initial transform must be the identity transformation. Do not pass the camera transformation inside. Instead, handle it on the rendering side.");
        // setup
        context.AssetDir = AssetDir;
        context.DefaultNumFrames = NumFrames;
        context.DefaultSlowMult = SlowMult;
        foreach (Background b in Backgrounds)
            b.UpdateBackground(context, config);
        context.AnimationByPlatID.Clear();
        context.MovingPlatformByPlatID.Clear();
        context.MovingPlatformDynamicOffset.Clear();
        context.MovingPlatformTransform.Clear();
        foreach (MovingPlatform mp in Assets.OfType<MovingPlatform>())
            mp.StoreMovingPlatformOffset(context, config.Time);
        context.DynamicCollisionPlatIDOwner.Clear();
        foreach (DynamicCollision dc in DynamicCollisions)
            dc.ClaimPlatIDOwnership(context);
        context.NavIDDictionary.Clear();
        foreach (NavNode n in NavNodes)
            n.RegisterNavNode(context);
        foreach (DynamicNavNode dn in DynamicNavNodes)
            dn.RegisterNavNodes(context);

        // drawing
        CameraBounds.DrawOn(canvas, trans, config, context, state);
        SpawnBotBounds.DrawOn(canvas, trans, config, context, state);
        foreach (Background b in Backgrounds)
            b.DrawOn(canvas, trans, config, context, state);
        /*
        foreach(LevelSound ls in LevelSounds)
            ls.DrawOn(canvas, trans, config, context, state);
        */
        TeamScoreboard?.DrawOn(canvas, trans, config, context, state);
        foreach (AbstractAsset a in Assets)
            a.DrawOn(canvas, trans, config, context, state);
        foreach (LevelAnim la in LevelAnims)
            la.DrawOn(canvas, trans, config, context, state);
        foreach (AbstractVolume v in Volumes)
            v.DrawOn(canvas, trans, config, context, state);
        foreach (AbstractCollision c in Collisions)
            c.DrawOn(canvas, trans, config, context, state);
        foreach (DynamicCollision dc in DynamicCollisions)
            dc.DrawOn(canvas, trans, config, context, state);
        foreach (Respawn r in Respawns)
            r.DrawOn(canvas, trans, config, context, state);
        foreach (DynamicRespawn dr in DynamicRespawns)
            dr.DrawOn(canvas, trans, config, context, state);
        foreach (AbstractItemSpawn i in ItemSpawns)
            i.DrawOn(canvas, trans, config, context, state);
        foreach (DynamicItemSpawn di in DynamicItemSpawns)
            di.DrawOn(canvas, trans, config, context, state);
        foreach (WaveData wd in WaveDatas)
            wd.DrawOn(canvas, trans, config, context, state);
        foreach (AnimatedBackground ab in AnimatedBackgrounds)
            ab.DrawOn(canvas, trans, config, context, state);
        foreach (LevelAnimation la in LevelAnimations)
            la.DrawOn(canvas, trans, config, context, state);
        foreach (NavNode n in NavNodes)
            n.DrawOn(canvas, trans, config, context, state);
        foreach (DynamicNavNode dn in DynamicNavNodes)
            dn.DrawOn(canvas, trans, config, context, state);

        //Gamemode stuff

        if (config.ShowRingRopes)
        {
            Gfx gfx = new()
            {
                AnimFile = "Animation_GameModes.swf",
                AnimClass = "a__AnimationRingRope",
                AnimScale = 2,
            };
            canvas.DrawAnim(gfx, "Ready", 0, trans * Transform.CreateTranslate(521, 1293), DrawPriorityEnum.FOREGROUND, null);
            canvas.DrawAnim(gfx, "Ready", 0, trans * Transform.CreateTranslate(2934, 1293) * Transform.CreateScale(-1, 1), DrawPriorityEnum.FOREGROUND, null);
        }

        if (config.ShowZombieSpawns)
        {
            foreach ((int x, int y) in ZOMBIE_SPAWNS)
                canvas.DrawCircle(x, y, config.RadiusZombieSpawn, config.ColorZombieSpawns, trans, DrawPriorityEnum.DATA, null);
        }

        if (config.ShowBombsketballTargets)
        {
            Goal? goalblue = Volumes.OfType<Goal>().FirstOrDefault(g => g.Team == 1);
            if (goalblue is not null)
            {
                Gfx gfx = new()
                {
                    AnimFile = "Animation_GameModes.swf",
                    AnimClass = "a__AnimationTargetAnchoredBlue",
                    BaseAnim = "Ready",
                    AnimScale = 1.7,
                };
                canvas.DrawAnim(gfx, "Ready", 0, trans * Transform.CreateTranslate(goalblue.X + 85, goalblue.Y + 85), DrawPriorityEnum.FOREGROUND, null);
            }
            Goal? goalred = Volumes.OfType<Goal>().FirstOrDefault(g => g.Team == 2);
            if (goalred is not null)
            {
                Gfx gfx = new()
                {
                    AnimFile = "Animation_GameModes.swf",
                    AnimClass = "a__AnimationTargetAnchoredRed",
                    BaseAnim = "Ready",
                    AnimScale = 1.7,
                };
                canvas.DrawAnim(gfx, "Ready", 0, trans * Transform.CreateTranslate(goalred.X + 85, goalred.Y + 85), DrawPriorityEnum.FOREGROUND, null);
            }
        }

        if (Array.Exists(config.ShowBombsketballBombTimers, x => x))
        {
            (double, double)[] timerLocations = [.. ItemSpawns
                .OfType<ItemSpawn>()
                .Select(i => (i.X + i.W / 2, i.Y + i.H / 2))
                .OrderBy(_ => _,BombsketballTimerPositionComparer.Singleton)
            ];

            if (timerLocations.Length >= 3)
            {
                timerLocations = timerLocations[..3];
                Gfx gfx = new()
                {
                    AnimFile = "Animation_GameModes.swf",
                    AnimClass = "a__AnimationPieTimer",
                    AnimScale = 1,
                };

                double[] yOff = [-100, -200, -100];
                double[] frames = [7500 / 16.0, 3000 / 10.0, 7500 / 16.0];
                for (int i = 0; i < 3; ++i)
                {
                    if (!config.ShowBombsketballBombTimers[i])
                        continue;
                    uint? frameCount_ = canvas.GetAnimationFrameCount(gfx, "Ready");
                    if (frameCount_ is not null)
                    {
                        uint frameCount = frameCount_.Value;
                        int frame = (int)Math.Floor(config.BombsketballBombTimerFrames[i] / frames[i] * frameCount);
                        canvas.DrawAnim(gfx, "Ready", frame,
                        trans * Transform.CreateTranslate(timerLocations[i].Item1, timerLocations[i].Item2 + yOff[i]),
                        DrawPriorityEnum.FOREGROUND, null
                        );
                    }
                }
            }
        }

        if (config.ShowHordeDoors)
        {
            Gfx doorGfx = new()
            {
                AnimFile = "Animation_GameModes.swf",
                AnimClass = "a__AnimationValhallaDoor",
                BaseAnim = "Ready",
                AnimScale = 1,
                FireAndForget = true,
            };

            Gfx sparkleGfx = new()
            {
                AnimFile = "SFX_GameModes.swf",
                AnimClass = "a_ValhallaDoor_Loop",
                AnimScale = 1.7,
            };

            int i = 0;
            foreach (Goal g in Volumes.OfType<Goal>())
            {
                int hits = (i >= config.DamageHordeDoors.Length) ? 0 : config.DamageHordeDoors[i];

                string animationName = hits switch
                {
                    <= 6 => "Ready",
                    <= 12 => "QuarterDamage",
                    <= 23 => "HalfDamage",
                    _ => "FullDamage",
                };

                canvas.DrawAnim(doorGfx, animationName, GET_ANIM_FRAME(config.Time), trans * Transform.CreateTranslate(g.X + g.W / 2.0, g.Y + g.H), DrawPriorityEnum.FOREGROUND, null);
                if (hits != 0 && hits < 24)
                    canvas.DrawAnim(sparkleGfx, "", GET_ANIM_FRAME(config.Time), trans * Transform.CreateTranslate(g.X + g.W / 2.0, g.Y + g.H), DrawPriorityEnum.FOREGROUND, null);

                ++i;
            }
        }

        if (config.HordePathType != RenderConfig.PathConfigEnum.NONE &&
            config.HordePathType != RenderConfig.PathConfigEnum.CUSTOM)
        {
            Goal[] goals = [.. Volumes.OfType<Goal>()];

            if (goals.Length >= 2)
            {
                double door1CX = goals[0].X + goals[0].W / 2;
                double door1CY = goals[0].Y + goals[0].H / 2;
                double door2CX = goals[1].X + goals[1].W / 2;
                double door2CY = goals[1].Y + goals[1].H / 2;
                GenerateRandomHordePaths(config.HordeRandomSeed, CameraBounds.X, CameraBounds.Y, CameraBounds.W, CameraBounds.H, door1CX, door1CY, door2CX, door2CY);

                (double, double)[][] pathList = config.HordePathType switch
                {
                    RenderConfig.PathConfigEnum.LEFT => _leftPaths,
                    RenderConfig.PathConfigEnum.RIGHT => _rightPaths,
                    RenderConfig.PathConfigEnum.TOP => _topPaths,
                    RenderConfig.PathConfigEnum.BOTTOM => _bottomPaths,
                    _ => throw new ArgumentException(nameof(config.HordePathType))
                };
                int pathIndex = BrawlhallaMath.SafeMod(config.HordePathIndex, 20);
                (double, double)[] path = pathList[pathIndex];
                foreach ((double x, double y) in path)
                {
                    canvas.DrawCircle(x, y, config.RadiusHordePathPoint, config.ColorHordePath, trans, DrawPriorityEnum.DATA, null);
                }

                for (int i = 0; i < path.Length - 1; ++i)
                {
                    canvas.DrawArrow(
                        path[i].Item1, path[i].Item2,
                        path[i + 1].Item1, path[i + 1].Item2,
                        config.OffsetHordePathArrowSide, config.OffsetHordePathArrowBack,
                        config.ColorHordePath, trans, DrawPriorityEnum.DATA,
                        null
                    );
                }
            }
        }
    }

    private readonly (double, double)[][] _topPaths = new (double, double)[20][];
    private readonly (double, double)[][] _leftPaths = new (double, double)[20][];
    private readonly (double, double)[][] _rightPaths = new (double, double)[20][];
    private readonly (double, double)[][] _bottomPaths = new (double, double)[20][];

    private void GenerateRandomHordePaths(
        uint hordeSeed, // seed
        double boundX, double boundY, double boundW, double boundH, // camera bounds
        double door1CX, double door1CY, // door 1
        double door2CX, double door2CY // door 2
    )
    {
        BrawlhallaRandom rand = new(hordeSeed);
        for (int i = 0; i < 10; ++i) _topPaths[i] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.TOP, PathEnum.CLOSE, i)];
        for (int i = 0; i < 10; ++i) _topPaths[i + 10] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.TOP, PathEnum.FAR, i)];
        for (int i = 0; i < 10; ++i) _leftPaths[i] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.LEFT, PathEnum.CLOSE, i)];
        for (int i = 0; i < 10; ++i) _leftPaths[i + 10] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.LEFT, PathEnum.FAR, i)];
        for (int i = 0; i < 10; ++i) _rightPaths[i] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.RIGHT, PathEnum.CLOSE, i)];
        for (int i = 0; i < 10; ++i) _rightPaths[i + 10] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.RIGHT, PathEnum.FAR, i)];
        for (int i = 0; i < 20; ++i) _bottomPaths[i] = [.. BrawlhallaMath.GenerateHordePath(rand, boundX, boundY, boundW, boundH, door1CX, door1CY, door2CX, door2CY, DirEnum.BOTTOM, PathEnum.ANY, i)];
    }

    private sealed class BombsketballTimerPositionComparer : IComparer<(double, double)>
    {
        public static BombsketballTimerPositionComparer Singleton { get; } = new();

        public int Compare((double, double) a, (double, double) b)
        {
            int result = (int)(a.Item1 - b.Item1);
            if (result == 0) result = (int)(a.Item2 - b.Item2);
            return result;
        }
    }
}