

namespace Sudoku.ConsoleApp.Application.Commands;

public sealed class UndoCommand : ICommand
    {
        private readonly Stack<ICommand> _undo;
        private readonly Stack<ICommand> _redo;

        public UndoCommand(Stack<ICommand> undo, Stack<ICommand> redo)
        {
            _undo = undo; _redo = redo;
        }

        public bool Execute()
        {
            if (_undo.Count == 0) return false;
            var cmd = _undo.Pop();
            cmd.Undo();
            _redo.Push(cmd);
            return true;
        }

        public void Undo() { /* no-op */ }

        public string Describe() => "undo";
    }

