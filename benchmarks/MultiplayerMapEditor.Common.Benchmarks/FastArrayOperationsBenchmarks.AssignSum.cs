using System.Numerics;
using System.Runtime.CompilerServices;

namespace MultiplayerMapEditor.Common.Benchmarks;

public partial class FastArrayOperationsBenchmarks
{
    [MemoryDiagnoser(displayGenColumns: false)]
    public class AssignSum
    {
        [Params(257)] public int Size { get; set; }

        public float[,] From { get; private set; } = null!;

        public float[,] To { get; private set; } = null!;

        [GlobalSetup]
        public void Setup()
        {
            From = new float[Size, Size];
            To = new float[Size, Size];
        }

        [Benchmark(Baseline = true)]
        public void ForLoop()
        {
            ForLoop(From, To, Size, Size);
        }

        [Benchmark]
        public void ForLoopUsingPointers()
        {
            ForLoopUsingPointers(From, To, Size, Size);
        }

        [Benchmark]
        public void ForLoopUsingVectors()
        {
            ForLoopUsingVectors(From, To);
        }

        [Benchmark]
        public void CurrentImplementation()
        {
            FastArrayOperations.AssignSum(From, To, To);
        }

        private void ForLoop(float[,] from, float[,] to, int rows, int cols)
        {
            for (var x = 0; x < rows; x++)
            {
                for (var y = 0; y < cols; y++)
                {
                    to[x, y] += from[x, y];
                }
            }
        }

        private unsafe void ForLoopUsingPointers(float[,] from, float[,] to, int rows, int cols)
        {
            fixed (float* fromPtr = from)
            fixed (float* toPtr = to)
            {
                for (var x = 0; x < rows; x++)
                {
                    for (var y = 0; y < cols; y++)
                    {
                        toPtr[rows * x + y] += fromPtr[rows * x + y];
                    }
                }
            }
        }

        private unsafe void ForLoopUsingVectors(float[,] from, float[,] to)
        {
            var length = from.Length;

            // The number of elements that can't be processed in the vector
            // NOTE: Vector<T>.Count is a JIT time constant and will get optimized accordingly
            var remaining = length % Vector<float>.Count;

            fixed (float* fromPtr = from)
            fixed (float* toPtr = to)
            {
                for (var i = 0; i < length - remaining; i += Vector<float>.Count)
                {
                    var fromVector = Unsafe.ReadUnaligned<Vector<float>>(ref Unsafe.AsRef<byte>(fromPtr + i));
                    var toVector = Unsafe.ReadUnaligned<Vector<float>>(ref Unsafe.AsRef<byte>(toPtr + i));

                    Unsafe.WriteUnaligned(
                        ref Unsafe.AsRef<byte>(toPtr + i),
                        fromVector + toVector
                    );
                }

                for (var i = length - remaining; i < length; i++)
                {
                    toPtr[i] += fromPtr[i];
                }
            }
        }
    }
}
