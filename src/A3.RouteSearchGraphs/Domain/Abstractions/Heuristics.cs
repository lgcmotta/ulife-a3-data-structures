namespace A3.RouteSearchGraphs.Domain.Abstractions;

internal static class Heuristics
{
    internal static int ResolveGridDimension(int nodes)
    {
        var dimension = (int)Math.Round(Math.Sqrt(nodes));
        return dimension > 0 && dimension * dimension == nodes ? dimension : -1;
    }

    internal static double Calculate(HeuristicKind kind, int from, int to, int dimension) => kind switch
    {
        HeuristicKind.Manhattan => Manhattan(from, to, dimension),
        HeuristicKind.Euclidean => Euclidean(from, to, dimension),
        _ => 0
    };

    private static double Manhattan(int from, int to, int dimension)
    {
        if (dimension <= 0)
        {
            return 0;
        }

        var (fromRow, fromCol) = ToGridPosition(from, dimension);
        var (toRow, toCol) = ToGridPosition(to, dimension);

        return Math.Abs(fromRow - toRow) + Math.Abs(fromCol - toCol);
    }

    private static double Euclidean(int from, int to, int dimension)
    {
        if (dimension <= 0)
        {
            return 0;
        }

        var (fromRow, fromCol) = ToGridPosition(from, dimension);
        var (toRow, toCol) = ToGridPosition(to, dimension);

        var dx = fromRow - toRow;
        var dy = fromCol - toCol;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static (int row, int column) ToGridPosition(int node, int dimension)
    {
        var row = node / dimension;
        var column = node % dimension;

        return (row, column);
    }
}