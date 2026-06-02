namespace Datateal.Ui.Client;

/// <summary>
/// Compares items by their topological position in a dependency graph.
/// Items with no dependencies sort first. Topologically equal items are ordered
/// by an optional fallback comparer.
/// </summary>
public sealed class TopologicalComparer<TItem, TKey> : IComparer<TItem>
    where TKey : notnull
{
    private readonly TKey[] _order;
    private readonly Func<TItem?, TKey> _keySelector;
    private readonly IComparer<TItem>? _tieBreaker;

    public TopologicalComparer(
        IEnumerable<TItem> items,
        Func<TItem?, TKey> keySelector,
        Func<TItem, IEnumerable<TKey>> dependenciesSelector,
        IComparer<TItem>? tieBreaker = null)
    {
        _keySelector = keySelector;
        _tieBreaker = tieBreaker;

        var itemsArray = items.ToArray();
        var withDeps = itemsArray.Where(i => dependenciesSelector(i).Any()).ToArray();
        _order = BuildTopologicalOrder(withDeps, keySelector, dependenciesSelector)
            .Select(keySelector)
            .ToArray();
    }

    public int Compare(TItem? x, TItem? y)
    {
        var xi = Array.IndexOf(_order, _keySelector(x));
        var yi = Array.IndexOf(_order, _keySelector(y));
        if (xi == yi && _tieBreaker is not null)
            return _tieBreaker.Compare(x, y);
        return xi.CompareTo(yi);
    }

    private static IEnumerable<TItem> BuildTopologicalOrder(
        TItem[] items,
        Func<TItem?, TKey> keySelector,
        Func<TItem, IEnumerable<TKey>> dependenciesSelector)
    {
        var stack = new Stack<TItem>();
        var visited = new Dictionary<TKey, VisitState>();

        foreach (var item in items)
            Visit(item, items, keySelector, dependenciesSelector, visited, stack);

        return stack.Reverse();
    }

    private static void Visit(
        TItem current,
        TItem[] items,
        Func<TItem?, TKey> keySelector,
        Func<TItem, IEnumerable<TKey>> dependenciesSelector,
        Dictionary<TKey, VisitState> visited,
        Stack<TItem> stack)
    {
        var key = keySelector(current);
        if (!visited.TryGetValue(key, out var state) || state == VisitState.NotVisited)
        {
            visited[key] = VisitState.Visiting;
            foreach (var dep in items.Where(i => dependenciesSelector(current).Contains(keySelector(i))))
                Visit(dep, items, keySelector, dependenciesSelector, visited, stack);
            visited[key] = VisitState.Visited;
            stack.Push(current);
        }
        // Visiting (cycle) and Visited states are intentionally skipped.
    }

    private enum VisitState { NotVisited, Visiting, Visited }
}
