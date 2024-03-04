namespace CombatDicesTeam.Graphs.PathFinding;

/// <summary>
/// Internal structure for the A* pathfinding algorithm.
/// </summary>
internal class AStarData<TPayload>
{
    /// <summary>
    /// Estimating the distance from the current vertex to the target.
    /// </summary>
    public int EstimateCost { get; set; }

    /// <summary>
    /// Cost of moving from the starting vertex to this node.
    /// Usually denoted as g(x).
    /// </summary>
    public int MovementCost { get; set; }

    /// <summary>
    /// Parent node.
    /// </summary>
    public IGraphNode<TPayload>? Parent { get; set; }

    /// <summary>
    /// Total cost.
    /// </summary>
    public int TotalCost => MovementCost + EstimateCost;
}