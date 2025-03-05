namespace Coding_Language.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement body)
        {
            Condition = condition;
            Body = body;
        }

        public override BoundNodeType Type => BoundNodeType.WhileStatement;
        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
    }
}
