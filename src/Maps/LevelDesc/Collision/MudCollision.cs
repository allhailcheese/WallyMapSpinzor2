using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class MudCollision : AbstractCollision, IDeserializable<MudCollision>
{
    public MudCollision() : base() { }
    private MudCollision(XElement e) : base(e) { }
    public static MudCollision Deserialize(XElement e) => new(e);

    public override Color GetColor(RenderConfig config) => config.ColorMudCollision;
    public override CollisionTypeFlags CollisionType => CollisionTypeFlags.GAMEMODE;
}