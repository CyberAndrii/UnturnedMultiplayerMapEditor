namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests.Remove;

public class ReunObjectRemoveReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(ReunObjectRemoveReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
