namespace DataCruncher.Day6;

public class FishStates
{
    Dictionary<int, long> fishCounts = new Dictionary<int, long>();
    public void Add(int timer)
    {
        IncrementCount(fishCounts, timer, 1);
    }

    public void Iterate()
    {
        fishCounts = GetNextState();
    }

    public long FishCount => fishCounts.Values.Sum();

    private static void IncrementCount(Dictionary<int, long> dict, int key, long incValue)
    {
        if (!dict.ContainsKey(key))
        {
            dict[key] = incValue;
        }
        else
        {
            dict[key] += incValue;
        }
    }

    private Dictionary<int, long> GetNextState()
    {
        var newState = new Dictionary<int, long>();
        foreach (var kvp in fishCounts)
        {
            if (kvp.Key == 0)
            {
                IncrementCount(newState, 6, kvp.Value);
                IncrementCount(newState, 8, kvp.Value);
            }
            else
            {
                IncrementCount(newState, kvp.Key - 1, kvp.Value);
            }
        }
        return newState;
    }
}