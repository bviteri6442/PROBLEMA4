

namespace Sudoku.ConsoleApp.Application.Commands;
public interface ICommand
{
    bool Execute();
    void Undo();
    string Describe();
}

