using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }
        public ExpressionSyntax Expression { get; }

        public override TokenType Type => TokenType.ExpressionStatement;
    }
}
