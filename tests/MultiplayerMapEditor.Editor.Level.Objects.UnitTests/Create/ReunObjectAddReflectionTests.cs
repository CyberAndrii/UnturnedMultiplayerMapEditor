namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests.Create;

public class ReunObjectAddReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(ReunObjectAddReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
