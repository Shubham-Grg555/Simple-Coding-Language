using System.Collections.Immutable;

namespace Coding_Language.Binding
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public override BoundNodeType Type => BoundNodeType.BlockStatement;
        public ImmutableArray<BoundStatement> Statements { get; }
    }
}
