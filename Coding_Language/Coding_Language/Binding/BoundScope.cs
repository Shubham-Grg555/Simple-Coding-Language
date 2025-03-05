using Coding_Language.Syntax;
using System.Collections.Immutable;

namespace Coding_Language.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }

        public BoundScope Parent { get; }

        /// <summary>
        /// Tries to declare a variable and returns a bool depending on the result
        /// Error created, if the variable cannot be declared as it returns false
        /// Causes an error in the class that class it
        /// </summary>
        /// <param name="variable"></ variable that is trying to be declared>
        /// <returns></ true if the variable was declared and false if it was not>
        public bool TryDeclare(VariableSymbol variable)
        {
            if (_variables.ContainsKey(variable.Name))
            {
                return false;
            }
            _variables.Add(variable.Name, variable);
            return true;
        }

        /// <summary>
        /// Method is used to check if the variable already exists
        /// Checks if it can declare the variable, as it has a different precedence to the variable declared
        /// Creates an error if it returns false and the try look up fails and returns false
        /// </summary>
        /// <param name="name"></ name of the variable>
        /// <param name="variable"></ variable to check>
        /// <returns></ true if it was able to find the variable
        /// Returns false if it was not able to find the variable at the specific scope>
        public bool TryLookUp(string name, out VariableSymbol variable)
        {
            if (_variables.TryGetValue(name, out variable))
            {
                return true;
            }
            if (Parent == null)
            {
                return false;
            }
            else
            {
                return Parent.TryLookUp(name, out variable);
            }
        }

        /// <summary>
        /// Method to get all the declared variables
        /// </summary>
        /// <returns></ all the declared variables in an immuatble array>
        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }


}
