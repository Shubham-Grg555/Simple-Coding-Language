using static Coding_Language.Syntax.Token;
namespace Coding_Language.Syntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(Token whileKeyWord, ExpressionSyntax condition, StatementSyntax body)
        {
            WhileKeyWord = whileKeyWord;
            Condition = condition;
            Body = body;
        }

        public override TokenType Type => TokenType.WhileStatement;
        public Token WhileKeyWord { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }
    }
}