namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests;

public class EditorObjectsReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(EditorObjectsReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
