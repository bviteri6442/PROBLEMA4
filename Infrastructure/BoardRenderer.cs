using Sudoku.ConsoleApp.Domain;


namespace Sudoku.ConsoleApp.Infrastructure;

    public sealed class BoardRenderer
    {
        public void Render(Board b)
        {
            Console.WriteLine("   1 2 3   4 5 6   7 8 9");
            Console.WriteLine(" +-------+-------+-------+");
            for (int r = 0; r < 9; r++)
            {
                Console.Write($"{r + 1}| ");
                for (int c = 0; c < 9; c++)
                {
                    var v = b[r, c].Value;
                    Console.Write(v == 0 ? ". " : $"{v} ");
                    if ((c + 1) % 3 == 0) Console.Write("| ");
                }
                Console.WriteLine();
                if ((r + 1) % 3 == 0) Console.WriteLine(" +-------+-------+-------+");
            }
        }
    }

