using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Domain
{
    internal class Board
    {
        private readonly Cell[,] _cells = new Cell[9, 9];

        public Board()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    _cells[r, c] = new Cell();
        }

        public static Board FromRows(string[] rows, bool markFixed = true)
        {
            if (rows is null || rows.Length != 9) throw new ArgumentException("Se requieren 9 filas.");
            var b = new Board();
            for (int r = 0; r < 9; r++)
            {
                if (rows[r].Length != 9) throw new ArgumentException($"Fila {r} debe tener 9 caracteres.");
                for (int c = 0; c < 9; c++)
                {
                    int ch = rows[r][c];
                    if (ch < '0' || ch > '9') throw new ArgumentException($"Valor inválido en ({r},{c}).");
                    int v = ch - '0';
                    b._cells[r, c].SetValue(v, asFixed: markFixed && v != 0);
                }
            }
            return b;
        }

        public Cell this[int row, int col] => _cells[row, col];
        public Cell this[Position p] => _cells[p.Row, p.Col];

        public void SetValue(Position p, int value) => _cells[p.Row, p.Col].SetValue(value);

        public IEnumerable<Position> AllPositions()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    yield return new Position(r, c);
        }

        public Board Clone()
        {
            var copy = new Board();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    copy._cells[r, c] = _cells[r, c].Clone();
            return copy;
        }

        public string[] Snapshot()
        {
            var rows = new string[9];
            for (int r = 0; r < 9; r++)
            {
                var sb = new StringBuilder(9);
                for (int c = 0; c < 9; c++) sb.Append(_cells[r, c].Value);
                rows[r] = sb.ToString();
            }
            return rows;
        }
    }
}
