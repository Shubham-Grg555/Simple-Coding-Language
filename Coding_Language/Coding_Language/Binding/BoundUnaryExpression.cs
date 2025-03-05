namespace Coding_Language.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public override Type ValueType => Op.ResultType;
        public override BoundNodeType Type => BoundNodeType.UnaryExpression;
        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
    }
}
