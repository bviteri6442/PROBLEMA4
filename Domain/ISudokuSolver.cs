using System;


namespace Sudoku.ConsoleApp.Domain;


    public sealed class SolverOptions
    {
        public bool DidacticMode { get; init; } = false;
        public bool PauseOnStep { get; init; } = true;
    }

    public sealed class SolverEvent
    {
        public int Depth { get; init; }
        public Position Pos { get; init; }
        public IReadOnlyList<int>? OrderedValues { get; init; }
        public string Kind { get; init; } = string.Empty; // "select","assign","prune","backtrack","done"
        public string Message { get; init; } = string.Empty;
    }

    public interface ISudokuSolver
    {
            event Action<SolverEvent>? OnStep;
            bool Solve(Board board, SolverOptions options);
    }

    

    

