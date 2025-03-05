namespace Coding_Language.Binding
{
    internal sealed class BoundFloatExpression : BoundExpression
    {

        public BoundFloatExpression(object value)
        {
            Value = value;
        }

        public object Value { get; }
        public override Type ValueType => Value.GetType();

        public override BoundNodeType Type => BoundNodeType.FloatExpression;
    }
}