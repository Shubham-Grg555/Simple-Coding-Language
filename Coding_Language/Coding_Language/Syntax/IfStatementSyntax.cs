using static Coding_Language.Syntax.Token;
namespace Coding_Language.Syntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(Token ifKeyword, ExpressionSyntax condition, StatementSyntax thenStatement, ElseClauseSyntax elseClause)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }

        public override TokenType Type => TokenType.IfStatement;
        public Token IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenStatement { get; }
        public ElseClauseSyntax ElseClause { get; }
    }
}
