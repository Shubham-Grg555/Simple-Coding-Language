namespace Coding_Language.Syntax
{
    // base expression class for the different types of expressions
    // so that all classes contain a token type and text span property and a get child method
    // also made this class, as an expression type class e.g Literal expression
    // that inherits from expression syntax, makes more sense than inheriting from syntax node
    public abstract class ExpressionSyntax : SyntaxNode
    {

    }
}
