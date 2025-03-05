using Coding_Language.Syntax;

namespace Coding_Language.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public VariableSymbol Variable { get; }
        public override BoundNodeType Type => BoundNodeType.VariableExpression;
        public override Type ValueType => Variable.Type;
    }


}
