using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class MinMethodSyntax : ExpressionSyntax
    {

        public MinMethodSyntax(Token keyWord, List<Token> numbers)
        {
            KeyWord = keyWord;
            Numbers = numbers;
        }

        public override TokenType Type => TokenType.MinMethod;
        public Token KeyWord { get; }
        public List<Token> Numbers { get; }
    }
}
