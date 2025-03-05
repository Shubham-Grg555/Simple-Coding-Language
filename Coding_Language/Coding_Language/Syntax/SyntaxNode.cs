using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Coding_Language.Text;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    // base syntax node for expressions and statements
    public abstract class SyntaxNode
    {
        // used to get the token type
        public abstract TokenType Type { get; }

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        // Method used to get the child nodes and allows us to traverse the tree
        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);
                    if (child != null)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    foreach (var child in children)
                    {
                        if (child != null)
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// So that I can print using the print tree method in the other class that handles input and output
        /// </summary>
        /// <param name="writer"></ input to handle>
        public void WriteTo(TextWriter writer)
        {
            PrintTree(writer, this);
        }

        /// <summary>
        /// Prints the tree
        /// </summary>
        /// <param name="writer"></ text writer>
        /// <param name="node"></ the node that will be printed>
        /// <param name="indent"></ indentation to make the tree more readable>
        /// <param name="isLast"></ used to get correct indentation>
        private static void PrintTree(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            writer.Write(marker);

            if (isToConsole)
            {
                Console.ForegroundColor = node is Token ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
                writer.Write(node.Type);
            }

            if (node is Token t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            if (isToConsole)
            {
                Console.ResetColor();
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";
            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrintTree(writer, child, indent, node == lastChild);
            }
        }

        /// <summary>
        /// Use string writer to get a string version of the ouptut
        /// </summary>
        /// <returns></ string version of writer>
        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
