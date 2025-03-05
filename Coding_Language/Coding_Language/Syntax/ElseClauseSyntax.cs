using static Coding_Language.Syntax.Token;
namespace Coding_Language.Syntax
{
    public sealed class ElseClauseSyntax : SyntaxNode
    {
        public ElseClauseSyntax(Token elseKeyWord, StatementSyntax elseStatement)
        {
            ElseKeyWord = elseKeyWord;
            ElseStatement = elseStatement;
        }

        public override TokenType Type => TokenType.ElseClause;
        public Token ElseKeyWord { get; }
        public StatementSyntax ElseStatement { get; }
    }
}
