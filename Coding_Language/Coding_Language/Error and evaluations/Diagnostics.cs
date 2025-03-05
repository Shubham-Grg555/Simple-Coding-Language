using Coding_Language.Text;

namespace Coding_Language.Syntax
{
    public class Diagnostics
    {
        public Diagnostics(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }

        public TextSpan Span { get; }
        public string Message { get; }
        public override string ToString() => Message;
    }
}
