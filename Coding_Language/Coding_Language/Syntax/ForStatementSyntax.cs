using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(Token keyWord, Token identifier, Token equalsToken, ExpressionSyntax lowerBound,
            Token toKeyword, ExpressionSyntax upperBound, StatementSyntax body)
        {
            KeyWord = keyWord;
            Identifier = identifier;
            EqualsToken = equalsToken;
            LowerBound = lowerBound;
            ToKeyword = toKeyword;
            UpperBound = upperBound;
            Body = body;
        }

        public override TokenType Type => TokenType.ForStatement;
        public Token KeyWord { get; }
        public Token Identifier { get; }
        public Token EqualsToken { get; }
        public ExpressionSyntax LowerBound { get; }
        public Token ToKeyword { get; }
        public ExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }
    }
}
