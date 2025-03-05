using System.Collections.Immutable;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(Token leftBrace, ImmutableArray<StatementSyntax> statements, Token rightBrace)
        {
            LeftBracket = leftBrace;
            Statements = statements;
            RightBracket = rightBrace;
        }

        public Token LeftBracket { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public Token RightBracket { get; }

        public override TokenType Type => TokenType.BlockStatement;
    }
}
