using Sudoku.ConsoleApp.Domain;


namespace Sudoku.ConsoleApp.Application.Solvers.Consistency;

    public interface IConstraintPropagator
    {
        IEnumerable<int> Candidates(Board b, Position p);
        bool IsConsistent(Board b, Position p, int v);

        /// <summary>Aplica poda (Forward Checking) y devuelve snapshot reversible.</summary>
        object Push(Board b, Position assigned, int value);
        void Pop(Board b, object snapshot);
    }

