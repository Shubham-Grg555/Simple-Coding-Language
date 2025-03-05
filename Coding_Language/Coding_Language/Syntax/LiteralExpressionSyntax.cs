using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    // instance of the expressions (actual expression, number expression)
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        // overload created so we don't need to parse in the value as well (used for numbers)
        public LiteralExpressionSyntax(Token literalToken)
            : this(literalToken, literalToken.Value)
        {
        }

        public LiteralExpressionSyntax(Token literalToken, object value)  // syntax token that represents a number
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override TokenType Type => TokenType.LiteralExpression;
        public Token LiteralToken { get; }
        public object Value { get; }
    }

}
