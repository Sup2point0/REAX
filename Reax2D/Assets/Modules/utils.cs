using System.Collections.Generic;

/// <summary>
/// General utilies for use throughout the entire project.
/// </summary>
public static class Utils
{
    public const string Tab = "\t";

    public static (string, string) Order((string, string) keys)
        => keys.Item1.CompareTo(keys.Item2) > 0
            ? (keys.Item2, keys.Item1)
            : (keys.Item1, keys.Item2);

    public static string RenderDebugInfo(Dictionary<string, object> data)
    {
        List<string> result = new();

        foreach (var each in data) {
            if (each.Key != "") {
                result.Add($"{each.Key} = <color={Colours.Accent}>{each.Value}</color>");
            } else {
                result.Add("");
            }
        }

        return string.Join("\n", result);
    }
}
