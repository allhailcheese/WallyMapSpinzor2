using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class Animation : IDeserializable<Animation>, ISerializable
{
    //these two can get a default from LevelDesc
    public int? NumFrames { get; set; }
    public double? SlowMult { get; set; }
    public double? CenterX { get; set; }
    public double? CenterY { get; set; }
    public bool EaseIn { get; set; }
    public bool EaseOut { get; set; }
    public uint EasePower { get; set; }
    public uint StartFrame { get; set; }
    public AbstractKeyFrame[] KeyFrames { get; set; } = null!;

    public bool HasCenter => CenterX is not null || CenterY is not null;

    public Animation() { }
    private Animation(XElement e)
    {
        NumFrames = e.GetIntAttributeOrNull("NumFrames");
        SlowMult = e.GetDoubleAttributeOrNull("SlowMult");
        if (e.HasAttribute("CenterX") || e.HasAttribute("CenterY"))
        {
            CenterX = e.GetDoubleAttribute("CenterX", 0);
            CenterY = e.GetDoubleAttribute("CenterY", 0);
        }
        EaseIn = e.GetBoolAttribute("EaseIn", false);
        EaseOut = e.GetBoolAttribute("EaseOut", false);
        EasePower = e.GetUIntAttribute("EasePower", 2);
        StartFrame = e.GetUIntAttribute("StartFrame", 0);
        KeyFrames = e.DeserializeKeyFrameChildren();
    }
    public static Animation Deserialize(XElement e) => new(e);

    public void Serialize(XElement e)
    {
        if (NumFrames is not null)
            e.SetAttributeValue("NumFrames", NumFrames.ToString());
        if (SlowMult is not null)
            e.SetAttributeValue("SlowMult", SlowMult.ToString());
        if (HasCenter)
        {
            if (CenterX != 0 || CenterY == 0)
                e.SetAttributeValue("CenterX", (CenterX ?? 0).ToString());
            if (CenterY != 0 || CenterX == 0)
                e.SetAttributeValue("CenterY", (CenterY ?? 0).ToString());
        }
        if (EaseIn)
            e.SetAttributeValue("EaseIn", EaseIn.ToString().ToLowerInvariant());
        if (EaseOut)
            e.SetAttributeValue("EaseOut", EaseOut.ToString().ToLowerInvariant());
        if (EasePower != 2 && (EaseIn || EaseOut))
            e.SetAttributeValue("EasePower", EasePower.ToString());
        if (StartFrame != 0)
            e.SetAttributeValue("StartFrame", StartFrame.ToString());

        e.AddManySerialized(KeyFrames);
    }

    /*
    The game creates a list of every position throughout the animation.
    This is bad for performance, so we use a shortcut.

    First, go through the list (without creating it) just to count how many entries will be in the list.

    Then, use the game's logic to determine two indices we need from the list.
    Then, do a second pass and get specifically the indices we need.

    We also need the index 0 in the list for Dynamic, so we grab that too during the second pass.

    And then a second optimization is that if there are no relevant frames between two keyframes, we just skip it.
    This should be 100% accurate to the game.
    */

    public ((double, double, double), (double, double)) GetOffset(RenderContext context, TimeSpan time)
    {
        // calculate the actual number of frames
        int currentFrame = 1;
        List<KeyFrame> keyframes = GetImplicitKeyFrames();
        for (int i = 0; i < keyframes.Count; ++i)
        {
            int frame2 = i == keyframes.Count - 1
                ? (NumFrames ?? context.DefaultNumFrames ?? 0) + 1
                : keyframes[i + 1].FrameNum;
            if (currentFrame < frame2)
                currentFrame = frame2;
        }
        int positionsCount = currentFrame - 1;

        // find the two position indices
        double brawlhallaTime = time.TotalSeconds * 60.0 * 16.0;
        double frames = 1000 * (positionsCount / 60.0) * (SlowMult ?? context.DefaultSlowMult ?? 1);
        double clampedTime = BrawlhallaMath.SafeMod(brawlhallaTime * 0.05, frames);
        double positionIndex = StartFrame + (context.ExtraStartFrame ?? 0) + clampedTime / frames * positionsCount;
        uint wholeIndex = (uint)Math.Floor(positionIndex + 1e-7);
        uint nextIndex = (uint)((wholeIndex + 1) % positionsCount);
        uint index = (uint)(wholeIndex % positionsCount);
        double smallDiff = positionIndex - wholeIndex;

        // go through the keyframes again, and find the positions
        bool gotA = false, gotP1 = false, gotP2 = false;
        (double ax, double ay) = (double.NaN, double.NaN);
        (double p1x, double p1y, double p1rot) = (double.NaN, double.NaN, double.NaN);
        (double p2x, double p2y, double p2rot) = (double.NaN, double.NaN, double.NaN);
        uint frameForNextIndex = nextIndex + 1;
        uint frameForIndex = index + 1;
        currentFrame = 1;
        for (int i = 0; i < keyframes.Count; ++i)
        {
            if (gotA && gotP1 && gotP2) break;

            KeyFrame k = keyframes[i];
            KeyFrame k2; int nextFrame;
            if (i == keyframes.Count - 1)
            {
                k2 = keyframes[0];
                nextFrame = (NumFrames ?? context.DefaultNumFrames ?? 0) + 1;
            }
            else
            {
                k2 = keyframes[i + 1];
                nextFrame = k2.FrameNum;
            }

            if (currentFrame >= nextFrame)
                continue;

            (double x, double y, double rot) LerpForFrame(uint frame)
            {
                double w = (frame - k.FrameNum) / (double)(nextFrame - k.FrameNum);
                w = BrawlhallaMath.EaseWeight(w,
                    k.EaseIn ?? EaseIn,
                    k.EaseOut ?? EaseOut,
                    k.EasePower ?? EasePower
                );
                (double x, double y) = k.CenterX is not null || k.CenterY is not null || CenterX is not null || CenterY is not null
                    ? BrawlhallaMath.BrawlhallaLerpWithCenter(k.X, k.Y, k2.X, k2.Y, k.CenterX ?? CenterX ?? 0, k.CenterY ?? CenterY ?? 0, w)
                    : BrawlhallaMath.BrawlhallaLerp(k.X, k.Y, k2.X, k2.Y, w);
                double rot = k.Rotation * (1 - w) + k2.Rotation * w;
                return (x, y, rot);
            }

            if (!gotA)
            {
                (ax, ay, _) = LerpForFrame((uint)currentFrame);
                gotA = true;
            }

            if (!gotP1 && currentFrame <= frameForNextIndex && frameForNextIndex < nextFrame)
            {
                (p1x, p1y, p1rot) = LerpForFrame(frameForNextIndex);
                gotP1 = true;
            }

            if (!gotP2 && currentFrame <= frameForIndex && frameForIndex < nextFrame)
            {
                (p2x, p2y, p2rot) = LerpForFrame(frameForIndex);
                gotP2 = true;
            }

            currentFrame = nextFrame;
        }

        double x = p1x * smallDiff + p2x * (1 - smallDiff);
        double y = p1y * smallDiff + p2y * (1 - smallDiff);

        if (Math.Abs(p1rot - p2rot) >= 180)
        {
            if (p1rot == 180 || p1rot == -180)
            {
                p1rot *= -1;
            }
            else if (p2rot == 180 || p2rot == -180)
            {
                p2rot *= -1;
            }
        }
        double rot = p1rot * smallDiff + p2rot * (1 - smallDiff);

        return ((x, y, rot), (ax, ay));
    }

    public List<KeyFrame> GetImplicitKeyFrames()
    {
        List<KeyFrame> result = [];
        foreach (AbstractKeyFrame akf in KeyFrames)
            akf.GetImplicitKeyFrames(result, 0, 0);
        return result;
    }
}