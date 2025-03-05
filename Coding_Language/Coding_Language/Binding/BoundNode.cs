namespace Coding_Language.Binding
{
    // base class where every bound class inherits from bound node
    // done by either inheriting a class that inherits from bound node
    // Done so every class contains a bound node type
    internal abstract class BoundNode
    {
        public abstract BoundNodeType Type { get;  }
    }
}
