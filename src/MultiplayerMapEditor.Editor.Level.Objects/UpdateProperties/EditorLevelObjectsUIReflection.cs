using System.Reflection;
using static System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal static class EditorLevelObjectsUIReflection
{
    private static readonly FieldInfo MaterialPaletteOverrideFieldField =
        typeof(SDG.Unturned.EditorLevelObjectsUI).GetField("materialPaletteOverrideField", Static | NonPublic)
        ?? throw new MissingFieldException(nameof(SDG.Unturned.EditorLevelObjectsUI), "materialPaletteOverrideField");

    private static readonly FieldInfo MaterialIndexOverrideFieldField =
        typeof(SDG.Unturned.EditorLevelObjectsUI).GetField("materialIndexOverrideField", Static | NonPublic)
        ?? throw new MissingFieldException(nameof(SDG.Unturned.EditorLevelObjectsUI), "materialIndexOverrideField");

    private static readonly FieldInfo FocusedLevelObjectField =
        typeof(SDG.Unturned.EditorLevelObjectsUI).GetField("focusedLevelObject", Static | NonPublic)
        ?? throw new MissingFieldException(nameof(SDG.Unturned.EditorLevelObjectsUI), "focusedLevelObject");

    public static ISleekField? GetMaterialPaletteOverrideField()
    {
        return (ISleekField?)MaterialPaletteOverrideFieldField.GetValue(null);
    }

    public static ISleekInt32Field? GetMaterialIndexOverrideField()
    {
        return (ISleekInt32Field?)MaterialIndexOverrideFieldField.GetValue(null);
    }

    public static LevelObject? GetFocusedLevelObject()
    {
        return (LevelObject?)FocusedLevelObjectField.GetValue(null);
    }
}
