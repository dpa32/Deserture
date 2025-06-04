using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> _data = new();

    public void Set<T>(BBKey<T> key, T value)
    {
        _data[key.Name] = value;
    }

    public T Get<T>(BBKey<T> key)
    {
        if (_data.TryGetValue(key.Name, out var value) && value is T casted)
        {
            return casted;
        }
        return default;
    }
    public bool TryGet<T>(BBKey<T> key, out T result)
    {
        if (_data.TryGetValue(key.Name, out var value) && value is T casted)
        {
            result = casted;
            return true;
        }
        result = default;
        return false;
    }

    public bool Has<T>(BBKey<T> key) => _data.ContainsKey(key.Name);
}
