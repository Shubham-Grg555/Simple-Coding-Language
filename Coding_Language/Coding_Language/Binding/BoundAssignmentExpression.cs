using Coding_Language.Syntax;

namespace Coding_Language.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }

        public override Type ValueType => Expression.ValueType;

        public override BoundNodeType Type => BoundNodeType.AssignmentExpression;
    }
}
