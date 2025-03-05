namespace Coding_Language.Binding
{
    // Creates necessary parameters and makes them a public value to make a binary expression
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override Type ValueType => Op.ResultType;
        public override BoundNodeType Type => BoundNodeType.BinaryExpression;
        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }
    }
}
