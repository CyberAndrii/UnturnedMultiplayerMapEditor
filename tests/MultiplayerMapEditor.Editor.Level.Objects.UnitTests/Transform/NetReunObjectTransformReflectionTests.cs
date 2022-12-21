using MultiplayerMapEditor.Editor.Level.Objects.Transform;

namespace MultiplayerMapEditor.Editor.Level.Objects.UnitTests.Transform;

public class NetReunObjectTransformReflectionTests
{
    [Fact]
    public void StaticCtorDoesNotThrow()
    {
        var type = typeof(ReunObjectTransformReflection);
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
    }
}
