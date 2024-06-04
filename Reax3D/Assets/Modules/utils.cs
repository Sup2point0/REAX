using System.Collections.Generic;


/// <summary>
/// General utilies for use throughout the entire project.
/// </summary>
public static class Utils
{
    public const string Tab = "\t";

    public static string RenderDebugInfo(Dictionary<string, object> data)
    {
        List<string> result = new();

        foreach (var each in data) {
            result.Add($"{each.Key} = <color={Colours.Accent}>{each.Value}</color>");
        }

        return string.Join("\n", result);
    }
}
