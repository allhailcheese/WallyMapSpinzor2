using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public abstract class AbstractPressurePlateCollision : AbstractCollision
{
    public double AnimOffsetX { get; set; }
    public double AnimOffsetY { get; set; }
    public double AnimRotation { get; set; }
    public string AssetName { get; set; } = null!;
    public int Cooldown { get; set; }
    public bool FaceLeft { get; set; }
    public double[] FireOffsetX { get; set; } = null!;
    public double[] FireOffsetY { get; set; } = null!;
    public string? PlatID { get; set; }
    public string[] TrapPowers { get; set; } = null!;

    public AbstractPressurePlateCollision() { }
    protected AbstractPressurePlateCollision(XElement e) : base(e)
    {
        AnimOffsetX = e.GetDoubleAttribute("AnimOffsetX", 0);
        AnimOffsetY = e.GetDoubleAttribute("AnimOffsetY", 0);
        AnimRotation = e.GetDoubleAttribute("AnimRotation", 0);
        AssetName = e.GetAttribute("AssetName");
        Cooldown = e.GetIntAttribute("Cooldown", 3000);
        FaceLeft = e.GetBoolAttribute("FaceLeft", false);
        FireOffsetX = e.HasAttribute("FireOffsetX") ? [.. e.GetAttribute("FireOffsetX").Split(',').Select(double.Parse)] : [];
        FireOffsetY = e.HasAttribute("FireOffsetY") ? [.. e.GetAttribute("FireOffsetY").Split(',').Select(double.Parse)] : [-10];
        PlatID = e.GetAttributeOrNull("PlatID");
        TrapPowers = e.GetAttribute("TrapPowers").Split(',');
    }

    public sealed override void Serialize(XElement e)
    {
        e.SetAttributeValue("AnimOffsetX", AnimOffsetX);
        e.SetAttributeValue("AnimOffsetY", AnimOffsetY);
        if (AnimRotation != 0)
            e.SetAttributeValue("AnimRotation", AnimRotation);
        e.SetAttributeValue("AssetName", AssetName);
        e.SetAttributeValue("Cooldown", Cooldown);
        e.SetAttributeValue("FaceLeft", FaceLeft.ToString().ToLowerInvariant());
        if (FireOffsetX.Length != 0)
            e.SetAttributeValue("FireOffsetX", string.Join(',', FireOffsetX));
        if (FireOffsetY.Length != 0)
            e.SetAttributeValue("FireOffsetY", string.Join(',', FireOffsetY));
        if (PlatID is not null)
            e.SetAttributeValue("PlatID", PlatID);
        e.SetAttributeValue("TrapPowers", string.Join(',', TrapPowers));
        base.Serialize(e);
    }

    public sealed override void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        base.DrawOn(canvas, trans, config, context, state);

        // if a pressure plate has an Anchor, its
        // pressureplate-ness is ignored by the game.
        if (AnchorX is not null || AnchorY is not null)
            return;

        for (int i = 0; i < TrapPowers.Length; ++i)
        {
            double fireOffsetX = GetOffset(FireOffsetX, i);
            // it's impossible ingame to create a pressure plate with an empty FireOffsetY array
            double fireOffsetY = GetOffset(FireOffsetY.Length == 0 ? [-10] : FireOffsetY, i);
            if (config.ShowFireOffsetLocation)
                canvas.DrawCircle(fireOffsetX, fireOffsetY, config.RadiusFireOffsetLocation, config.ColorFireOffset, trans, DrawPriorityEnum.FIRE_OFFSET, this);
            if (config.ShowFireOffsetLine)
                canvas.DrawLine((X1 + X2) / 2, (Y1 + Y2) / 2, fireOffsetX, fireOffsetY, config.ColorFireOffsetLine, trans, DrawPriorityEnum.FIRE_OFFSET, this);
            if (config.ShowFireDirection)
            {
                double arrowEndX = fireOffsetX + (FaceLeft ? -1 : 1) * config.LengthFireDirectionArrow;
                double arrowEndY = fireOffsetY;
                canvas.DrawArrow(fireOffsetX, fireOffsetY, arrowEndX, arrowEndY, config.OffsetFireDirectionArrowSide, config.OffsetFireDirectionArrowBack, config.ColorFireDirection, trans, DrawPriorityEnum.FIRE_OFFSET, this);
            }
        }

        if (config.ShowAssets)
        {
            MovingPlatform? movingPlatform = PlatID is not null
                // pressure plate is affected by the last moving platform with that PlatID
                ? context.MovingPlatformByPlatID.GetValueOrDefault(PlatID, []).LastOrDefault()
                : null;

            Transform platTrans = movingPlatform is not null
                ? context.MovingPlatformTransform[movingPlatform]
                : Transform.IDENTITY;

            Transform spriteTrans = Transform.CreateFrom(x: AnimOffsetX, y: AnimOffsetY, rot: AnimRotation * Math.PI / 180);
            canvas.DrawAnim(Gfx, "Ready", 0, spriteTrans * platTrans, DrawPriorityEnum.MIDGROUND, this);
        }
    }

    public Gfx Gfx => new()
    {
        AnimFile = "Animation_GameModes.swf",
        AnimClass = "a__AnimationPressurePlate",
        AnimScale = 1,
        FireAndForget = true,
        CustomArts = AssetName.Length > 26 ? [new CustomArt() { FileName = "Animation_GameModes.swf", Name = AssetName[25..] }] : [],
    };

    // this is how it's done ingame
    private double GetOffset(double[] offset, int i)
    {
        if (offset.Length == 0)
            return 0;
        else if (offset.Length == TrapPowers.Length)
            return offset[i];
        else
            return offset[0];
    }
}