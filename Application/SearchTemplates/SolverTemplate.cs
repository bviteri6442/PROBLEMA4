using Sudoku.ConsoleApp.Application.Solvers.Consistency;
using Sudoku.ConsoleApp.Application.Solvers.Heuristics;
using Sudoku.ConsoleApp.Domain;
using static Sudoku.ConsoleApp.Application.Solvers.Heuristics.MrvHeuristic;
using static Sudoku.ConsoleApp.Domain.ISudokuSolver;

namespace Sudoku.ConsoleApp.Application.SearchTemplates;

    public abstract class SolverTemplate : ISudokuSolver
    {
        public event Action<SolverEvent>? OnStep;

        protected IHeuristic _selectHeuristic = new MrvHeuristic();
        protected IValueOrder _valueOrder = new LcvHeuristic();
        protected IConstraintPropagator _propagator = new ForwardChecking();

        public bool Solve(Board board, SolverOptions options) =>
            Recurse(board, 0, options);

        protected bool Recurse(Board board, int depth, SolverOptions options)
        {
            // Selección de variable (MRV)
            var pos = _selectHeuristic.SelectVariable(board);
            if (pos is null) // no quedan vacías
            {
                OnStep?.Invoke(new SolverEvent { Depth = depth, Kind = "done", Message = "Completado" });
                return true;
            }
            OnStep?.Invoke(new SolverEvent { Depth = depth, Kind = "select", Pos = pos.Value, Message = $"Elegida {pos.Value}" });

            // Candidatos por consistencia
            var candidates = _propagator.Candidates(board, pos.Value);
            var ordered = _valueOrder.OrderValues(board, pos.Value, candidates).ToList();

            OnStep?.Invoke(new SolverEvent
            {
                Depth = depth,
                Pos = pos.Value,
                Kind = "order",
                OrderedValues = ordered,
                Message = $"Orden LCV: {string.Join(",", ordered)}"
            });

            foreach (var v in ordered)
            {
                if (!_propagator.IsConsistent(board, pos.Value, v)) continue;

                int prev = board[pos.Value].Value;
                board.SetValue(pos.Value, v);
                OnStep?.Invoke(new SolverEvent { Depth = depth, Pos = pos.Value, Kind = "assign", Message = $"Asigno {v} en {pos.Value}" });

                // Propagación (Forward Checking)
                var snapshot = _propagator.Push(board, pos.Value, v);

                bool ok = Recurse(board, depth + 1, options);
                if (ok) return true;

                // Backtrack
                _propagator.Pop(board, snapshot);
                board.SetValue(pos.Value, prev);
                OnStep?.Invoke(new SolverEvent { Depth = depth, Pos = pos.Value, Kind = "backtrack", Message = $"Retroceso desde {pos.Value}" });
            }

            return false;
        }
    }

