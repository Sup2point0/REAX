using System;
using System.Collections.Generic;

public class AdjacencyMatrix<T>
{
    private Dictionary<(string, string), T> _data;

    public AdjacencyMatrix()
        => _data = new();

    public AdjacencyMatrix(Dictionary<(string, string), T> data)
    {
        _data = new();
        foreach (var item in data) {
            _data[Utils.Order(item.Key)] = item.Value;
        }
    }

    public T this[string prot, string deut]
    {
        get => _data[Utils.Order((prot, deut))];
        set => _data[Utils.Order((prot, deut))] = value;
    }

    public T this[(string prot, string deut) items]
        => this[items.prot, items.deut];

    public bool TryGetValue((string, string) keys, out T value)
        => _data.TryGetValue(Utils.Order((keys.Item1, keys.Item2)), out value);
}
