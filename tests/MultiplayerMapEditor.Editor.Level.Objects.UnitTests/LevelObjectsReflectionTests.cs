namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests;

public class LevelObjectsReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(LevelObjectsReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
