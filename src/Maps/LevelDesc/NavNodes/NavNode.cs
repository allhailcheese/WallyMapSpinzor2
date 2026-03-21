using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WallyMapSpinzor2;

public sealed class NavNode : IDeserializable<NavNode>, ISerializable, IDrawable
{
    public uint NavID { get; set; }
    public NavNodeTypeEnum Type { get; set; }
    // the given type in the path doesn't actually do anything
    public (uint, NavNodePathTypeFlags)[] Path { get; set; } = null!;
    public double X { get; set; }
    public double Y { get; set; }

    public DynamicNavNode? Parent { get; set; }

    public NavNode() { }
    private NavNode(XElement e)
    {
        (NavID, Type) = ParseNavID(e.GetAttribute("NavID"));
        //the "not empty" is a guard against an empty path, where an empty string would be passed to ParseNavID
        Path = [.. e.GetAttribute("Path").Split(',').Where(s => s != "").Select(ParsePathNavID)];
        X = e.GetDoubleAttribute("X", 0);
        Y = e.GetDoubleAttribute("Y", 0);
    }
    public static NavNode Deserialize(XElement e) => new(e);

    public static (uint, NavNodeTypeEnum) ParseNavID(string navId)
    {
        ReadOnlySpan<char> id = navId.AsSpan();
        if (id.Length == 0)
            return (0, NavNodeTypeEnum._);

        NavNodeTypeEnum type = id[0] switch
        {
            'W' => NavNodeTypeEnum.W,
            'A' => NavNodeTypeEnum.A,
            'L' => NavNodeTypeEnum.L,
            'G' => NavNodeTypeEnum.G,
            'T' => NavNodeTypeEnum.T,
            'S' => NavNodeTypeEnum.S,
            _ => NavNodeTypeEnum._,
        };

        if (type != NavNodeTypeEnum._)
            id = id[1..];

        uint parsedId = (uint)Utils.AS3ParseInt(id);
        return (parsedId, type);
    }

    // the path type is parsed by the game, but ignored
    public static (uint, NavNodePathTypeFlags) ParsePathNavID(string navId)
    {
        NavNodePathTypeFlags result = 0;

        ReadOnlySpan<char> id = navId.AsSpan();
        while (id.Length > 0 && (id[0] < '0' || id[0] > '9'))
        {
            NavNodePathTypeFlags curr = id[0] switch
            {
                'D' => NavNodePathTypeFlags.D,
                'A' => NavNodePathTypeFlags.A,
                'L' => NavNodePathTypeFlags.L,
                'G' => NavNodePathTypeFlags.G,
                'T' => NavNodePathTypeFlags.T,
                'S' => NavNodePathTypeFlags.S,
                _ => 0,
            };
            result |= curr;
            id = id[1..];
        }
        uint parsedId = (uint)Utils.AS3ParseInt(id);

        return (parsedId, result);
    }

    public static string NavIDToString(uint id, NavNodeTypeEnum type)
    {
        return type == NavNodeTypeEnum._
            ? id.ToString()
            : $"{type}{id}";
    }

    public static string PathNavIDToString(uint id, NavNodePathTypeFlags type)
    {
        StringBuilder sb = new();
        if (type.HasFlag(NavNodePathTypeFlags.D)) sb.Append('D');
        if (type.HasFlag(NavNodePathTypeFlags.A)) sb.Append('A');
        if (type.HasFlag(NavNodePathTypeFlags.L)) sb.Append('L');
        if (type.HasFlag(NavNodePathTypeFlags.G)) sb.Append('G');
        if (type.HasFlag(NavNodePathTypeFlags.T)) sb.Append('T');
        if (type.HasFlag(NavNodePathTypeFlags.S)) sb.Append('S');
        sb.Append(id);
        return sb.ToString();
    }

    private static Color GetNavIDColor(NavNodeTypeEnum type, RenderConfig config) => type switch
    {
        NavNodeTypeEnum.W => config.ColorNavNodeW,
        NavNodeTypeEnum.A => config.ColorNavNodeA,
        NavNodeTypeEnum.L => config.ColorNavNodeL,
        NavNodeTypeEnum.G => config.ColorNavNodeG,
        NavNodeTypeEnum.T => config.ColorNavNodeT,
        NavNodeTypeEnum.S => config.ColorNavNodeS,
        _ => config.ColorNavNode
    };

    public void Serialize(XElement e)
    {
        e.SetAttributeValue("NavID", NavIDToString(NavID, Type));
        e.SetAttributeValue("Path", string.Join(',', Path.Select(_ => PathNavIDToString(_.Item1, _.Item2))));
        if (X != 0)
            e.SetAttributeValue("X", X);
        if (Y != 0)
            e.SetAttributeValue("Y", Y);
    }

    public void RegisterNavNode(RenderContext data, double xOff = 0, double yOff = 0)
    {
        data.NavIDDictionary[NavID] = (X + xOff, Y + yOff);
    }

    public void DrawOn(ICanvas canvas, Transform trans, RenderConfig config, RenderContext context, RenderState state)
    {
        if (!config.ShowNavNode) return;
        (double x, double y) = trans * (X, Y);
        canvas.DrawCircle(x, y, config.RadiusNavNode, GetNavIDColor(Type, config), Transform.IDENTITY, DrawPriorityEnum.NAVNODE, this);
        foreach ((uint id, _) in Path)
        {
            if (context.NavIDDictionary.TryGetValue(id, out (double, double) pos))
            {
                (double targetX, double targetY) = pos;
                canvas.DrawArrow(x, y, targetX, targetY, config.OffsetNavLineArrowSide, config.OffsetNavLineArrowBack, config.ColorNavPath, Transform.IDENTITY, DrawPriorityEnum.NAVLINE, this);
            }
        }
    }
}