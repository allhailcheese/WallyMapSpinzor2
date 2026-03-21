using System;
using System.IO;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public abstract class AbstractAsset : ISerializable, IDrawable
{
    public string? AssetName { get; set; }
    public double Rotation { get; set; }
    public double ScaleX { get; set; }
    public double ScaleY { get; set; }
    public double? W { get; set; }
    public double? H { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public AbstractAsset? Parent { get; set; }

    public AbstractAsset() { }
    protected AbstractAsset(XElement e)
    {
        AssetName = e.GetAttributeOrNull("AssetName");
        Rotation = e.GetDoubleAttribute("Rotation", 0);
        double scale = e.GetDoubleAttribute("Scale", 1);
        ScaleX = e.GetDoubleAttribute("ScaleX", scale);
        ScaleY = e.GetDoubleAttribute("ScaleY", scale);
        W = e.GetDoubleAttributeOrNull("W");
        H = e.GetDoubleAttributeOrNull("H");
        X = e.GetDoubleAttribute("X", 0);
        Y = e.GetDoubleAttribute("Y", 0);
    }

    public virtual void Serialize(XElement e)
    {
        if (AssetName is not null)
            e.SetAttributeValue("AssetName", AssetName);

        if (Rotation != 0)
            e.SetAttributeValue("Rotation", Rotation);

        bool hasScaleX = AssetName is null || W is null;
        bool hasScaleY = AssetName is null || H is null;
        if (hasScaleX && hasScaleY && ScaleX == ScaleY)
        {
            if (ScaleX != 1)
                e.SetAttributeValue("Scale", ScaleX);
        }
        else
        {
            if (hasScaleX && ScaleX != 1)
                e.SetAttributeValue("ScaleX", ScaleX);
            if (hasScaleY && ScaleY != 1)
                e.SetAttributeValue("ScaleY", ScaleY);
        }

        if (H is not null)
            e.SetAttributeValue("H", H);
        if (W is not null)
            e.SetAttributeValue("W", W);
        if (X != 0)
            e.SetAttributeValue("X", X);
        if (Y != 0)
            e.SetAttributeValue("Y", Y);
    }

    public virtual void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        if (!config.ShowAssets)
            return;

        if (AssetName is null)
            return;

        if (context.AssetDir is null)
            throw new InvalidOperationException("Attempting to draw an asset, but the render context is missing the AssetDir.");

        string path = Path.Combine(context.AssetDir, AssetName);
        canvas.DrawTextureRect(path, 0, 0, W, H, trans * Transform, DrawPriorityEnum.MIDGROUND, this);
    }

    public Transform Transform =>
        AssetName is null
        ? Transform.CreateFrom(
            x: X,
            y: Y,
            rot: Rotation * Math.PI / 180,
            scaleX: ScaleX,
            scaleY: ScaleY
        )
        : Transform.CreateFrom(
            x: X,
            y: Y,
            rot: Rotation * Math.PI / 180,
            scaleX: W is not null ? 1 : ScaleX,
            scaleY: H is not null ? 1 : ScaleY
        );
}