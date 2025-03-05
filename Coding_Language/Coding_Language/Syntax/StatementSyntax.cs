namespace Coding_Language.Syntax
{
    // inheirts form syntax node, so all classes that inherit from statement syntax
    // need to have a text span, token type properties and a get child method
    // class made as statement syntax type classes e.g if statement
    // will make more sense to inherit from statement syntax compared to syntax node
    public abstract class StatementSyntax : SyntaxNode
    {

    }
}
