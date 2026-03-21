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
        /*
        Animation is always supposed to exist
        The game technically supports it not existing
        In which case the moving platform doesn't exist
        */
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

    public void StoreMovingPlatformOffset(RenderContext context, TimeSpan time)
    {
        /*
        If there are multiple MovingPlatforms with the same numeric PlatID,
        the first one's Animation is used for the others too.

        Thanks bmg.
        */
        uint platID = (uint)Utils.AS3ParseInt(PlatID);
        context.AnimationByPlatID.TryAdd(platID, Animation);

        Animation animation = context.AnimationByPlatID[platID];

        ((double offX, double offY, double rot), (double anmX, double anmY)) = animation.GetOffset(context, time);

        if (context.MovingPlatformByPlatID.TryGetValue(PlatID, out var list))
            list.Add(this);
        else
            context.MovingPlatformByPlatID[PlatID] = [this];

        context.MovingPlatformDynamicOffset[this] = (offX - anmX, offY - anmY);

        context.MovingPlatformTransform[this] = Transform.CreateFrom(
            x: offX + X,
            y: offY + Y,
            rot: rot * Math.PI / 180
        );
    }

    public override void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        if (!context.MovingPlatformTransform.TryGetValue(this, out Transform platTrans))
            throw new InvalidOperationException($"Moving platform transform dictionary did not contain moving platform with PlatID {PlatID} when attempting to draw MovingPlatform. Make sure to call {nameof(StoreMovingPlatformOffset)}.");

        Transform childTrans = trans * platTrans;
        foreach (AbstractAsset a in Assets)
            a.DrawOn(canvas, childTrans, config, context, state);
    }
}