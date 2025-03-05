using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class MaxMethodSyntax : ExpressionSyntax
    {

        public MaxMethodSyntax(Token keyWord, List<Token> numbers)
        {
            KeyWord = keyWord;
            Numbers = numbers;
        }

        public override TokenType Type => TokenType.MaxMethod;
        public Token KeyWord { get; }
        public List<Token> Numbers { get; }
    }
}
