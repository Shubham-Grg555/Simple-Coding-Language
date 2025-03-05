namespace Coding_Language.Binding
{
    // base class that all statements inherit from, so they contain the property of a bound node type
    // Since it inherits from bound node
    // class made, as statements type classes inheirting from bound statement makes more sense and has better readability
    // Then statement type classes inheriting from bound node
    internal abstract class BoundStatement : BoundNode
    {

    }
}
