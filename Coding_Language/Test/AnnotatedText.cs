using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        /// <summary>
        /// Parses the text with the requirement of annotated text
        /// If text is not annotated properly, it creates an exception to say
        /// </summary>
        /// <param name="text"></ text to parse>
        /// <returns></ instance of annotated text>
        /// <exception cref="ArgumentException"></ invalid amount of [] entered>
        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;

            foreach (var c in text)
            {
                if (c == '[')
                {
                    startStack.Push(position);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                        throw new ArgumentException("Too many ']' in text", nameof(text));

                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position++;
                    textBuilder.Append(c);
                }
            }

            if (startStack.Count != 0)
                throw new ArgumentException("Missing ']' in text", nameof(text));

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        /// <summary>
        /// Unidents the line by running the unindent lines method
        /// </summary>
        /// <param name="text"></ input from user>
        /// <returns></ string of text where there is indented lines>
        private static string Unindent(string text)
        {
            var lines = UnindentLines(text);
            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Method to remove the indent of lines
        /// Makes it easier to use
        /// </summary>
        /// <param name="text"></ text to remove indent>
        /// <returns></ array of the lines, where there is no indent>
        public static string[] UnindentLines(string text)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var minIndentation = int.MaxValue;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                var indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                {
                    continue;
                }

                lines[i] = lines[i].Substring(minIndentation);
            }

            while (lines.Count > 0 && lines[0].Length == 0)
            {
                lines.RemoveAt(0);
            }

            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines.ToArray();
        }
    }
}
