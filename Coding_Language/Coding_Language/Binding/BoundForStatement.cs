using Coding_Language.Syntax;

namespace Coding_Language.Binding
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(VariableSymbol variable, BoundExpression lowerBound, BoundExpression upperBound, BoundStatement body)
        {
            Variable = variable;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Body = body;
        }

        public override BoundNodeType Type => BoundNodeType.ForStatement;
        public VariableSymbol Variable { get; }
        public BoundExpression LowerBound { get; }
        public BoundExpression UpperBound { get; }
        public BoundStatement Body { get; }
    }
}
