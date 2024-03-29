﻿namespace CombatDicesTeam.Graphs.PathFinding;

/// <summary>
/// Interface to setup and run the AStar algorithm.
/// </summary>
/// <remarks>
/// https://en.wikipedia.org/wiki/A*
/// The general algorithm is:
/// 1. At the beginning, the starting node is placed in the list of open nodes.
/// 2. The first node that is not in the list of closed ones is selected from the open list.
/// The first one chosen will be the cheapest, because the open list is sorted.
/// 3. We get all the neighbors of the node and place them in an open list. Neighbors must not be on the closed list.
/// 4. For each neighbor, we remember how we came to him.
/// 5. At the end, following the marks of how we arrived, we restore the entire path.
/// </remarks>
public sealed class AStar<TPayload>
{
    /// <summary>
    /// The closed list.
    /// </summary>
    private readonly HashSet<IGraphNode<TPayload>> _closedList;

    private readonly IAStarContext<TPayload> _context;
    private readonly Dictionary<IGraphNode<TPayload>, AStarData<TPayload>> _dataDict;

    /// <summary>
    /// The open list.
    /// </summary>
    private readonly SortedList<int, IGraphNode<TPayload>> _openList;

    /// <summary>
    /// The goal node.
    /// </summary>
    private IGraphNode<TPayload> _goal;

    /// <summary>
    /// Creates a new AStar algorithm instance with the provided start and goal nodes.
    /// </summary>
    /// <param name="context"> Search execution context. </param>
    /// <param name="start">The starting node for the AStar algorithm.</param>
    /// <param name="goal">The goal node for the AStar algorithm.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AStar(IAStarContext<TPayload> context, IGraphNode<TPayload> start, IGraphNode<TPayload> goal)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _openList = new SortedList<int, IGraphNode<TPayload>>(DuplicateComparer.Instance);
        _closedList = new HashSet<IGraphNode<TPayload>>();
        _dataDict = new Dictionary<IGraphNode<TPayload>, AStarData<TPayload>>();

        _context = context ?? throw new ArgumentNullException(nameof(context));

        Reset(start, goal);
    }

    /// <summary>
    /// Gets the current node that the AStar algorithm is at.
    /// </summary>
    public IGraphNode<TPayload> CurrentNode { get; private set; }

    /// <summary>
    /// Gets the path of the last solution of the AStar algorithm.
    /// Will return a partial path if the algorithm has not finished yet.
    /// </summary>
    /// <returns>Returns empty if the algorithm has never been run.</returns>
    public IGraphNode<TPayload>[] GetPath()
    {
        if (CurrentNode is null)
        {
            return Array.Empty<IGraphNode<TPayload>>();
        }

        var next = CurrentNode;
        var path = new List<IGraphNode<TPayload>>();
        while (next != null)
        {
            path.Add(next);

            var nextData = GetData(next);

            next = nextData.Parent;
        }

        path.Reverse();
        return path.ToArray();
    }

    /// <summary>
    /// Steps the AStar algorithm forward until it either fails or finds the goal node.
    /// </summary>
    /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
    public State Run()
    {
        // Continue searching until either failure or the goal node has been found.
        while (true)
        {
            var state = Step();
            if (state != State.Searching)
            {
                return state;
            }
        }
    }

    private AStarData<TPayload> GetData(IGraphNode<TPayload> node)
    {
        if (_dataDict.TryGetValue(node, out var data))
        {
            return data;
        }

        data = new AStarData<TPayload>();
        _dataDict.Add(node, data);

        return data;
    }

    /// <summary>
    /// Resets the AStar algorithm with the newly specified start node and goal node.
    /// </summary>
    /// <param name="start">The starting node for the AStar algorithm.</param>
    /// <param name="goal">The goal node for the AStar algorithm.</param>
    private void Reset(IGraphNode<TPayload> start, IGraphNode<TPayload> goal)
    {
        _openList.Clear();
        _closedList.Clear();
        _dataDict.Clear();

        CurrentNode = start;
        _goal = goal;

        var currentData = GetData(CurrentNode);
        _openList.AddWithData(CurrentNode, currentData);
    }

    /// <summary>
    /// Moves the AStar algorithm forward one step.
    /// </summary>
    /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
    private State Step()
    {
        while (true)
        {
            // There are no more nodes to search, return failure.
            if (_openList.IsEmpty())
            {
                return State.Failed;
            }

            // Check the next best node in the graph by TotalCost.
            CurrentNode = _openList.Pop();

            // This node has already been searched, check the next one.
            if (_closedList.Contains(CurrentNode))
            {
                continue;
            }

            // An unsearched node has been found, search it.
            break;
        }

        // Remove from the open list and place on the closed list 
        // since this node is now being searched.

        var currentData = GetData(CurrentNode);

        _openList.Remove(currentData.TotalCost);

        _closedList.Add(CurrentNode);

        // Found the goal, stop searching.
        if (CurrentNode == _goal)
        {
            return State.GoalFound;
        }

        // Node was not the goal so add all children nodes to the open list.
        // Each child needs to have its movement cost set and estimated cost.

        var neighbors = _context.GetNext(CurrentNode);

        foreach (var child in neighbors)
        {
            // If the child has already been searched (closed list) or is on
            // the open list to be searched then do not modify its movement cost
            // or estimated cost since they have already been set previously.
            if (_openList.ContainsValue(child) || _closedList.Contains(child))
            {
                continue;
            }

            var childData = GetData(child);
            currentData = GetData(CurrentNode);

            childData.Parent = CurrentNode;
            childData.MovementCost = currentData.MovementCost + 1;
            childData.EstimateCost = _context.GetDistanceBetween(CurrentNode, _goal);

            _openList.AddWithData(child, childData);
        }

        // This step did not find the goal so return status of still searching.
        return State.Searching;
    }
}