using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, Token operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override TokenType Type => TokenType.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public Token OperatorToken { get; }
        public ExpressionSyntax Right { get; }
    }

}
