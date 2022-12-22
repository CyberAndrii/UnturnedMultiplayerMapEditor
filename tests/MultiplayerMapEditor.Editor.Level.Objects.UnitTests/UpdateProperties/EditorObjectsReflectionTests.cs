using MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests.UpdateProperties;

public class EditorLevelObjectsUIReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(EditorLevelObjectsUIReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
