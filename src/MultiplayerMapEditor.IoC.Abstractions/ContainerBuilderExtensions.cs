namespace MultiplayerMapEditor.IoC.Abstractions;

public static class ContainerBuilderExtensions
{
    public static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
        RegisterTypeTernary<TImplementer1, TImplementer2>(this ContainerBuilder builder, bool isFirst)
        where TImplementer1 : notnull
        where TImplementer2 : notnull
    {
        return builder.RegisterType(isFirst ? typeof(TImplementer1) : typeof(TImplementer2));
    }

    public static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
        RegisterTypeTernary(this ContainerBuilder builder, Type type1, Type type2, bool isFirst)
    {
        return builder.RegisterType(isFirst ? type1 : type2);
    }
}
