using System.Linq;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

internal static class MapXmlExtensions
{
    public static AbstractCollision[] DeserializeCollisionChildren(this XElement e) =>
        [.. e.Elements().Select(DeserializeCollision).Where(c => c is not null)!];

    public static AbstractCollision? DeserializeCollision(this XElement e) => e.Name.LocalName switch
    {
        nameof(HardCollision) => e.DeserializeTo<HardCollision>(),
        nameof(SoftCollision) => e.DeserializeTo<SoftCollision>(),
        nameof(NoSlideCollision) => e.DeserializeTo<NoSlideCollision>(),
        nameof(BouncyHardCollision) => e.DeserializeTo<BouncyHardCollision>(),
        nameof(BouncySoftCollision) => e.DeserializeTo<BouncySoftCollision>(),
        nameof(BouncyNoSlideCollision) => e.DeserializeTo<BouncyNoSlideCollision>(),
        nameof(GameModeHardCollision) => e.DeserializeTo<GameModeHardCollision>(),
        nameof(StickyCollision) => e.DeserializeTo<StickyCollision>(),
        nameof(TriggerCollision) => e.DeserializeTo<TriggerCollision>(),
        nameof(ItemIgnoreCollision) => e.DeserializeTo<ItemIgnoreCollision>(),
        nameof(PressurePlateCollision) => e.DeserializeTo<PressurePlateCollision>(),
        nameof(SoftPressurePlateCollision) => e.DeserializeTo<SoftPressurePlateCollision>(),
        nameof(LavaCollision) => e.DeserializeTo<LavaCollision>(),
        nameof(MudCollision) => e.DeserializeTo<MudCollision>(),
        _ => null
    };

    public static AbstractItemSpawn[] DeserializeItemSpawnChildren(this XElement e) =>
        [.. e.Elements().Select(DeserializeItemSpawn).Where(i => i is not null)!];

    public static AbstractItemSpawn? DeserializeItemSpawn(this XElement e) => e.Name.LocalName switch
    {
        nameof(ItemSpawn) => e.DeserializeTo<ItemSpawn>(),
        nameof(ItemInitSpawn) => e.DeserializeTo<ItemInitSpawn>(),
        nameof(ItemSet) => e.DeserializeTo<ItemSet>(),
        nameof(TeamItemInitSpawn) => e.DeserializeTo<TeamItemInitSpawn>(),
        _ => null
    };

    public static AbstractVolume[] DeserializeVolumeChildren(this XElement e) =>
        [.. e.Elements().Select(DeserializeVolume).Where(v => v is not null)!];

    public static AbstractVolume? DeserializeVolume(this XElement e) => e.Name.LocalName switch
    {
        nameof(Volume) => e.DeserializeTo<Volume>(),
        nameof(Goal) => e.DeserializeTo<Goal>(),
        nameof(NoDodgeZone) => e.DeserializeTo<NoDodgeZone>(),
        _ => null
    };

    public static AbstractKeyFrame[] DeserializeKeyFrameChildren(this XElement e) =>
        [.. e.Elements().Select(DeserializeKeyFrame).Where(k => k is not null)!];

    public static AbstractKeyFrame? DeserializeKeyFrame(this XElement e) => e.Name.LocalName switch
    {
        nameof(KeyFrame) => e.DeserializeTo<KeyFrame>(),
        nameof(Phase) => e.DeserializeTo<Phase>(),
        _ => null
    };

    public static AbstractAsset[] DeserializeAssetChildren(this XElement e) =>
        [.. e.Elements().Select(DeserializeAsset).Where(a => a is not null)!];

    public static AbstractAsset? DeserializeAsset(this XElement e) => e.Name.LocalName switch
    {
        nameof(Platform) => e.DeserializeTo<Platform>(),
        nameof(Asset) => e.DeserializeTo<Asset>(),
        nameof(MovingPlatform) => e.DeserializeTo<MovingPlatform>(),
        _ => null
    };
}