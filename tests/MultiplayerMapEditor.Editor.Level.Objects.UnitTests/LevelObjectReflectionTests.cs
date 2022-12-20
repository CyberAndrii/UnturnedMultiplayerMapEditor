namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests;

public class LevelObjectReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(LevelObjectReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
