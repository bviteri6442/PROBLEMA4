

namespace Sudoku.ConsoleApp.Domain.Validation;

    public sealed class ClassicSudokuValidator : IBoardValidator
    {
        public bool IsValid(Board board) => SudokuRules.BoardIsValid(board);
    }

