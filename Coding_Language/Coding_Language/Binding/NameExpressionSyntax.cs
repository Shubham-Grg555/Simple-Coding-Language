using Coding_Language.Syntax;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Binding
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(Token identifierToken)
        {
            IdentifierToken = identifierToken;
        }
        public override TokenType Type => TokenType.NameExpression;

        public Token IdentifierToken { get; }
    }
}
