namespace How.Common.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<TV, TK> ReverseKEyValue<TK, TV>(this IDictionary<TK, TV> dict)
    {
        return dict.ToDictionary(x => x.Value, x => x.Key);
    }
}