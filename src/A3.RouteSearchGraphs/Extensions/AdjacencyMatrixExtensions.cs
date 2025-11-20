namespace A3.RouteSearchGraphs.Extensions;

internal static class AdjacencyMatrixExtensions
{
    private static readonly char[] Separators = [' ', '\t', ',', ';'];

    extension(string filePath)
    {
        internal async ValueTask<int[][]> ParseAdjacencyMatrix(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                ArgumentException.ThrowIfNullOrWhiteSpace("File path must be provided.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Input file '{filePath}' was not found.", filePath);
            }

            var content = await File.ReadAllLinesAsync(filePath, cancellationToken);

            var lines = content.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            if (lines.Length == 0)
            {
                throw new InvalidDataException("The input file is empty or contains no valid lines.");
            }

            var matrix = new int[lines.Length][];

            var expectedColumns = -1;

            for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
            {
                var columns = lines[rowIndex].Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                if (expectedColumns == -1)
                {
                    expectedColumns = columns.Length;
                }
                else if (columns.Length != expectedColumns)
                {
                    throw new InvalidDataException($"Line {rowIndex + 1} has {columns.Length} columns, expected {expectedColumns}.");
                }

                matrix[rowIndex] = new int[columns.Length];

                for (var col = 0; col < columns.Length; col++)
                {
                    if (!int.TryParse(columns[col], out var value))
                    {
                        throw new InvalidDataException($"Unable to parse value '{columns[col]}' at line {rowIndex + 1}, column {col + 1}.");
                    }

                    matrix[rowIndex][col] = value;
                }
            }

            return expectedColumns == lines.Length
                ? matrix
                : throw new InvalidDataException($"The matrix is not square: {lines.Length} rows and {expectedColumns} columns.");
        }
    }
}