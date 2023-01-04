namespace MultiplayerMapEditor.Common;

/// <summary>
/// Provides optimized utility methods to work with big arrays.
/// </summary>
internal static class FastArrayOperations
{
    /// <summary>
    /// <paramref name="destination"/>[i] = <paramref name="first"/>[i] + <paramref name="second"/>[i];
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="destination">An array to write the result to.</param>
    public static unsafe void AssignSum(float[,] first, float[,] second, float[,] destination)
    {
        AssertLengthEquals(first, second, destination);

        // This can be further optimized by using the System.Numerics.Vector struct
        // but at this point it doesn't worth the complication.

        var length = first.Length;

        fixed (float* firstPtr = first)
        fixed (float* secondPtr = second)
        fixed (float* destinationPtr = destination)
        {
            for (var i = 0; i < length; i++)
            {
                destinationPtr[i] = firstPtr[i] + secondPtr[i];
            }
        }
    }

    /// <summary>
    /// <paramref name="destination"/>[i] = <paramref name="first"/>[i] - <paramref name="second"/>[i];
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="destination">An array to write the result to.</param>
    public static unsafe void AssignSubtract(float[,] first, float[,] second, float[,] destination)
    {
        AssertLengthEquals(first, second, destination);

        // This can be further optimized by using the System.Numerics.Vector struct
        // but at this point it doesn't worth the complication.

        var length = first.Length;

        fixed (float* firstPtr = first)
        fixed (float* secondPtr = second)
        fixed (float* destinationPtr = destination)
        {
            for (var i = 0; i < length; i++)
            {
                destinationPtr[i] = firstPtr[i] - secondPtr[i];
            }
        }
    }

    /// <summary>
    /// Throws the <see cref="ArgumentException"/> if sizes of dimensions of the input arrays doesn't match.
    /// </summary>
    /// <param name="a">First array.</param>
    /// <param name="b">Second array.</param>
    /// <param name="c">Third array.</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException">Thrown if sizes of dimensions of the input arrays doesn't match.</exception>
    private static void AssertLengthEquals<T>(T[,] a, T[,] b, T[,] c)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);

        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);

        var cRows = c.GetLength(0);
        var cCols = c.GetLength(1);

        if (aRows != bRows || aCols != bCols || cRows != bRows || cCols != bCols)
        {
            throw new ArgumentException("Array length does not match");
        }
    }
}
