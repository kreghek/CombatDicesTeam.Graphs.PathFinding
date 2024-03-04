namespace CombatDicesTeam.Graphs.PathFinding;

/// <summary>
/// Extension methods to make the System.Collections.Generic.SortedList easier to use.
/// </summary>
internal static class SortedListExtensions
{
    /// <summary>
    /// Adds a INode to the SortedList.
    /// </summary>
    /// <param name="sortedList">SortedList to add the node to.</param>
    /// <param name="node">Node to add to the sortedList.</param>
    /// <param name="data"> Node data for the algorithm. </param>
    internal static void AddWithData<TPayload>(this SortedList<int, IGraphNode<TPayload>> sortedList,
        IGraphNode<TPayload> node, AStarData<TPayload> data)
    {
        sortedList.Add(data.TotalCost, node);
    }

    /// <summary>
    /// Checks if the SortedList is empty.
    /// </summary>
    /// <param name="sortedList">SortedList to check if it is empty.</param>
    /// <returns>True if sortedList is empty, false if it still has elements.</returns>
    internal static bool IsEmpty<TKey, TValue>(this SortedList<TKey, TValue> sortedList) where TKey : notnull
    {
        return sortedList.Count == 0;
    }

    /// <summary>
    /// Removes the node from the sorted list with the smallest TotalCost and returns that node.
    /// </summary>
    /// <param name="sortedList">SortedList to remove and return the smallest TotalCost node.</param>
    /// <returns>Node with the smallest TotalCost.</returns>
    internal static IGraphNode<TPayload> Pop<TPayload>(this SortedList<int, IGraphNode<TPayload>> sortedList)
    {
        var top = sortedList.Values[0];
        sortedList.RemoveAt(0);
        return top;
    }
}