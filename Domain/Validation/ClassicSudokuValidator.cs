using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Domain.Validation
{
    internal class ClassicSudokuValidator : IBoardValidator
    {
        public bool IsValid(Board board) => SudokuRules.BoardIsValid(board);
    }
}
