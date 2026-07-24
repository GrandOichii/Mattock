namespace Mattock.Core.Utility;

public static class Common
{
    public static T[][] GetCombinations<T>(IEnumerable<T> items, int n)
    {
    if (n == 0)
        return [ [] ];
    
    var result = new List<T[]>();
    var itemList = items.ToList();
    
    if (n > itemList.Count)
        return [.. result];
    
    for (int i = 0; i < itemList.Count; i++)
    {
        var currentItem = itemList[i];
        var remainingItems = itemList.Skip(i + 1);
        var subCombinations = GetCombinations(remainingItems, n - 1);
        
        foreach (var subCombination in subCombinations)
        {
            result.Add([.. new[] { currentItem }.Concat(subCombination)]);
        }
    }
    
    return [.. result];
}
}