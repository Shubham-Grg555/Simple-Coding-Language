using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class BracketExpressionSyntax : ExpressionSyntax
    {
        public BracketExpressionSyntax(Token lBracket, ExpressionSyntax expression, Token rBracket)
        {
            LBracket = lBracket;
            Expression = expression;
            RBracket = rBracket;
        }

        public override TokenType Type => TokenType.BracketExpression;
        public Token LBracket { get; }
        public ExpressionSyntax Expression { get; }
        public Token RBracket { get; }
    }

}
