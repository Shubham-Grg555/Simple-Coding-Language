using System.Collections.Immutable;

namespace Coding_Language.Syntax
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostics> diagnostics, object value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public object Value { get; }
    }
}
