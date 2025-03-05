namespace Coding_Language.Binding
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }


        public override BoundNodeType Type => BoundNodeType.ExpressionStatement;

        public BoundExpression Expression { get; }
    }
}
