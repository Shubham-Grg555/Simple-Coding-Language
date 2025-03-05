namespace Coding_Language.Binding
{
    // base class where every expression inherits from, so they all contain a property of type Type
    // Class inherits from bound node, so all classes that inherit form this contains
    // So all classes that inherit from this class, contains a bound node type property
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type ValueType { get; }
    }


}
