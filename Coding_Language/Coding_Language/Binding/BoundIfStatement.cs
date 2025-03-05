namespace Coding_Language.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement thenStatement, BoundStatement elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public override BoundNodeType Type => BoundNodeType.IfStatement;
        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement ElseStatement { get; }
    }
}
