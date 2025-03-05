using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(StatementSyntax statement, Token endOfFile)
        {
            Statement = statement;
            EndOfFile = endOfFile;
        }

        public StatementSyntax Statement { get; }
        public Token EndOfFile { get; }

        public override TokenType Type => TokenType.CompilationUnit;
    }
}
