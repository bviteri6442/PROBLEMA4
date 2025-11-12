using Sudoku.ConsoleApp.Domain;


namespace Sudoku.ConsoleApp.Application.Solvers.Consistency;

    public sealed class ForwardChecking : IConstraintPropagator
    {
        public IEnumerable<int> Candidates(Board b, Position p) => CandidatesFor(b, p);

        public bool IsConsistent(Board b, Position p, int v)
        {
            // fila
            for (int c = 0; c < 9; c++) if (c != p.Col && b[p.Row, c].Value == v) return false;
            // columna
            for (int r = 0; r < 9; r++) if (r != p.Row && b[r, p.Col].Value == v) return false;
            // subcuadro
            int br = (p.Row / 3) * 3, bc = (p.Col / 3) * 3;
            for (int r = br; r < br + 3; r++)
                for (int c = bc; c < bc + 3; c++)
                    if ((r != p.Row || c != p.Col) && b[r, c].Value == v) return false;
            return true;
        }

        public object Push(Board b, Position assigned, int value)
        {
            // snapshot de dominios podados para revertir
            var pruned = new List<(Position pos, int val)>();

            foreach (var q in Neighbors(assigned))
            {
                var cell = b[q];
                if (cell.Value != 0) continue;
                if (cell.Domain.Contains(value))
                {
                    cell.Domain.Remove(value);
                    pruned.Add((q, value));
                }
            }
            return pruned;
        }

        public void Pop(Board b, object snapshot)
        {
            var pruned = (List<(Position pos, int val)>)snapshot;
            foreach (var (p, v) in pruned) b[p].Domain.Add(v);
        }

        // ---------- Utilidades compartidas ----------
        public static Dictionary<Position, HashSet<int>> GetCandidates(Board b)
        {
            var map = new Dictionary<Position, HashSet<int>>();
            foreach (var p in b.AllPositions())
                map[p] = new HashSet<int>(CandidatesFor(b, p));
            return map;
        }

        public static HashSet<int> CandidatesFor(Board b, Position p)
        {
            var cell = b[p];
            if (cell.Value != 0) return new HashSet<int> { cell.Value };

            var set = new HashSet<int>(Enumerable.Range(1, 9));

            // fila
            for (int c = 0; c < 9; c++) set.Remove(b[p.Row, c].Value);
            // columna
            for (int r = 0; r < 9; r++) set.Remove(b[r, p.Col].Value);
            // subcuadro
            int br = (p.Row / 3) * 3, bc = (p.Col / 3) * 3;
            for (int r = br; r < br + 3; r++)
                for (int c = bc; c < bc + 3; c++)
                    set.Remove(b[r, c].Value);

            set.Remove(0);
            return set;
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

