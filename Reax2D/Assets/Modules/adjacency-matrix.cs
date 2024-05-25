using System;
using System.Collections.Generic;
using System.Net.Security;

public class AdjacencyMatrix<T>
{
    private Dictionary<(string, string), T> _data;

    public AdjacencyMatrix()
        => _data = new();

    public AdjacencyMatrix(Dictionary<(string, string), T> data)
    {
        _data = new();
        foreach (var item in data) {
            _data[Order(item.Key)] = item.Value;
        }
    }

    public T this[string prot, string deut]
    {
        get => _data[Order((prot, deut))];
        set => _data[Order((prot, deut))] = value;
    }

    private (string, string) Order((string, string) keys)
        => keys.Item1.CompareTo(keys.Item2) > 0
            ? (keys.Item2, keys.Item1)
            : (keys.Item1, keys.Item2);

    public bool TryGetValue((string, string) keys, out T value)
        => _data.TryGetValue(Order((keys.Item1, keys.Item2)), out value);
}