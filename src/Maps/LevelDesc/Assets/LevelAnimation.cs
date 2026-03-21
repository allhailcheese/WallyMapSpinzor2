using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class LevelAnimation : IDeserializable<LevelAnimation>, ISerializable, IDrawable
{
    public string[] AnimationName { get; set; } = null!;
    public bool PlayMidground { get; set; }
    public bool Flip { get; set; }
    public string FileName { get; set; } = null!;
    public bool PlayForeground { get; set; }
    public bool PlayBackground { get; set; }
    public uint InitDelay { get; set; }
    public uint Interval { get; set; }
    public uint IntervalRand { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Rotation { get; set; } // radians
    public double Scale { get; set; }
    public double RandX { get; set; }
    public double RandY { get; set; }
    public uint LoopIterations { get; set; }
    public int TotalLoops { get; set; } // 0 means infinite
    public string? PlatID { get; set; }
    public bool IgnoreOnBlurBG { get; set; }

    public LevelAnimation() { }
    private LevelAnimation(XElement e)
    {
        AnimationName = e.GetAttribute("AnimationName").Split(',');
        PlayMidground = e.GetBoolAttribute("PlayMidground", false);
        Flip = e.GetBoolAttribute("Flip", false);
        FileName = e.GetAttribute("FileName");
        PlayForeground = e.GetBoolAttribute("PlayForeground", false);
        PlayBackground = e.GetBoolAttribute("PlayBackground", false);
        InitDelay = e.GetUIntAttribute("InitDelay", 0);
        Interval = e.GetUIntAttribute("Interval", 0);
        IntervalRand = e.GetUIntAttribute("IntervalRand", 0);
        PositionX = e.GetDoubleAttribute("PositionX");
        PositionY = e.GetDoubleAttribute("PositionY");
        Rotation = e.GetDoubleAttribute("Rotation", 0);
        Scale = e.GetDoubleAttribute("Scale", 0); // yes, this defaults to 0
        RandX = e.GetDoubleAttribute("RandX", 0);
        RandY = e.GetDoubleAttribute("RandY", 0);
        LoopIterations = e.GetUIntAttribute("LoopIterations", 0);
        TotalLoops = e.GetIntAttribute("TotalLoops", 0);
        PlatID = e.GetAttributeOrNull("PlatID");
        IgnoreOnBlurBG = e.GetBoolAttribute("IgnoreOnBlurBG", false);
    }
    public static LevelAnimation Deserialize(XElement e) => new(e);

    public void Serialize(XElement e)
    {
        e.SetAttributeValue("AnimationName", string.Join(',', AnimationName));
        if (PlayMidground)
            e.SetAttributeValue("PlayMidground", PlayMidground);
        if (Flip)
            e.SetAttributeValue("Flip", Flip);
        e.SetAttributeValue("FileName", FileName);
        if (PlayForeground)
            e.SetAttributeValue("PlayForeground", PlayForeground);
        if (PlayBackground)
            e.SetAttributeValue("PlayBackground", PlayBackground);
        if (InitDelay != 0)
            e.SetAttributeValue("InitDelay", InitDelay);
        if (Interval != 0)
            e.SetAttributeValue("Interval", Interval);
        if (IntervalRand != 0)
            e.SetAttributeValue("IntervalRand", IntervalRand);
        e.SetAttributeValue("PositionX", PositionX);
        e.SetAttributeValue("PositionY", PositionY);
        if (Rotation != 0)
            e.SetAttributeValue("Rotation", Rotation);
        if (Scale != 0)
            e.SetAttributeValue("Scale", Scale);
        if (RandX != 0)
            e.SetAttributeValue("RandX", RandX);
        if (RandY != 0)
            e.SetAttributeValue("RandY", RandY);
        if (LoopIterations != 0)
            e.SetAttributeValue("LoopIterations", LoopIterations);
        if (TotalLoops != 0)
            e.SetAttributeValue("TotalLoops", TotalLoops);
        if (PlatID is not null)
            e.SetAttributeValue("PlatID", PlatID);
        if (IgnoreOnBlurBG)
            e.SetAttributeValue("IgnoreOnBlurBG", IgnoreOnBlurBG);
    }

    private static TimeSpan FromTimestamp(uint timestamp) => TimeSpan.FromSeconds(timestamp / 60.0 / 16.0);
    private TimeSpan RollInterval(BrawlhallaRandom rand) => FromTimestamp((uint)Math.Floor(rand.NextF() * (IntervalRand + 1)));

    /*
    This is not a fully accurate simulation of the game, because the game is a piece of shit.

    The game starts with a delay of InitDelay, and then does delays of Interval.
    Except that it adds a random value between 0 and IntervalRand to each.

    The main difficulty is shit breaks because you can roll back time.
    Brawlhalla fixes this for replays, but in a way that requires knowing the length of the replay and doesn't work with negative time.
    Generally, the implementation ingame is pretty weird and is hard to adapt.
    Mostly because of how brawlhalla keeps track of time.

    I opted to just use simple TimeSpans instead.

    The animation ingame DOES break when you rewind time in training room, so you could argue that a bugged rewind is being accurate to the game.
    But whatever. This code is cleaner anyways.
    */
    public void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state_)
    {
        if (IgnoreOnBlurBG && !config.AnimatedBackgrounds)
            return;

        BrawlhallaRandom random = state_.Random;
        State state = state_[this];
        if (!state.Initialized)
        {
            uint initialOffset = InitDelay != 0 ? InitDelay : Interval;
            state.NextAnimationStartTime = FromTimestamp(initialOffset) + RollInterval(random);
            // if the user starts rolling back now, we need to pretend an animation used to play.
            // otherwise the "went before animation" logic won't trigger.
            state.AnimationStartTime = state.NextAnimationStartTime - FromTimestamp(Interval);

            state.Initialized = true;
        }

        bool forcePlayNew = false;
        /*
        moving to before a playing animation.
        move back in intervals until reaching a theoretical animation start.
        then force-select a new animation.
        needs a while loop for cases of overshooting.
        */
        while (config.Time < state.AnimationStartTime)
        {
            state.NextAnimationStartTime = state.AnimationStartTime;
            state.AnimationStartTime -= FromTimestamp(Interval) + RollInterval(random);
            forcePlayNew = true;
            state.Gfx = null;
        }

        // needs a while loop because high speeds can make the global time overshoot
        while (forcePlayNew || config.Time >= state.NextAnimationStartTime)
        {
            double positionX = PositionX + 2 * random.NextF() * RandX - RandX;
            double positionY = PositionY + 2 * random.NextF() * RandY - RandY;
            int animIndex = (int)Math.Floor(AnimationName.Length * random.NextF());
            string anim = AnimationName[animIndex];

            state.Gfx = new()
            {
                AnimFile = FileName,
                AnimClass = anim,
                BaseAnim = "Ready",
                FireAndForget = true,
                AnimScale = Scale,
            };

            state.Layer = PlayForeground
                ? DrawPriorityEnum.FOREGROUND
                : PlayMidground
                    ? DrawPriorityEnum.MIDGROUND
                    : PlayBackground
                        ? DrawPriorityEnum.BACKGROUND
                        : DrawPriorityEnum.MIDGROUND; // if none apply, there's a secret 4th layer. TODO: figure out what it is.

            double rotation = Rotation % Math.Tau;
            if (rotation < -Math.PI) rotation += Math.Tau;
            if (rotation > Math.PI) rotation -= Math.Tau;

            state.Trans = Transform.CreateFrom(
                x: positionX, y: positionY,
                scaleX: Scale * (Flip ? -1 : 1), scaleY: Scale,
                rot: rotation
            );

            if (!forcePlayNew)
            {
                state.AnimationStartTime = state.NextAnimationStartTime;
                state.NextAnimationStartTime += FromTimestamp(Interval) + RollInterval(random);
            }
            forcePlayNew = false; // prevent an infinite loop
        }

        if (state.Gfx is not null)
        {
            MovingPlatform? movingPlatform = PlatID is not null
                // LevelAnimation is affected by the first moving platform with that PlatID
                ? context.MovingPlatformByPlatID.GetValueOrDefault(PlatID, []).FirstOrDefault()
                : null;

            Transform platTrans = movingPlatform is not null
                ? context.MovingPlatformTransform[movingPlatform]
                : Transform.IDENTITY;

            // LevelAnimation is not affected by keyframe Rotation
            platTrans = Transform.CreateTranslate(platTrans.TranslateX, platTrans.TranslateY);

            long frame = LevelDesc.GET_ANIM_FRAME(config.Time - state.AnimationStartTime);
            canvas.DrawAnim(state.Gfx, "Ready", frame, trans * state.Trans * platTrans, state.Layer, this, loopLimit: LoopIterations != 0 ? LoopIterations + 1 : 1);
        }

        state_[this] = state;
    }

    internal class State
    {
        public bool Initialized = false;

        public Gfx? Gfx;
        public Transform Trans;
        public DrawPriorityEnum Layer;

        public TimeSpan AnimationStartTime;
        public TimeSpan NextAnimationStartTime;
    }
}