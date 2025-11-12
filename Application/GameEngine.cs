using Sudoku.ConsoleApp.Application.Commands;
using Sudoku.ConsoleApp.Domain;
using Sudoku.ConsoleApp.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sudoku.ConsoleApp.Domain.ISudokuSolver;

namespace Sudoku.ConsoleApp.Application
{
    internal class GameEngine
    {
        private readonly BoardRenderer _renderer = new();
        private readonly ClassicSudokuValidator _validator = new();
        private readonly Stack<ICommand> _undo = new();
        private readonly Stack<ICommand> _redo = new();

        private Board _board = new();
        private bool _didactic = false;

        public void Run()
        {
            Console.WriteLine("=== Sudoku (C# .NET Console) ===");
            PrintHelp();
            _renderer.Render(_board);

            while (true)
            {
                Console.Write("\n> ");
                var line = Console.ReadLine();
                if (line is null) continue;
                var args = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (args.Length == 0) continue;
                var cmd = args[0].ToLowerInvariant();

                try
                {
                    switch (cmd)
                    {
                        case "help": PrintHelp(); break;
                        case "print": _renderer.Render(_board); break;
                        case "load":
                            if (args.Length < 2) { Console.WriteLine("Uso: load ruta.txt"); break; }
                            _board = new BoardLoader().LoadFromFile(args[1]);
                            _undo.Clear(); _redo.Clear();
                            Console.WriteLine("Tablero cargado.");
                            _renderer.Render(_board);
                            break;

                        case "save":
                            if (args.Length < 2) { Console.WriteLine("Uso: save ruta.txt"); break; }
                            new BoardLoader().SaveToFile(_board, args[1]);
                            Console.WriteLine("Guardado.");
                            break;

                        case "set":
                            if (args.Length != 4) { Console.WriteLine("Uso: set r c v (1..9, 0=vaciar)"); break; }
                            int r = int.Parse(args[1]) - 1, c = int.Parse(args[2]) - 1, v = int.Parse(args[3]);
                            var place = new PlaceNumberCommand(_board, new Position(r, c), v);
                            if (place.Execute())
                            {
                                _undo.Push(place); _redo.Clear();
                                Console.WriteLine(place.Describe());
                                _renderer.Render(_board);
                            }
                            else Console.WriteLine("No se pudo setear (¿celda fija?).");
                            break;

                        case "clear":
                            if (args.Length != 3) { Console.WriteLine("Uso: clear r c"); break; }
                            r = int.Parse(args[1]) - 1; c = int.Parse(args[2]) - 1;
                            place = new PlaceNumberCommand(_board, new Position(r, c), 0);
                            if (place.Execute()) { _undo.Push(place); _redo.Clear(); _renderer.Render(_board); }
                            else Console.WriteLine("No se pudo limpiar (¿celda fija?).");
                            break;

                        case "undo":
                            if (new UndoCommand(_undo, _redo).Execute()) _renderer.Render(_board);
                            else Console.WriteLine("Nada que deshacer.");
                            break;

                        case "redo":
                            if (_redo.Count == 0) { Console.WriteLine("Nada que rehacer."); break; }
                            var redoCmd = _redo.Pop();
                            if (redoCmd.Execute()) { _undo.Push(redoCmd); _renderer.Render(_board); }
                            break;

                        case "validate":
                            Console.WriteLine(_validator.IsValid(_board) ? "OK" : "Inválido");
                            break;

                        case "didactico":
                            _didactic = !_didactic;
                            Console.WriteLine($"Modo didáctico: {(_didactic ? "ON" : "OFF")}");
                            break;

                        case "hint":
                            var hint = Hints.MrvHint(_board);
                            Console.WriteLine(hint ?? "No hay sugerencias (¿resuelto?)");
                            break;

                        case "solve":
                            SolveCurrent();
                            break;

                        case "exit":
                        case "salir":
                            Console.WriteLine("Hasta luego.");
                            return;

                        default:
                            Console.WriteLine("Comando no reconocido. Escribe 'help'.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void SolveCurrent()
        {
            if (!_validator.IsValid(_board))
            {
                Console.WriteLine("El tablero actual viola reglas básicas.");
                return;
            }

            var solver = new Solvers.BacktrackingSolver();
            solver.OnStep += e =>
            {
                if (!_didactic) return;
                Console.WriteLine($"[{e.Depth}] {e.Kind} {e.Message}");
                if (e.Kind is "assign" or "backtrack" or "prune" or "select")
                {
                    _renderer.Render(_board);
                    Console.Write("continuar (Enter)...");
                    Console.ReadLine();
                }
            };

            var ok = solver.Solve(_board, new SolverOptions { DidacticMode = _didactic, PauseOnStep = _didactic });
            Console.WriteLine(ok ? "¡Solucionado!" : "Sin solución.");
            _renderer.Render(_board);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("""
Comandos:
  load <archivo.txt>        Cargar tablero (9 filas de 9 dígitos, 0 = vacío)
  save <archivo.txt>        Guardar tablero actual
  print                     Imprimir tablero ASCII
  set <r> <c> <v>           Poner valor (1..9) o 0 para limpiar (r/c 1..9)
  clear <r> <c>             Limpia la celda (equivale a set r c 0)
  undo | redo               Deshacer / Rehacer
  validate                  Verificar reglas básicas
  didactico                 Toggle modo didáctico (pausas, pasos MRV/LCV/FC)
  hint                      Sugerir celda por MRV y valor viable con FC
  solve                     Resolver automáticamente (DFS + MRV + LCV + FC)
  help                      Mostrar ayuda
  exit                      Salir
""");
        }
    }

    /// <summary>Ayudas simples para el jugador.</summary>
    internal static class Hints
    {
        public static string? MrvHint(Board b)
        {
            Position? best = null;
            int bestCount = int.MaxValue;
            var cand = Solvers.Consistency.ForwardChecking.GetCandidates(b);

            foreach (var kv in cand)
            {
                if (b[kv.Key].Value != 0) continue;
                int count = kv.Value.Count;
                if (count < bestCount) { bestCount = count; best = kv.Key; }
            }
            if (best is null) return null;
            var values = string.Join(",", cand[best.Value]);
            return $"Sugerencia MRV: celda ({best.Value.Row + 1},{best.Value.Col + 1}) con {{ {values} }}";
        }
    }
}
