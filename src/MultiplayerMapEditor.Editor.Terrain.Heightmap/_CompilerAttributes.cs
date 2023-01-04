// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor)]
    internal class SetsRequiredMembersAttribute : Attribute
    {
    }
}

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    {
    }

    internal sealed class RequiredMemberAttribute
    {
    }

    internal sealed class CompilerFeatureRequiredAttribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
        }
    }
}
