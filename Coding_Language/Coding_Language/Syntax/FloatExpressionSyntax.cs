using static Coding_Language.Syntax.Token;
namespace Coding_Language.Syntax
{
    public sealed class FloatExpressionSyntax : ExpressionSyntax
    {

        public FloatExpressionSyntax(Token floatToken)
            : this(floatToken, floatToken.Value)
        {
        }

        public FloatExpressionSyntax(Token floatToken, object value)
        {
            FloatToken = floatToken;
            Value = value;
        }

        public Token FloatToken { get; }
        public object Value { get; }

        public override TokenType Type => TokenType.FloatExpression;
    }
}