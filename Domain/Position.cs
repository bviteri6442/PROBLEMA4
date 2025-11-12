using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Domain
{
    internal class Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            if (row is < 0 or > 8) throw new ArgumentOutOfRangeException(nameof(row));
            if (col is < 0 or > 8) throw new ArgumentOutOfRangeException(nameof(col));
            Row = row; Col = col;
        }

        public override string ToString() => $"({Row},{Col})";
    }
}
