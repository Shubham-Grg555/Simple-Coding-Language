using Coding_Language.Syntax;
using System.Collections.Immutable;

namespace Coding_Language.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope previous, ImmutableArray<Diagnostics> diagnostics, ImmutableArray<VariableSymbol> variables, BoundStatement statement)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Statement = statement;
        }

        public BoundGlobalScope Previous { get; }
        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundStatement Statement { get; }
    }


}
