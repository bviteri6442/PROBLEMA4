using System;

namespace Sudoku.ConsoleApp.Domain;

    public static class SudokuRules
    {
        public static bool RowIsValid(Board b, int row)
        {
            var seen = new bool[10];
            for (int c = 0; c < 9; c++)
            {
                int v = b[row, c].Value;
                if (v == 0) continue;
                if (seen[v]) return false;
                seen[v] = true;
            }
            return true;
        }

        public static bool ColIsValid(Board b, int col)
        {
            var seen = new bool[10];
            for (int r = 0; r < 9; r++)
            {
                int v = b[r, col].Value;
                if (v == 0) continue;
                if (seen[v]) return false;
                seen[v] = true;
            }
            return true;
        }

        public static bool BoxIsValid(Board b, int boxRow, int boxCol)
        {
            var seen = new bool[10];
            for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                {
                    int v = b[r, c].Value;
                    if (v == 0) continue;
                    if (seen[v]) return false;
                    seen[v] = true;
                }
            return true;
        }

        public static bool BoardIsValid(Board b)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!RowIsValid(b, i)) return false;
                if (!ColIsValid(b, i)) return false;
            }
            for (int br = 0; br < 3; br++)
                for (int bc = 0; bc < 3; bc++)
                    if (!BoxIsValid(b, br, bc)) return false;

            return true;
        }
    }

