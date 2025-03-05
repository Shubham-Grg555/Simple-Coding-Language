using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class AverageMethodSyntax : ExpressionSyntax
    {

        public AverageMethodSyntax(Token keyWord, List<Token> numbers)
        {
            KeyWord = keyWord;
            Numbers = numbers;
        }

        public override TokenType Type => TokenType.AvgMethod;
        public Token KeyWord { get; }
        public List<Token> Numbers { get; }
    }
}
