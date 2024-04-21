namespace How.Common.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<V, K> ReverseKEyValue<K, V>(this IDictionary<K, V> dict)
    {
        return dict.ToDictionary(x => x.Value, x => x.Key);
    }
}