using Sudoku.ConsoleApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Infrastructure
{
    public sealed class BoardLoader
    {
        public Board LoadFromFile(string path)
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .Select(l => l.Trim())
                            .Take(9)
                            .ToArray();
            if (lines.Length != 9) throw new InvalidOperationException("El archivo debe tener 9 líneas con 9 dígitos (0..9).");
            return Board.FromRows(lines, markFixed: true);
        }

        public void SaveToFile(Board b, string path)
        {
            File.WriteAllLines(path, b.Snapshot(), Encoding.UTF8);
        }
    }
}
