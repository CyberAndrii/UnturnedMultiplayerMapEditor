namespace MultiplayerMapEditor.Common.UnitTests;

public partial class FastArrayOperationsTests
{
    public class AssignSubtract
    {
        [Theory]
        [InlineData(257, 257, true)]
        [InlineData(257, 257, false)]
        public void SubtractsAndAssigns(int rows, int cols, bool isHardwareAccelerated)
        {
            var first = new float[rows, cols];
            var second = new float[rows, cols];
            var destination = new float[rows, cols];
            var num = 0;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    first[row, col] = num++;
                    second[row, col] = num++;
                }
            }

            FastArrayOperations.AssignSubtract(first, second, destination);

            num = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    // Assert input arrays are unchanged
                    Assert.Equal(num++, first[row, col]);
                    Assert.Equal(num++, second[row, col]);

                    // Assert has valid output value
                    Assert.Equal(first[row, col] - second[row, col], destination[row, col]);
                }
            }
        }
    }
}
