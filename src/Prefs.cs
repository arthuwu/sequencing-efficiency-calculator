public class Prefs
{
    public required List<Route> Routes { get; init; }
}

public class Route
{
    public required string Name { get; init; }
    public required string FplHas { get; init; }
    public required double[] Startfix { get; init; }
    public required double[] Endfix { get; init; }
    public required int[] StartAlt { get; init; }
    public required int[] EndAlt { get; init; }
}