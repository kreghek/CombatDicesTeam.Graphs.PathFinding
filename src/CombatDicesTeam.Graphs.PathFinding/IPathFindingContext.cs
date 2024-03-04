namespace CombatDicesTeam.Graphs.PathFinding;

public interface IPathFindingContext<TPayload>
{
    IEnumerable<IGraphNode<TPayload>> GetNext(IGraphNode<TPayload> current);
}