namespace CombatDicesTeam.Graphs.PathFinding;

public interface IAStarContext<TPayload> : IPathFindingContext<TPayload>
{
    int GetDistanceBetween(IGraphNode<TPayload> current, IGraphNode<TPayload> target);
}