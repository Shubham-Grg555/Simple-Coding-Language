using Coding_Language.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Binding
{
    internal enum BoundNodeType
    {
        // Expressions
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        VariableExpression,
        AssignmentExpression,
        FloatExpression,

        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,

        // Built in method
        AvgMethod,
        MinMethod,
        MaxMethod,
    }

    internal sealed class Binder
    {

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;
        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        /// <summary>
        /// Used to bind a global scope onto a variable and create errors if there was a problem
        /// </summary>
        /// <param name="previous"></ previous bound gobal scope of the variable>
        /// <param name="syntax"></ variable to bind>
        /// <returns></ a new bound global scope for the variable>
        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var expression = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostics, variables, expression);
        }

        /// <summary>
        /// Used to create the parent scope using previous parameter
        /// </summary>
        /// <param name="previous"></ previous submission of the variable>
        /// <returns></ correct reverse order of previous by using a stack>
        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var variable in previous.Variables)
                {
                    scope.TryDeclare(variable);
                }
                parent = scope;
            }

            return parent;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        /// <summary>
        /// method used to create the correct expression by comparing the token type
        /// </summary>
        /// <param name="syntax"></ the expression syntax to bind>
        /// <returns></ method that creates binded expression type>
        /// <exception cref="Exception"></ token type was not any of the expression coded, it says unexpected syntax>
        private BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Type)
            {
                case TokenType.BracketExpression:
                    return BindBracketExpression((BracketExpressionSyntax)syntax);
                case TokenType.FloatExpression:
                    return BindFloatExpression((FloatExpressionSyntax)syntax);
                case TokenType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case TokenType.AvgMethod:
                    return BindAverageMethod((AverageMethodSyntax)syntax);
                case TokenType.MinMethod:
                    return BindMinMethod((MinMethodSyntax)syntax);
                case TokenType.MaxMethod:
                    return BindMaxMethod((MaxMethodSyntax)syntax);
                case TokenType.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case TokenType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case TokenType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case TokenType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);

                default:
                    throw new Exception("Unexpected expression syntax: " + syntax.Type);
            }
        }

        /// <summary>
        /// Method to bind the condition needed for statements e.g if statements
        /// Creates error if the type of bound syntax is not the desired type
        /// </summary>
        /// <param name="syntax"></ syntax to bind>
        /// <param name="targetType"></ the type the bound syntax should be>
        /// <returns></ the bound syntax>
        private BoundExpression BindCondition(ExpressionSyntax syntax, Type targetType)
        {
            var result = BindExpression(syntax);
            if (result.ValueType != targetType)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, result.ValueType, targetType);
            }
            return result;
        }

        /// <summary>
        /// Method to bind the bracket expression
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ syntax to bind>
        /// <returns></ bind expression method run again, so it "binds" both brackets
        /// and makes sure it has the highest precedence>
        private BoundExpression BindBracketExpression(BracketExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        /// <summary>
        /// Method to bind float expression
        /// Done by creating the necessary parameters for the class constructor
        /// In this case, it is just the value of the float
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ float expression to bind>
        /// <returns></ a new instance of the bound float expression, with the value of the current float>
        private BoundExpression BindFloatExpression(FloatExpressionSyntax syntax)
        {
            var value = syntax.Value ?? -1; // -1 so it is easier to test if there is a float error
            return new BoundFloatExpression(value);
        }

        /// <summary>
        /// method to bind the literal expression
        /// Done by creating the necessary parameters for the class constructor
        /// In this case, it is just the value of the integer
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ literal expression to be evaluated>
        /// <returns></ the instance of the bound literal expression, with the value of the current integer>
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        /// <summary>
        /// Method to bind the average method
        /// Done by creating the necessary parameter for the class constructor 
        /// In this case, it is just the list of tokens in the method, which contains the value of the numbers
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ average method syntax to be evaluated>
        /// <returns></ instance of the bound average method, using the list of numbers made>
        private BoundExpression BindAverageMethod(AverageMethodSyntax syntax)
        {
            List<Token> numbers = new List<Token>();
            foreach (var number in syntax.Numbers)
            {
                numbers.Add(number);
            }
            return new BoundAverageMethod(numbers);
        }

        /// <summary>
        /// Method to bind the minimum method
        /// Done by creating the necessary parameteres for the class constructor
        /// In this case, it is just the list of tokens in the method, which contains the value of the numbers
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ minimum method syntax to be evaluated>
        /// <returns></ instance of the bound minimum method, using the list of numbers>
        private BoundExpression BindMinMethod(MinMethodSyntax syntax)
        {
            List<Token> numbers = new List<Token>();
            foreach (var number in syntax.Numbers)
            {
                numbers.Add(number);
            }
            return new BoundMinMethod(numbers);
        }

        /// <summary>
        /// Method to bind the maximum method
        /// Done by creating the necessary parameteres for the class constructor
        /// In this case, it is just the list of tokens in the method, which contains the value of the numbers
        /// No error check as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ maximum method syntax to be evaluated>
        /// <returns></instance of the bound maximum method, using the list of numbers>
        private BoundExpression BindMaxMethod(MaxMethodSyntax syntax)
        {
            List<Token> numbers = new List<Token>();
            foreach (var number in syntax.Numbers)
            {
                numbers.Add(number);
            }
            return new BoundMaxMethod(numbers);
        }

        /// <summary>
        /// Methdo to bind the name expression
        /// Done by creating the parameters for the class construct
        /// In this case, the actual variable
        /// Error check if there is no variable name, or undefined variable name
        /// </summary>
        /// <param name="syntax"></ variable to be binded>
        /// <returns></ an instance of the bound variable expression class, using the variable, variable>
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if (string.IsNullOrEmpty(name))
            {
                return new BoundLiteralExpression(0);
            }

            if (!_scope.TryLookUp(name, out var variable))
            {
                _diagnostics.ReportUndefinedVariable(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }
            return new BoundVariableExpression(variable);
        }

        /// <summary>
        /// Used to bind the assignment expression
        /// Done by creating the necessary parameters for the class constructor
        /// In this case, the name of the variable being assigned to and the value
        /// Error checks if the variable has been declared, if it is read only, or cannot convert the value
        /// </summary>
        /// <param name="syntax"></ variable to assign>
        /// <returns></ returns a bound assignment expression, using the variable and boundExpression
        /// Which is the actual variable and its value>
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.ExpressionSyntax);

            if (!_scope.TryLookUp(name, out var variable))
            {
                _diagnostics.ReportUndefinedVariable(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.IsReadOnly)
            {
                _diagnostics.ReportCannotAssign(syntax.IdentifierToken.Span, name);
            }

            if (boundExpression.ValueType != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.ExpressionSyntax.Span, boundExpression.ValueType, variable.Type);
                return boundExpression;
            }
            return new BoundAssignmentExpression(variable, boundExpression);
        }

        /// <summary>
        /// method to bind the unary expression
        /// Done by creating the necessary parameters for the class instance
        /// In this case being the number and unary operator
        /// Error checks if the unary operator being used is defined or not
        /// </summary>
        /// <param name="syntax"></ unary expression to be evaluated>
        /// <returns></ the instance of the bound unary expression, using boundOperator and boundOperand
        /// Which is the unary operator and the value>
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            // creating necessary parameters
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Type, boundOperand.ValueType);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.ValueType);
                return boundOperand;    // returns something as it has to return something
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        /// <summary>
        /// method to bind the binary expression
        /// Done by creating the necessary parameters for the class instance
        /// In this case, being the value on left, the operator and the value on the right e.g left being 2, for 2 + 5
        /// Error checks if the binary operator being used is defined or not
        /// </summary>
        /// <param name="syntax"></ binary expression to be evaluated>
        /// <returns></ the instance of the bound binary expression, using boundLeft, boundOperator and boundRight
        /// Which is the value on the left of the binary operator, binary operaor and value on the right>
        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            // creating necessary parameters
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Type, boundLeft.ValueType, boundRight.ValueType);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.ValueType, boundRight.ValueType);
                return boundLeft; // return something as it has to return something
            }
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        /// <summary>
        /// Method to bind the correct statement by checking the statement token type
        /// </summary>
        /// <param name="syntax"></ statement syntax to bind>
        /// <returns></returns>
        /// <exception cref="Exception"></ an unexpected statement syntax was entered>
        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Type)
            {
                case TokenType.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case TokenType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case TokenType.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case TokenType.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case TokenType.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case TokenType.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                default:
                    throw new Exception("Unexpected statement syntax: " + syntax.Type);
            }
        }

        /// <summary>
        /// Method to bind the block statement
        /// Done by creating the statement variable to bind e.g { block statement }
        /// No error checks, as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ Block statement to be binded>
        /// <returns></ instance of the bound bock statment class, using statements
        /// Which is made into an immuatble array, so that it cannot be changed and fit the conditions of the constructor>
        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;
            return new BoundBlockStatement(statements.ToImmutable());
        }

        /// <summary>
        /// Method to bind an expression statement
        /// Done by creating the expression to bind, e.g 5
        /// No error check, as it is just returning an input e.g 5
        /// </summary>
        /// <param name="syntax"></ expression syntax to bind>
        /// <returns></ new instance of the bound expression statement, using the expression variable
        /// Which is just the value of what is inside the expression e.g 5>
        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        /// <summary>
        /// Method to bind the variable declaration
        /// Done by creating the necessary parameters for the class instance
        /// In this case, being the name of the variable, checking if it is read only, initaliser (assigning) and an instance of the variable symbol
        /// Error checks if the variable has already been declared before
        /// </summary>
        /// <param name="syntax"></ variable to be binded>
        /// <returns></ the instance of the bound variable declaration, using variable and initialiser>
        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var isReadOnly = syntax.KeyWord.Type == TokenType.LetKeyWord;
            var initialiser = BindExpression(syntax.Initialiser);
            var variable = new VariableSymbol(name, isReadOnly, initialiser.ValueType);

            if (!_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
            }
            return new BoundVariableDeclaration(variable, initialiser);
        }

        /// <summary>
        /// Method to bind the if statement
        /// Done by creating the necessary parameters for the class instance
        /// In this case, being the if condition, then statement and else statement
        /// No error checks, as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ if statement to bind>
        /// <returns></ the instance of the bound if statement, using condition, thenStatement and elseStatement>
        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindCondition(syntax.Condition, typeof(bool));
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        /// <summary>
        /// Method to bind the while statement
        /// Done by creating the necessary parameters for the class instance
        /// In this case, being the while condition and code in the while loop
        /// No error checks, as it is done in the parser
        /// </summary>
        /// <param name="syntax"></ while statement to bind>
        /// <returns></ the instance of the bound while statement, using condition, and body>
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindCondition(syntax.Condition, typeof(bool));
            var body = BindStatement(syntax.Body);
            return new BoundWhileStatement(condition, body);
        }

        /// <summary>
        /// Method to bind the for statement
        /// Done by creating the necessary parameters for the class instance
        /// In this case, being the variable incremented, what the variable starts as,
        /// What the variable should end on and the code in the for loop
        /// Error checks the variable by trying to declare it
        /// Error checks Whether the lower and upper bound are the type it was expecting (valid type check)
        /// </summary>
        /// <param name="syntax"></ for statement to bind>
        /// <returns></ the instance of the bound for statement, using variable, lower bound, upper bound and body>
        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var lowerBound = BindCondition(syntax.LowerBound, typeof(int));
            var upperBound = BindCondition(syntax.UpperBound, typeof(int));

            _scope = new BoundScope(_scope);

            var name = syntax.Identifier.Text;
            var variable = new VariableSymbol(name, true, typeof(int));
            if (!_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
            }

            var body = BindStatement(syntax.Body);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, lowerBound, upperBound, body);
        }
    }

    // unary operator tokens
    internal enum BoundUnaryOperatorType
    {
        Identity,
        Negation,
        LogicalReverse,
    }

    // binary operator tokens
    internal enum BoundBinaryOperatorType
    {
        // math
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Power,

        // logic operators
        LogicalAND,
        LogicalOR,
        TrueEquals,
        NOTEquals,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
    }
}
