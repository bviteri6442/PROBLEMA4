using Sudoku.ConsoleApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Application.Solvers.Heuristics
{
    public sealed class MrvHeuristic : IHeuristic
    {
        public Position? SelectVariable(Board board)
        {
            Position? best = null;
            int bestCount = int.MaxValue;
            var all = ForwardChecking.GetCandidates(board);
            foreach (var kv in all)
            {
                if (board[kv.Key].Value != 0) continue;
                int count = kv.Value.Count;
                if (count < bestCount) { best = kv.Key; bestCount = count; }
            }
            return best;
        }

        public sealed class LcvHeuristic : IValueOrder
        {
            public IEnumerable<int> OrderValues(Board board, Position pos, IEnumerable<int> candidates)
            {
                return candidates
                    .Select(v => (v, score: CountConflicts(board, pos, v)))
                    .OrderBy(t => t.score)
                    .Select(t => t.v);
            }

            private static int CountConflicts(Board b, Position p, int v)
            {
                int conflicts = 0;
                // cuenta cuántas veces v aparece como candidato en vecinos (si lo pongo, a cuántos afecta)
                foreach (var q in Neighbors(p))
                {
                    if (b[q].Value != 0) continue;
                    if (ForwardChecking.CandidatesFor(b, q).Contains(v)) conflicts++;
                }
                return conflicts;
            }

            private static IEnumerable<Position> Neighbors(Position p)
            {
                for (int c = 0; c < 9; c++) if (c != p.Col) yield return new Position(p.Row, c);
                for (int r = 0; r < 9; r++) if (r != p.Row) yield return new Position(r, p.Col);
                int br = (p.Row / 3) * 3, bc = (p.Col / 3) * 3;
                for (int r = br; r < br + 3; r++)
                    for (int c = bc; c < bc + 3; c++)
                        if (r != p.Row || c != p.Col)
                            yield return new Position(r, c);
            }
        }
    }
}
