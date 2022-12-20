using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects;

internal static class EditorObjectsReflection
{
    private static readonly FieldInfo SelectionField =
        typeof(EditorObjects).GetField("selection", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(EditorObjects), "selection");

    private static readonly MethodInfo SelectDecalsMethod =
        typeof(EditorObjects).GetMethod("selectDecals", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(EditorObjects), "selectDecals");

    private static readonly MethodInfo CalculateHandleOffsetsMethod =
        typeof(EditorObjects).GetMethod("calculateHandleOffsets", Static | NonPublic)
        ?? throw new MissingMemberException(nameof(EditorObjects), "calculateHandleOffsets");

    public static List<EditorSelection> GetSelections()
    {
        return (List<EditorSelection>)SelectionField.GetValue(null);
    }

    // https://github.com/Unturned-Datamining/Unturned-Datamining/blob/d2c09ed739163b4aa6eb2af5d7aa6119d56300d4/Assembly-CSharp/SDG.Unturned/EditorObjects.cs#L162
    public static void RemoveSelection(Transform? select)
    {
        var selections = GetSelections();

        for (var index = 0; index < selections.Count; ++index)
        {
            var selection = selections[index];

            if (selection.transform != select)
            {
                continue;
            }

            // In the original code this line was at the bottom but moved higher.
            selections.RemoveAt(index);

            // This check is missing in the original code and causes NullReferenceException.
            if (selection.transform == null)
            {
                break;
            }

            HighlighterTool.unhighlight(select);
            SelectDecals(select, false);

            if (selection.transform.CompareTag("Barricade") || selection.transform.CompareTag("Structure"))
            {
                selection.transform.localScale = Vector3.one;
            }

            break;
        }

        CalculateHandleOffsets();
    }

    public static void SelectDecals(Transform select, bool isSelected)
    {
        SelectDecalsMethod.Invoke(null, new object[] { select, isSelected });
    }

    public static void CalculateHandleOffsets()
    {
        CalculateHandleOffsetsMethod.Invoke(null, Array.Empty<object>());
    }
}
