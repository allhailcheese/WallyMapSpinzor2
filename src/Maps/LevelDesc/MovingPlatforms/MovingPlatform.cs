using System;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

/*
yes, moving platforms can technically have the stuff you'd normally expect from a Platform
if they have an AssetName, then they'd act like a Platform, and won't do anything with Animation
but that would never be done ingame, so we just ignore AssetName when drawing.
and yes the game does technically support MovingPlatform inside a Platform or MovingPlatform.
don't question it.

now, the real reason for making MovingPlatform an AbstractAsset is that it's possible for the game
to put its MovingPlatforms AFTER Platforms, which would alter the drawing order.

thanks bmg.
*/
public sealed class MovingPlatform : AbstractAsset, IDeserializable<MovingPlatform>
{
    public string PlatID { get; set; } = null!;
    public Animation Animation { get; set; } = null!;
    public AbstractAsset[] Assets { get; set; } = null!;

    public MovingPlatform() { }
    private MovingPlatform(XElement e) : base(e)
    {
        PlatID = e.GetAttribute("PlatID");
        //Animation is always supposed to exist
        //The game technically supports it not existing
        //In which case the moving platform doesn't exist
        Animation = e.DeserializeRequiredChildOfType<Animation>();
        Assets = e.DeserializeAssetChildren();
        foreach (AbstractAsset a in Assets)
            a.Parent = this;
    }
    public static MovingPlatform Deserialize(XElement e) => new(e);

    public override void Serialize(XElement e)
    {
        e.SetAttributeValue("PlatID", PlatID);
        base.Serialize(e);
        e.Add(Animation.SerializeToXElement());
        foreach (AbstractAsset a in Assets)
            e.Add(a.SerializeToXElement());
    }

    public void StoreMovingPlatformOffset(RenderContext ctx, TimeSpan time)
    {
        ((double offX, double offY, double rot), (double anmX, double anmY)) = Animation.GetOffset(ctx, time);
        ctx.PlatIDDynamicOffset[PlatID] = (offX - anmX, offY - anmY);
        ctx.PlatIDMovingPlatformTransform[PlatID] = Transform.CreateFrom(
            x: offX + Math.Round(X * 100) / 100,
            y: offY + Math.Round(Y * 100) / 100,
            rot: rot * Math.PI / 180
        );
    }

    public override void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        if (!context.PlatIDMovingPlatformTransform.TryGetValue(PlatID, out Transform platTrans))
            throw new InvalidOperationException($"Plat ID dictionary did not contain plat id {PlatID} when attempting to draw MovingPlatform. Make sure to call {nameof(StoreMovingPlatformOffset)}.");

        Transform childTrans = trans * platTrans;
        foreach (AbstractAsset a in Assets)
            a.DrawOn(canvas, childTrans, config, context, state);
    }
}