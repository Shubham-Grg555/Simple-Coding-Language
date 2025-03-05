using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding_Language.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthAndLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthAndLineBreak = lengthAndLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
        public int LengthAndLineBreak { get; }
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanAndLineBreak => new TextSpan(Start, LengthAndLineBreak);
        public override string ToString() => Text.ToString(Span);
    }
}
