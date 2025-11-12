using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ConsoleApp.Domain.Validation
{
    internal interface IBoardValidator
    {
        bool IsValid(Board board);
    }
}
