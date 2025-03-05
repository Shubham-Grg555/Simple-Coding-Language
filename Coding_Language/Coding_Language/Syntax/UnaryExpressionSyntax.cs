using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(Token operatorToken, ExpressionSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        // used to quickly switch to the correct token, which is more efficient then checking for the actual instant
        public override TokenType Type => TokenType.UnaryExpression;
        public Token OperatorToken { get; }
        public ExpressionSyntax Operand { get; }
    }

}
