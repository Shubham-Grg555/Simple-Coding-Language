using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coding_Language.Binding;

namespace Coding_Language.Syntax
{
    public class Compilation
    {
        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        private Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }
        private BoundGlobalScope _globalScope;
        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    // only allows for the variable to be created once if it has not been created before
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        /// <summary>
        /// Used to evaluate the input and see if there are errors
        /// </summary>
        /// <returns></ evaluation result of erros if any erros are found>
        /// <returns></ evaluation result if there are no result>
        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {

            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }
            var evaluator = new Evaluator(GlobalScope.Statement, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostics>.Empty, value);
        }
    }
}
