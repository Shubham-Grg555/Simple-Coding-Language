using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding_Language.Text
{
    public sealed class SourceText
    {

        private readonly string _text;
        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }
        public ImmutableArray<TextLine> Lines { get; }
        public char this[int index] => _text[index];
        public int Length => _text.Length;

        /// <summary>
        /// Binary search done to get the line index, using the position
        /// </summary>
        /// <param name="position"></ position of character>
        /// <returns></ correct line index>
        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;

            while (lower <= upper)
            {
                var index = lower + (upper - lower) / 2;
                var start = Lines[index].Start;

                if (position == start)
                {
                    return index;
                }
                else if (start > position)
                {
                    upper = index - 1;
                }
                else
                {
                    lower = index + 1;
                }
            }

            return lower - 1;
        }

        /// <summary>
        /// Used to parse the source text
        /// </summary>
        /// <param name="sourceText"></ the source text to parse>
        /// <param name="text"></ string version of the text>
        /// <returns></ result of parsing as an immuatble array>
        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var position = 0;
            var lineStart = 0;
            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, position, lineStart, lineBreakWidth);

                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
            {
                AddLine(result, sourceText, position, lineStart, 0);
            }

            return result.ToImmutable();
        }

        /// <summary>
        /// Method to add lines into the text file
        /// </summary>
        /// <param name="result"></ to add lines>
        /// <param name="sourceText"></ source text to add to>
        /// <param name="position"></ position of a character>
        /// <param name="lineStart"></ start of a line>
        /// <param name="lineBreakWidth"></ width of line break>
        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            var lineLengthAndLineBreak = lineLength + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthAndLineBreak);
            result.Add(line);
        }

        /// <summary>
        /// Used to get the line break width
        /// </summary>
        /// <param name="text"></ string version of the text>
        /// <param name="position"></ position>
        /// <returns></ the line break width number>
        private static int GetLineBreakWidth(string text, int position)
        {
            var character = text[position];
            var lookAhead = position + 1 >= text.Length ? '\0' : text[position + 1];
            if (character == '\r' && lookAhead == '\n')
            {
                return 2;
            }
            else if (character == '\r' || character == '\n')
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }

        // overrided methods, so that it returns the value I want
        public override string ToString() => _text;
        public string ToString(int start, int length) => _text.Substring(start, length);
        public string ToString(TextSpan span) => ToString(span.Start, span.Length);
    }
}
