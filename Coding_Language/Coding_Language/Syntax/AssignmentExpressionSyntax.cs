using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(Token identifierToken, Token equalsToken, ExpressionSyntax expressionSyntax)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            ExpressionSyntax = expressionSyntax;
        }

        public override TokenType Type => TokenType.AssignmentExpression;

        public Token IdentifierToken { get; }
        public Token EqualsToken { get; }
        public ExpressionSyntax ExpressionSyntax { get; }
    }
}
