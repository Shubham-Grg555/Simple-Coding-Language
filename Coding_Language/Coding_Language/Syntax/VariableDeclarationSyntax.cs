using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(Token keyWord, Token identifier, Token equalsToken, ExpressionSyntax initaliser)
        {
            KeyWord = keyWord;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initialiser = initaliser;
        }


        public override TokenType Type => TokenType.VariableDeclaration;

        public Token KeyWord { get; }
        public Token Identifier { get; }
        public Token EqualsToken { get; }
        public ExpressionSyntax Initialiser { get; }
    }
}
