using Coding_Language.Syntax;

namespace Coding_Language.Binding
{
    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initaliser)
        {
            Variable = variable;
            Initaliser = initaliser;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression Initaliser { get; }

        public override BoundNodeType Type => BoundNodeType.VariableDeclaration;
    }
}
