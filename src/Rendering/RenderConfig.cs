using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class RenderConfig : IDeserializable<RenderConfig>, ISerializable
{
    private const string INDEX = "Index";

    public enum PathConfigEnum
    {
        NONE,
        CUSTOM,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
    }

    public TimeSpan Time { get; set; }

    //Which ScoringType to use
    public required ScoringTypeEnum ScoringType { get; set; }
    //Which theme to use
    public required ThemeEnum Theme { get; set; }
    //What keys to render in Tutorial1
    public required HotkeyEnum Hotkey { get; set; }
    //Whether backgrounds are animated.
    public required bool AnimatedBackgrounds { get; set; }
    //Whether to act as if -noskulls is on
    public required bool NoSkulls { get; set; }

    //Disables PickedPlatform display
    public required bool ShowPickedPlatform { get; set; }
    //In platform king, which of the platforms is the red one
    public required int PickedPlatform { get; set; }

    //Scores of team Blue and team Red
    //used for TeamScoreboard
    public required int BlueScore { get; set; }
    public required int RedScore { get; set; }

    //whether to show the brawldown ropes
    public required bool ShowRingRopes { get; set; }
    //whether to show the zombie spawn points
    public required bool ShowZombieSpawns { get; set; }

    //whether to show the bombsketball tagets
    public required bool ShowBombsketballTargets { get; set; }
    //whether to use the bombsketball-specific size for TeamScoreboard digits
    public required bool UseBombsketballDigitSize { get; set; }
    //whether to show each bombsketball bomb timer
    public required bool[] ShowBombsketballBombTimers { get; set; }
    //the time in frames spent on each bombsketball bomb timer
    public required double[] BombsketballBombTimerFrames { get; set; }

    //whether to show the horde doors
    public required bool ShowHordeDoors { get; set; }
    //how many hits each door took. if array is too short, rest of doors will have 0
    public required int[] DamageHordeDoors { get; set; }
    //what horde path type (if any) to show
    public required PathConfigEnum HordePathType { get; set; }
    //the path index
    public required int HordePathIndex { get; set; }
    //the horde wave
    public required int HordeWave { get; set; }
    //the horde random seed
    public required uint HordeRandomSeed { get; set; }

    //whether to show the camera bounds
    public required bool ShowCameraBounds { get; set; }
    //wether to show the kill bounds
    public required bool ShowKillBounds { get; set; }
    //whether to show the sidekick bounds
    public required bool ShowSpawnBotBounds { get; set; }
    //whether to show assets
    public required bool ShowAssets { get; set; }
    //whether to show the background
    public required bool ShowBackground { get; set; }
    //whether to show collision
    public required bool ShowCollision { get; set; }
    //whether to show collision normal overrides
    public required bool ShowCollisionNormalOverride { get; set; }
    //whether to show fire offset of pressure plates
    public required bool ShowFireOffsetLocation { get; set; }
    public required bool ShowFireOffsetLine { get; set; }
    public required bool ShowFireDirection { get; set; }
    //whether to show goals
    public required bool ShowGoal { get; set; }
    //whether to show no dodge zones
    public required bool ShowNoDodgeZone { get; set; }
    //whether to show volumes
    public required bool ShowVolume { get; set; }
    //whether to show respawns
    public required bool ShowRespawn { get; set; }
    //whether to show item spawns
    public required bool ShowItemSpawn { get; set; }
    //whether to show navnodes
    public required bool ShowNavNode { get; set; }
    //whether to show bot panic line
    public required bool ShowBotPanicLine { get; set; }
    //whether to show bot ground line
    public required bool ShowBotGroundLine { get; set; }

    //circle radius to use for respawns
    public required double RadiusRespawn { get; set; }
    //circle radius to use for zombie spawns
    public required double RadiusZombieSpawn { get; set; }
    //circle radius to use for navnodes
    public required double RadiusNavNode { get; set; }
    //size of points in the horde path
    public required double RadiusHordePathPoint { get; set; }
    //size of radius to indicate pressure plate fire offset
    public required double RadiusFireOffsetLocation { get; set; }

    //length of collision normals
    public required double LengthCollisionNormal { get; set; }
    //when drawing pressure plate fire direction, length of the direction arrow
    public required double LengthFireDirectionArrow { get; set; }

    //when drawing navline arrows, sideways offset of arrow sides
    public required double OffsetNavLineArrowSide { get; set; }
    //when drawing navline arrows, backwards offset of arrow sides
    public required double OffsetNavLineArrowBack { get; set; }
    //when drawing horde path arrows, sideways offset of arrow sides
    public required double OffsetHordePathArrowSide { get; set; }
    //when drawing horde path arrows, backwards offset of arrow sides
    public required double OffsetHordePathArrowBack { get; set; }
    //when drawing pressure plate fire direction, sideways offset of arrow sides
    public required double OffsetFireDirectionArrowSide { get; set; }
    //when drawing pressure plate fire direction, backwards offset of arrow sides
    public required double OffsetFireDirectionArrowBack { get; set; }

    //colors
    public required Color ColorCameraBounds { get; set; }
    public required Color ColorKillBounds { get; set; }
    public required Color ColorSpawnBotBounds { get; set; }

    public required Color ColorHardCollision { get; set; }
    public required Color ColorSoftCollision { get; set; }
    public required Color ColorNoSlideCollision { get; set; }
    public required Color ColorGameModeHardCollision { get; set; }
    public required Color ColorBouncyHardCollision { get; set; }
    public required Color ColorBouncySoftCollision { get; set; }
    public required Color ColorBouncyNoSlideCollision { get; set; }
    public required Color ColorTriggerCollision { get; set; }
    public required Color ColorStickyCollision { get; set; }
    public required Color ColorItemIgnoreCollision { get; set; }
    public required Color ColorPressurePlateCollision { get; set; }
    public required Color ColorSoftPressurePlateCollision { get; set; }
    public required Color ColorLavaCollision { get; set; }

    public required Color ColorCollisionNormal { get; set; }
    public required Color ColorFireOffset { get; set; }
    public required Color ColorFireOffsetLine { get; set; }
    public required Color ColorFireDirection { get; set; }

    public required Color ColorRespawn { get; set; }
    public required Color ColorInitialRespawn { get; set; }
    public required Color ColorExpandedInitRespawn { get; set; }

    public required Color ColorItemSpawn { get; set; }
    public required Color ColorItemInitSpawn { get; set; }
    public required Color ColorItemSet { get; set; }
    public required Color ColorTeamItemInitSpawn { get; set; }

    //1 indexed
    public required Color[] ColorCollisionTeam { get; set; }
    //0 indexed. team 0 is rarely used.
    public required Color[] ColorVolumeTeam { get; set; }

    public required Color ColorZombieSpawns { get; set; }

    public required Color ColorNavNode { get; set; }
    public required Color ColorNavNodeW { get; set; }
    public required Color ColorNavNodeL { get; set; }
    public required Color ColorNavNodeA { get; set; }
    public required Color ColorNavNodeG { get; set; }
    public required Color ColorNavNodeT { get; set; }
    public required Color ColorNavNodeS { get; set; }

    public required Color ColorNavPath { get; set; }

    public required Color ColorBotPanicLine { get; set; }
    public required Color ColorBotGroundLine { get; set; }

    public required Color ColorHordePath { get; set; }

    public RenderConfig() { }

    [SetsRequiredMembers]
    public RenderConfig(RenderConfig other)
    {
        Time = other.Time;
        ScoringType = other.ScoringType;
        Theme = other.Theme;
        Hotkey = other.Hotkey;
        AnimatedBackgrounds = other.AnimatedBackgrounds;
        NoSkulls = other.NoSkulls;
        ShowPickedPlatform = other.ShowPickedPlatform;
        PickedPlatform = other.PickedPlatform;
        BlueScore = other.BlueScore;
        RedScore = other.RedScore;
        ShowRingRopes = other.ShowRingRopes;
        ShowZombieSpawns = other.ShowZombieSpawns;
        ShowBombsketballTargets = other.ShowBombsketballTargets;
        UseBombsketballDigitSize = other.UseBombsketballDigitSize;
        ShowBombsketballBombTimers = [.. other.ShowBombsketballBombTimers];
        BombsketballBombTimerFrames = [.. other.BombsketballBombTimerFrames];
        ShowHordeDoors = other.ShowHordeDoors;
        DamageHordeDoors = [.. other.DamageHordeDoors];
        HordePathType = other.HordePathType;
        HordePathIndex = other.HordePathIndex;
        HordeWave = other.HordeWave;
        HordeRandomSeed = other.HordeRandomSeed;
        ShowCameraBounds = other.ShowCameraBounds;
        ShowKillBounds = other.ShowKillBounds;
        ShowSpawnBotBounds = other.ShowSpawnBotBounds;
        ShowAssets = other.ShowAssets;
        ShowBackground = other.ShowBackground;
        ShowCollision = other.ShowCollision;
        ShowCollisionNormalOverride = other.ShowCollisionNormalOverride;
        ShowFireOffsetLocation = other.ShowFireOffsetLocation;
        ShowFireOffsetLine = other.ShowFireOffsetLine;
        ShowFireDirection = other.ShowFireDirection;
        ShowGoal = other.ShowGoal;
        ShowNoDodgeZone = other.ShowNoDodgeZone;
        ShowVolume = other.ShowVolume;
        ShowRespawn = other.ShowRespawn;
        ShowItemSpawn = other.ShowItemSpawn;
        ShowNavNode = other.ShowNavNode;
        ShowBotPanicLine = other.ShowBotPanicLine;
        ShowBotGroundLine = other.ShowBotGroundLine;
        RadiusRespawn = other.RadiusRespawn;
        RadiusZombieSpawn = other.RadiusZombieSpawn;
        RadiusNavNode = other.RadiusNavNode;
        RadiusHordePathPoint = other.RadiusHordePathPoint;
        RadiusFireOffsetLocation = other.RadiusFireOffsetLocation;
        LengthCollisionNormal = other.LengthCollisionNormal;
        LengthFireDirectionArrow = other.LengthFireDirectionArrow;
        OffsetNavLineArrowSide = other.OffsetNavLineArrowSide;
        OffsetNavLineArrowBack = other.OffsetNavLineArrowBack;
        OffsetHordePathArrowSide = other.OffsetHordePathArrowSide;
        OffsetHordePathArrowBack = other.OffsetHordePathArrowBack;
        OffsetFireDirectionArrowSide = other.OffsetFireDirectionArrowSide;
        OffsetFireDirectionArrowBack = other.OffsetFireDirectionArrowBack;
        ColorCameraBounds = other.ColorCameraBounds;
        ColorKillBounds = other.ColorKillBounds;
        ColorSpawnBotBounds = other.ColorSpawnBotBounds;
        ColorHardCollision = other.ColorHardCollision;
        ColorSoftCollision = other.ColorSoftCollision;
        ColorNoSlideCollision = other.ColorNoSlideCollision;
        ColorGameModeHardCollision = other.ColorGameModeHardCollision;
        ColorBouncyHardCollision = other.ColorBouncyHardCollision;
        ColorBouncySoftCollision = other.ColorBouncySoftCollision;
        ColorBouncyNoSlideCollision = other.ColorBouncyNoSlideCollision;
        ColorTriggerCollision = other.ColorTriggerCollision;
        ColorStickyCollision = other.ColorStickyCollision;
        ColorItemIgnoreCollision = other.ColorItemIgnoreCollision;
        ColorPressurePlateCollision = other.ColorPressurePlateCollision;
        ColorSoftPressurePlateCollision = other.ColorSoftPressurePlateCollision;
        ColorLavaCollision = other.ColorLavaCollision;
        ColorCollisionNormal = other.ColorCollisionNormal;
        ColorFireOffset = other.ColorFireOffset;
        ColorFireOffsetLine = other.ColorFireOffsetLine;
        ColorFireDirection = other.ColorFireDirection;
        ColorRespawn = other.ColorRespawn;
        ColorInitialRespawn = other.ColorInitialRespawn;
        ColorExpandedInitRespawn = other.ColorExpandedInitRespawn;
        ColorItemSpawn = other.ColorItemSpawn;
        ColorItemInitSpawn = other.ColorItemInitSpawn;
        ColorItemSet = other.ColorItemSet;
        ColorTeamItemInitSpawn = other.ColorTeamItemInitSpawn;
        ColorCollisionTeam = [.. other.ColorCollisionTeam];
        ColorVolumeTeam = [.. other.ColorVolumeTeam];
        ColorZombieSpawns = other.ColorZombieSpawns;
        ColorNavNode = other.ColorNavNode;
        ColorNavNodeW = other.ColorNavNodeW;
        ColorNavNodeL = other.ColorNavNodeL;
        ColorNavNodeA = other.ColorNavNodeA;
        ColorNavNodeG = other.ColorNavNodeG;
        ColorNavNodeT = other.ColorNavNodeT;
        ColorNavNodeS = other.ColorNavNodeS;
        ColorNavPath = other.ColorNavPath;
        ColorBotPanicLine = other.ColorBotPanicLine;
        ColorBotGroundLine = other.ColorBotGroundLine;
        ColorHordePath = other.ColorHordePath;
    }

    [SetsRequiredMembers]
    public RenderConfig(XElement e)
    {
        bool getBool(string name, bool @default) => e.GetBoolElement(name, @default);
        int getInt(string name, int @default) => e.GetIntElement(name, @default);
        uint getUInt(string name, uint @default) => e.GetUIntElement(name, @default);
        double getDouble(string name, double @default) => e.GetDoubleElement(name, @default);
        E getEnum<E>(string name, E @default) where E : struct, Enum => e.GetEnumElement<E>(name, @default);
        Color getColor(string name, Color @default) => e.GetColorElement(name, @default);

        T[] getArray<T>(string name, T[] @default, Func<string?, T?> f, int count, int offset = 0) where T : struct => Enumerable.Range(0, count).Select(i => f(e.Element(name)?.GetElementOrNull($"{INDEX}{i + offset}")) ?? @default[i]).ToArray();
        bool[] getBoolArray(string name, bool[] @default, int count, int offset = 0) => getArray(name, @default, Utils.ParseBoolOrNull, count, offset);
        int[] getIntArray(string name, int[] @default, int count, int offset = 0) => getArray(name, @default, Utils.ParseIntOrNull, count, offset);
        uint[] getUIntArray(string name, uint[] @default, int count, int offset = 0) => getArray(name, @default, Utils.ParseUIntOrNull, count, offset);
        double[] getDoubleArray(string name, double[] @default, int count, int offset = 0) => getArray(name, @default, Utils.ParseDoubleOrNull, count, offset);
        Color[] getColorArray(string name, Color[] @default, int count, int offset = 0) => [.. getUIntArray(name, [.. @default.Select(c => c.ToHex())], count, offset).Select(Color.FromHex)];

        RenderConfig @default = Default;
        ScoringType = getEnum(nameof(ScoringType), @default.ScoringType);
        Theme = getEnum(nameof(Theme), @default.Theme);
        Hotkey = getEnum(nameof(Hotkey), @default.Hotkey);
        AnimatedBackgrounds = getBool(nameof(AnimatedBackgrounds), @default.AnimatedBackgrounds);
        NoSkulls = getBool(nameof(NoSkulls), @default.NoSkulls);
        ShowPickedPlatform = getBool(nameof(ShowPickedPlatform), @default.ShowPickedPlatform);
        PickedPlatform = getInt(nameof(PickedPlatform), @default.PickedPlatform);
        BlueScore = getInt(nameof(BlueScore), @default.BlueScore);
        RedScore = getInt(nameof(RedScore), @default.RedScore);
        ShowRingRopes = getBool(nameof(ShowRingRopes), @default.ShowRingRopes);
        ShowZombieSpawns = getBool(nameof(ShowZombieSpawns), @default.ShowZombieSpawns);
        ShowBombsketballTargets = getBool(nameof(ShowBombsketballTargets), @default.ShowBombsketballTargets);
        UseBombsketballDigitSize = getBool(nameof(UseBombsketballDigitSize), @default.UseBombsketballDigitSize);
        ShowBombsketballBombTimers = getBoolArray(nameof(ShowBombsketballBombTimers), @default.ShowBombsketballBombTimers, 3);
        BombsketballBombTimerFrames = getDoubleArray(nameof(BombsketballBombTimerFrames), @default.BombsketballBombTimerFrames, 3);
        ShowHordeDoors = getBool(nameof(ShowHordeDoors), @default.ShowHordeDoors);
        DamageHordeDoors = getIntArray(nameof(DamageHordeDoors), @default.DamageHordeDoors, 2);
        HordePathType = getEnum(nameof(HordePathType), @default.HordePathType);
        HordePathIndex = getInt(nameof(HordePathIndex), @default.HordePathIndex);
        HordeWave = getInt(nameof(HordeWave), @default.HordeWave);
        HordeRandomSeed = getUInt(nameof(HordeRandomSeed), @default.HordeRandomSeed);
        ShowCameraBounds = getBool(nameof(ShowCameraBounds), @default.ShowCameraBounds);
        ShowKillBounds = getBool(nameof(ShowKillBounds), @default.ShowKillBounds);
        ShowSpawnBotBounds = getBool(nameof(ShowSpawnBotBounds), @default.ShowSpawnBotBounds);
        ShowAssets = getBool(nameof(ShowAssets), @default.ShowAssets);
        ShowBackground = getBool(nameof(ShowBackground), @default.ShowBackground);
        ShowCollision = getBool(nameof(ShowCollision), @default.ShowCollision);
        ShowCollisionNormalOverride = getBool(nameof(ShowCollisionNormalOverride), @default.ShowCollisionNormalOverride);
        ShowFireOffsetLocation = getBool(nameof(ShowFireOffsetLocation), @default.ShowFireOffsetLocation);
        ShowFireOffsetLine = getBool(nameof(ShowFireOffsetLine), @default.ShowFireOffsetLine);
        ShowFireDirection = getBool(nameof(ShowFireDirection), @default.ShowFireDirection);
        ShowGoal = getBool(nameof(ShowGoal), @default.ShowGoal);
        ShowNoDodgeZone = getBool(nameof(ShowNoDodgeZone), @default.ShowNoDodgeZone);
        ShowVolume = getBool(nameof(ShowVolume), @default.ShowVolume);
        ShowRespawn = getBool(nameof(ShowRespawn), @default.ShowRespawn);
        ShowItemSpawn = getBool(nameof(ShowItemSpawn), @default.ShowItemSpawn);
        ShowNavNode = getBool(nameof(ShowNavNode), @default.ShowNavNode);
        ShowBotPanicLine = getBool(nameof(ShowBotPanicLine), @default.ShowBotPanicLine);
        ShowBotGroundLine = getBool(nameof(ShowBotGroundLine), @default.ShowBotGroundLine);
        RadiusRespawn = getDouble(nameof(RadiusRespawn), @default.RadiusRespawn);
        RadiusZombieSpawn = getDouble(nameof(RadiusZombieSpawn), @default.RadiusZombieSpawn);
        RadiusNavNode = getDouble(nameof(RadiusNavNode), @default.RadiusNavNode);
        RadiusHordePathPoint = getDouble(nameof(RadiusHordePathPoint), @default.RadiusHordePathPoint);
        RadiusFireOffsetLocation = getDouble(nameof(RadiusFireOffsetLocation), @default.RadiusFireOffsetLocation);
        LengthCollisionNormal = getDouble(nameof(LengthCollisionNormal), @default.LengthCollisionNormal);
        LengthFireDirectionArrow = getDouble(nameof(LengthFireDirectionArrow), @default.LengthFireDirectionArrow);
        OffsetNavLineArrowSide = getDouble(nameof(OffsetNavLineArrowSide), @default.OffsetNavLineArrowSide);
        OffsetNavLineArrowBack = getDouble(nameof(OffsetNavLineArrowBack), @default.OffsetNavLineArrowBack);
        OffsetHordePathArrowSide = getDouble(nameof(OffsetHordePathArrowSide), @default.OffsetHordePathArrowSide);
        OffsetHordePathArrowBack = getDouble(nameof(OffsetHordePathArrowBack), @default.OffsetHordePathArrowBack);
        OffsetFireDirectionArrowSide = getDouble(nameof(OffsetFireDirectionArrowSide), @default.OffsetFireDirectionArrowSide);
        OffsetFireDirectionArrowBack = getDouble(nameof(OffsetFireDirectionArrowBack), @default.OffsetFireDirectionArrowBack);
        ColorCameraBounds = getColor(nameof(ColorCameraBounds), @default.ColorCameraBounds);
        ColorKillBounds = getColor(nameof(ColorKillBounds), @default.ColorKillBounds);
        ColorSpawnBotBounds = getColor(nameof(ColorSpawnBotBounds), @default.ColorSpawnBotBounds);
        ColorHardCollision = getColor(nameof(ColorHardCollision), @default.ColorHardCollision);
        ColorSoftCollision = getColor(nameof(ColorSoftCollision), @default.ColorSoftCollision);
        ColorNoSlideCollision = getColor(nameof(ColorNoSlideCollision), @default.ColorNoSlideCollision);
        ColorGameModeHardCollision = getColor(nameof(ColorGameModeHardCollision), @default.ColorGameModeHardCollision);
        ColorBouncyHardCollision = getColor(nameof(ColorBouncyHardCollision), @default.ColorBouncyHardCollision);
        ColorBouncySoftCollision = getColor(nameof(ColorBouncySoftCollision), @default.ColorBouncySoftCollision);
        ColorBouncyNoSlideCollision = getColor(nameof(ColorBouncyNoSlideCollision), @default.ColorBouncyNoSlideCollision);
        ColorTriggerCollision = getColor(nameof(ColorTriggerCollision), @default.ColorTriggerCollision);
        ColorStickyCollision = getColor(nameof(ColorStickyCollision), @default.ColorStickyCollision);
        ColorItemIgnoreCollision = getColor(nameof(ColorItemIgnoreCollision), @default.ColorItemIgnoreCollision);
        ColorPressurePlateCollision = getColor(nameof(ColorPressurePlateCollision), @default.ColorPressurePlateCollision);
        ColorSoftPressurePlateCollision = getColor(nameof(ColorSoftPressurePlateCollision), @default.ColorSoftPressurePlateCollision);
        ColorLavaCollision = getColor(nameof(ColorLavaCollision), @default.ColorLavaCollision);
        ColorCollisionNormal = getColor(nameof(ColorCollisionNormal), @default.ColorCollisionNormal);
        ColorFireOffset = getColor(nameof(ColorFireOffset), @default.ColorFireOffset);
        ColorFireOffsetLine = getColor(nameof(ColorFireOffsetLine), @default.ColorFireOffsetLine);
        ColorFireDirection = getColor(nameof(ColorFireDirection), @default.ColorFireDirection);
        ColorRespawn = getColor(nameof(ColorRespawn), @default.ColorRespawn);
        ColorInitialRespawn = getColor(nameof(ColorInitialRespawn), @default.ColorInitialRespawn);
        ColorExpandedInitRespawn = getColor(nameof(ColorExpandedInitRespawn), @default.ColorExpandedInitRespawn);
        ColorItemSpawn = getColor(nameof(ColorItemSpawn), @default.ColorItemSpawn);
        ColorItemInitSpawn = getColor(nameof(ColorItemInitSpawn), @default.ColorItemInitSpawn);
        ColorItemSet = getColor(nameof(ColorItemSet), @default.ColorItemSet);
        ColorTeamItemInitSpawn = getColor(nameof(ColorTeamItemInitSpawn), @default.ColorTeamItemInitSpawn);
        ColorCollisionTeam = getColorArray(nameof(ColorCollisionTeam), @default.ColorCollisionTeam, 5, 1);
        ColorVolumeTeam = getColorArray(nameof(ColorVolumeTeam), @default.ColorVolumeTeam, 6);
        ColorZombieSpawns = getColor(nameof(ColorZombieSpawns), @default.ColorZombieSpawns);
        ColorNavNode = getColor(nameof(ColorNavNode), @default.ColorNavNode);
        ColorNavNodeW = getColor(nameof(ColorNavNodeW), @default.ColorNavNodeW);
        ColorNavNodeL = getColor(nameof(ColorNavNodeL), @default.ColorNavNodeL);
        ColorNavNodeA = getColor(nameof(ColorNavNodeA), @default.ColorNavNodeA);
        ColorNavNodeG = getColor(nameof(ColorNavNodeG), @default.ColorNavNodeG);
        ColorNavNodeT = getColor(nameof(ColorNavNodeT), @default.ColorNavNodeT);
        ColorNavNodeS = getColor(nameof(ColorNavNodeS), @default.ColorNavNodeS);
        ColorNavPath = getColor(nameof(ColorNavPath), @default.ColorNavPath);
        ColorBotPanicLine = getColor(nameof(ColorBotPanicLine), @default.ColorBotPanicLine);
        ColorBotGroundLine = getColor(nameof(ColorBotGroundLine), @default.ColorBotGroundLine);
        ColorHordePath = getColor(nameof(ColorHordePath), @default.ColorHordePath);
    }
    public static RenderConfig Deserialize(XElement e) => new(e);

    public void Serialize(XElement e)
    {
        void addValue(string name, object? value) => e.AddChild(name, value);
        void addColor(string name, Color value) => addValue(name, "0x" + value.ToHex().ToString("X8"));
        void addArray<T>(string name, T[] values, int count, int offset = 0)
        {
            XElement element = new(name);
            for (int i = 0; i < count; ++i) element.AddChild($"{INDEX}{i + offset}", values[i]);
            e.Add(element);
        }
        void addColorArray(string name, Color[] values, int count, int offset = 0) => addArray(name, [.. values.Select(c => "0x" + c.ToHex().ToString("X8"))], count, offset);

        addValue(nameof(ScoringType), ScoringType);
        addValue(nameof(Theme), Theme);
        addValue(nameof(Hotkey), Hotkey);
        addValue(nameof(AnimatedBackgrounds), AnimatedBackgrounds);
        addValue(nameof(NoSkulls), NoSkulls);
        addValue(nameof(ShowPickedPlatform), ShowPickedPlatform);
        addValue(nameof(PickedPlatform), PickedPlatform);
        addValue(nameof(BlueScore), BlueScore);
        addValue(nameof(RedScore), RedScore);
        addValue(nameof(ShowRingRopes), ShowRingRopes);
        addValue(nameof(ShowZombieSpawns), ShowZombieSpawns);
        addValue(nameof(ShowBombsketballTargets), ShowBombsketballTargets);
        addValue(nameof(UseBombsketballDigitSize), UseBombsketballDigitSize);
        addArray(nameof(ShowBombsketballBombTimers), ShowBombsketballBombTimers, 3);
        addArray(nameof(BombsketballBombTimerFrames), BombsketballBombTimerFrames, 3);
        addValue(nameof(ShowHordeDoors), ShowHordeDoors);
        addArray(nameof(DamageHordeDoors), DamageHordeDoors, 2);
        addValue(nameof(HordePathType), HordePathType);
        addValue(nameof(HordePathIndex), HordePathIndex);
        addValue(nameof(HordeWave), HordeWave);
        addValue(nameof(HordeRandomSeed), HordeRandomSeed);
        addValue(nameof(ShowCameraBounds), ShowCameraBounds);
        addValue(nameof(ShowKillBounds), ShowKillBounds);
        addValue(nameof(ShowSpawnBotBounds), ShowSpawnBotBounds);
        addValue(nameof(ShowAssets), ShowAssets);
        addValue(nameof(ShowBackground), ShowBackground);
        addValue(nameof(ShowCollision), ShowCollision);
        addValue(nameof(ShowCollisionNormalOverride), ShowCollisionNormalOverride);
        addValue(nameof(ShowFireOffsetLocation), ShowFireOffsetLocation);
        addValue(nameof(ShowFireOffsetLine), ShowFireOffsetLine);
        addValue(nameof(ShowFireDirection), ShowFireDirection);
        addValue(nameof(ShowGoal), ShowGoal);
        addValue(nameof(ShowNoDodgeZone), ShowNoDodgeZone);
        addValue(nameof(ShowVolume), ShowVolume);
        addValue(nameof(ShowRespawn), ShowRespawn);
        addValue(nameof(ShowItemSpawn), ShowItemSpawn);
        addValue(nameof(ShowNavNode), ShowNavNode);
        addValue(nameof(ShowBotPanicLine), ShowBotPanicLine);
        addValue(nameof(ShowBotGroundLine), ShowBotGroundLine);
        addValue(nameof(RadiusRespawn), RadiusRespawn);
        addValue(nameof(RadiusZombieSpawn), RadiusZombieSpawn);
        addValue(nameof(RadiusNavNode), RadiusNavNode);
        addValue(nameof(RadiusHordePathPoint), RadiusHordePathPoint);
        addValue(nameof(RadiusFireOffsetLocation), RadiusFireOffsetLocation);
        addValue(nameof(LengthCollisionNormal), LengthCollisionNormal);
        addValue(nameof(LengthFireDirectionArrow), LengthFireDirectionArrow);
        addValue(nameof(OffsetNavLineArrowSide), OffsetNavLineArrowSide);
        addValue(nameof(OffsetNavLineArrowBack), OffsetNavLineArrowBack);
        addValue(nameof(OffsetHordePathArrowSide), OffsetHordePathArrowSide);
        addValue(nameof(OffsetHordePathArrowBack), OffsetHordePathArrowBack);
        addValue(nameof(OffsetFireDirectionArrowSide), OffsetFireDirectionArrowSide);
        addValue(nameof(OffsetFireDirectionArrowBack), OffsetFireDirectionArrowBack);
        addColor(nameof(ColorCameraBounds), ColorCameraBounds);
        addColor(nameof(ColorKillBounds), ColorKillBounds);
        addColor(nameof(ColorSpawnBotBounds), ColorSpawnBotBounds);
        addColor(nameof(ColorHardCollision), ColorHardCollision);
        addColor(nameof(ColorSoftCollision), ColorSoftCollision);
        addColor(nameof(ColorNoSlideCollision), ColorNoSlideCollision);
        addColor(nameof(ColorGameModeHardCollision), ColorGameModeHardCollision);
        addColor(nameof(ColorBouncyHardCollision), ColorBouncyHardCollision);
        addColor(nameof(ColorBouncySoftCollision), ColorBouncySoftCollision);
        addColor(nameof(ColorBouncyNoSlideCollision), ColorBouncyNoSlideCollision);
        addColor(nameof(ColorTriggerCollision), ColorTriggerCollision);
        addColor(nameof(ColorStickyCollision), ColorStickyCollision);
        addColor(nameof(ColorItemIgnoreCollision), ColorItemIgnoreCollision);
        addColor(nameof(ColorPressurePlateCollision), ColorPressurePlateCollision);
        addColor(nameof(ColorSoftPressurePlateCollision), ColorSoftPressurePlateCollision);
        addColor(nameof(ColorLavaCollision), ColorLavaCollision);
        addColor(nameof(ColorCollisionNormal), ColorCollisionNormal);
        addColor(nameof(ColorFireOffset), ColorFireOffset);
        addColor(nameof(ColorFireOffsetLine), ColorFireOffsetLine);
        addColor(nameof(ColorFireDirection), ColorFireDirection);
        addColor(nameof(ColorRespawn), ColorRespawn);
        addColor(nameof(ColorInitialRespawn), ColorInitialRespawn);
        addColor(nameof(ColorExpandedInitRespawn), ColorExpandedInitRespawn);
        addColor(nameof(ColorItemSpawn), ColorItemSpawn);
        addColor(nameof(ColorItemInitSpawn), ColorItemInitSpawn);
        addColor(nameof(ColorItemSet), ColorItemSet);
        addColor(nameof(ColorTeamItemInitSpawn), ColorTeamItemInitSpawn);
        addColorArray(nameof(ColorCollisionTeam), ColorCollisionTeam, 5, 1);
        addColorArray(nameof(ColorVolumeTeam), ColorVolumeTeam, 5);
        addColor(nameof(ColorZombieSpawns), ColorZombieSpawns);
        addColor(nameof(ColorNavNode), ColorNavNode);
        addColor(nameof(ColorNavNodeW), ColorNavNodeW);
        addColor(nameof(ColorNavNodeL), ColorNavNodeL);
        addColor(nameof(ColorNavNodeA), ColorNavNodeA);
        addColor(nameof(ColorNavNodeG), ColorNavNodeG);
        addColor(nameof(ColorNavNodeT), ColorNavNodeT);
        addColor(nameof(ColorNavNodeS), ColorNavNodeS);
        addColor(nameof(ColorNavPath), ColorNavPath);
        addColor(nameof(ColorBotPanicLine), ColorBotPanicLine);
        addColor(nameof(ColorBotGroundLine), ColorBotGroundLine);
        addColor(nameof(ColorHordePath), ColorHordePath);
    }

    public static RenderConfig Default => new()
    {
        ScoringType = ScoringTypeEnum.None,
        Theme = ThemeEnum.None,
        Hotkey = HotkeyEnum.Keyboard,
        AnimatedBackgrounds = false,
        NoSkulls = false,
        ShowPickedPlatform = false,
        PickedPlatform = 0,
        BlueScore = 0,
        RedScore = 0,
        ShowRingRopes = false,
        ShowZombieSpawns = false,
        ShowBombsketballTargets = false,
        UseBombsketballDigitSize = false,
        ShowBombsketballBombTimers = [false, false, false],
        BombsketballBombTimerFrames = [0, 0, 0],
        ShowHordeDoors = false,
        DamageHordeDoors = [0, 0],
        HordePathType = PathConfigEnum.NONE,
        HordePathIndex = 0,
        HordeWave = 0,
        HordeRandomSeed = 0,
        ShowCameraBounds = true,
        ShowKillBounds = true,
        ShowSpawnBotBounds = false,
        ShowAssets = true,
        ShowBackground = true,
        ShowCollision = true,
        ShowCollisionNormalOverride = true,
        ShowFireOffsetLocation = false,
        ShowFireOffsetLine = false,
        ShowFireDirection = false,
        ShowGoal = true,
        ShowNoDodgeZone = true,
        ShowVolume = true,
        ShowRespawn = true,
        ShowItemSpawn = true,
        ShowNavNode = false,
        ShowBotPanicLine = false,
        ShowBotGroundLine = false,
        RadiusRespawn = 30,
        RadiusZombieSpawn = 20,
        RadiusNavNode = 20,
        RadiusHordePathPoint = 15,
        RadiusFireOffsetLocation = 15,
        LengthCollisionNormal = 50,
        LengthFireDirectionArrow = 50,
        OffsetNavLineArrowSide = 7,
        OffsetNavLineArrowBack = 7,
        OffsetHordePathArrowSide = 18,
        OffsetHordePathArrowBack = 28,
        OffsetFireDirectionArrowSide = 18,
        OffsetFireDirectionArrowBack = 28,
        ColorCameraBounds = Color.FromHex(0xFF00007F),
        ColorKillBounds = Color.FromHex(0x5F00007F),
        ColorSpawnBotBounds = Color.FromHex(0xFFFFCC7F),
        ColorHardCollision = Color.FromHex(0x00FF00FF),
        ColorSoftCollision = Color.FromHex(0xFFFF00FF),
        ColorNoSlideCollision = Color.FromHex(0x00FFFFFF),
        ColorGameModeHardCollision = Color.FromHex(0xCCFFCCFF),
        ColorBouncyHardCollision = Color.FromHex(0x339933FF),
        ColorBouncySoftCollision = Color.FromHex(0x999933FF),
        ColorBouncyNoSlideCollision = Color.FromHex(0x339999FF),
        ColorTriggerCollision = Color.FromHex(0x0000FFFF),
        ColorStickyCollision = Color.FromHex(0x66FF66FF),
        ColorItemIgnoreCollision = Color.FromHex(0xFF00FFFF),
        ColorPressurePlateCollision = Color.FromHex(0xCC6619FF),
        ColorSoftPressurePlateCollision = Color.FromHex(0x7F1900FF),
        ColorLavaCollision = Color.FromHex(0xFF8000FF),
        ColorCollisionNormal = Color.FromHex(0xFFFFFF7F),
        ColorFireOffset = Color.FromHex(0x7000AA70),
        ColorFireOffsetLine = Color.FromHex(0xBB7000D0),
        ColorFireDirection = Color.FromHex(0xCC5000BE),
        ColorRespawn = Color.FromHex(0xFF7F0060),
        ColorInitialRespawn = Color.FromHex(0xFF000060),
        ColorExpandedInitRespawn = Color.FromHex(0xFF00FF60),
        ColorItemSpawn = Color.FromHex(0x007FFF60),
        ColorItemInitSpawn = Color.FromHex(0x7F007F60),
        ColorItemSet = Color.FromHex(0x007F7F60),
        ColorTeamItemInitSpawn = Color.FromHex(0x7F7F0060),
        ColorCollisionTeam =
        [
            Color.FromHex(0xFF00007F),
            Color.FromHex(0x0000FF7F),
            Color.FromHex(0x00FF007F),
            Color.FromHex(0xFFFF007F),
            Color.FromHex(0xFF00FF7F)
        ],
        ColorVolumeTeam =
        [
            Color.FromHex(0xFFA50033),
            Color.FromHex(0xFF000033),
            Color.FromHex(0x0000FF33),
            Color.FromHex(0x00FF0033),
            Color.FromHex(0xFFFF0033),
            Color.FromHex(0xFF00FF33)
        ],
        ColorZombieSpawns = Color.FromHex(0xFFFFFF7F),
        ColorNavNode = Color.FromHex(0x7F7F7F90),
        ColorNavNodeW = Color.FromHex(0x7F700F90),
        ColorNavNodeL = Color.FromHex(0x7F007F90),
        ColorNavNodeA = Color.FromHex(0x007F7F90),
        ColorNavNodeG = Color.FromHex(0x007F0090),
        ColorNavNodeT = Color.FromHex(0x00007F90),
        ColorNavNodeS = Color.FromHex(0x7F000090),
        ColorNavPath = Color.FromHex(0x8060607F),
        ColorBotPanicLine = Color.FromHex(0xFF5F5F8F),
        ColorBotGroundLine = Color.FromHex(0x8F8F8FAF),
        ColorHordePath = Color.FromHex(0x93529960),
    };
}