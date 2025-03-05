using Coding_Language.Binding;
using Coding_Language.Text;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coding_Language.Syntax
{
    internal static class Program
    {
        private static void Main()
        {
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            bool isBlank;
            bool showTree = false;
            Compilation previous = null;

            while (true)
            {
                // makes the arrow a different colour
                // so user knows it is for input
                // also made it print a symbol, so they know they can input the code
                Console.ForegroundColor = ConsoleColor.Green;
                if (textBuilder.Length == 0)
                {
                    Console.Write("» ");
                }
                else
                {
                    Console.Write("・ ");
                }

                Console.ResetColor();

                var input = Console.ReadLine();
                isBlank = string.IsNullOrEmpty(input);
                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        return;
                    }
                    else if (input == "#ShowTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                        continue;
                    }
                    else if (input == "#Clear")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#Reset")
                    {
                        previous = null;
                        continue;
                    }
                    else if (input == "#Exit")
                    {
                        Environment.Exit(0);
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                {
                    continue;
                }
                
                var compilation = previous == null 
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);
                var result = compilation.Evaluate(variables);

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                if (result.Diagnostics.Any())
                {
                    foreach (var diagnositic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnositic.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnositic.Span.Start - line.Start + 1;
                        Console.WriteLine(); // for better formating

                        // diganostic error is a different colour
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"({lineNumber}), ({character}): ");
                        Console.WriteLine(diagnositic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnositic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnositic.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnositic.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        // indent then output to make it more clear, also .Write to output in the same line
                        Console.Write("    ");
                        Console.Write(prefix);

                        // writing the invalid part / erorr in red, also .Write to output in the same line
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(error);
                        Console.ResetColor();

                        // .Write to output in the same line
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                    Console.WriteLine();    // formating
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    previous = compilation;
                }
                textBuilder.Clear();
            }
        }

    }
}