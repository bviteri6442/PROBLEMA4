using Sudoku.ConsoleApp.Domain;


namespace Sudoku.ConsoleApp.Application.Commands;

public sealed class PlaceNumberCommand : ICommand
{
        private readonly Board _board;
        private readonly Position _pos;
        private readonly int _newValue;
        private int _oldValue = 0;
        private readonly bool _wasFixed;

        public PlaceNumberCommand(Board board, Position pos, int newValue)
        {
            _board = board;
            _pos = pos;
            _newValue = newValue;
        }

        public bool Execute()
        {
            var cell = _board[_pos];
            if (cell.IsFixed) return false;
            _oldValue = cell.Value;
            _board.SetValue(_pos, _newValue);
            return true;
        }

        public void Undo() => _board.SetValue(_pos, _oldValue);

        public string Describe() => $"set {_pos.Row + 1} {_pos.Col + 1} -> {_newValue}";
}

