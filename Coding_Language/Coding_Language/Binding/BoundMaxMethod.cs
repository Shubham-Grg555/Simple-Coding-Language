﻿using Coding_Language.Syntax;
namespace Coding_Language.Binding
{
    internal sealed class BoundMaxMethod : BoundExpression
    {
        public BoundMaxMethod(List<Token> numbers)
        {
            Numbers = numbers;
        }

        public override BoundNodeType Type => BoundNodeType.MaxMethod;
        public List<Token> Numbers { get; }

        // not needed for the method, but needed so that the code runs, as this is an abstract method
        // means it has to be implemented in each class that inherits from bound expression
        public override Type ValueType => throw new NotImplementedException();
    }
}
