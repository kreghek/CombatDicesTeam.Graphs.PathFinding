namespace CombatDicesTeam.Graphs.PathFinding;

/// <summary>
/// System.Collections.Generic.SortedList by default does not allow duplicate items.
/// Since items are keyed by TotalCost there can be duplicate entries per key.
/// </summary>
internal class DuplicateComparer : IComparer<int>
{
    static DuplicateComparer()
    {
        Instance = new DuplicateComparer();
    }

    private DuplicateComparer()
    {
    }

    public static DuplicateComparer Instance { get; }

    public int Compare(int x, int y)
    {
        return x <= y ? -1 : 1;
    }
}