using Sudoku.ConsoleApp.Domain;


namespace Sudoku.ConsoleApp.Application.Solvers.Heuristics
{
    public interface IHeuristic
    {
        Position? SelectVariable(Board board);
    }

    public interface IValueOrder
    {
        IEnumerable<int> OrderValues(Board board, Position pos, IEnumerable<int> candidates);
    }
}
