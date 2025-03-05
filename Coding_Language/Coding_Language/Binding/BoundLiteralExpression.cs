namespace Coding_Language.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override BoundNodeType Type => BoundNodeType.LiteralExpression;
        public override Type ValueType => Value.GetType();
        public object Value { get; }
    }

}
