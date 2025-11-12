

namespace Sudoku.ConsoleApp.Domain;

    public class Cell
    {
        public int Value { get; private set; } // 0..9 (0 = vacío)
        public bool IsFixed { get; private set; }
        public HashSet<int> Domain { get; }

        public Cell(int value = 0, bool isFixed = false)
        {
            if (value is < 0 or > 9) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
            IsFixed = isFixed;
            Domain = value == 0
                ? new HashSet<int>(Enumerable.Range(1, 9))
                : new HashSet<int> { value };
        }

        public void SetValue(int value, bool asFixed = false)
        {
            if (value is < 0 or > 9) throw new ArgumentOutOfRangeException(nameof(value));
            if (IsFixed && value != Value) throw new InvalidOperationException("No se puede modificar una celda fija.");
            Value = value;
            IsFixed = IsFixed || asFixed;
            Domain.Clear();
            if (value == 0) foreach (var v in Enumerable.Range(1, 9)) Domain.Add(v);
            else Domain.Add(value);
        }

        public Cell Clone()
        {
            var clone = new Cell(Value, IsFixed);
            clone.Domain.Clear();
            foreach (var v in Domain) clone.Domain.Add(v);
            return clone;
        }
    }

