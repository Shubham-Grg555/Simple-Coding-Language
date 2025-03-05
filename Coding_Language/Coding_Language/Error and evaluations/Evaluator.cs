using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Coding_Language.Binding;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    internal sealed class Evaluator
    {
        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;
        private object _lastValue;

        /// <summary>
        /// Method runs evaluate statement, as that method runs evaluate expression if it could not find a statement
        /// Else it creates an error saying that there was an unexpected statement or expression
        /// </summary
        /// <returns></ _lastValue to return value of the statement>
        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue;
        }

        /// <summary>
        /// Sees what expression the node is and runs the necessary method
        /// </summary>
        /// <param name="node"></ type of child node>
        /// <returns></ now returns object type data so it can return correct equation or other data types from keywords>
        /// <exception cref="Exception"></ unexpected binary operator is entered, or node that does not match>
        private object EvaluateExpression(BoundExpression node)
        {
            // checks if it is a number expression
            switch (node)
            {
                case BoundLiteralExpression literalExpression:
                    return EvaluateLiteralExpression(literalExpression);
                case BoundFloatExpression floatExpression:
                    return EvaluateFloatExpression(floatExpression);
                case BoundVariableExpression variableExpression:
                    return EvaluateVariableExpression(variableExpression);
                case BoundAssignmentExpression assignmentExpression:
                    return EvaluateAssignmentExpression(assignmentExpression);
                case BoundUnaryExpression unaryExpression:
                    return EvaluateUnaryExpression(unaryExpression);
                case BoundBinaryExpression binaryExpression:
                    return EvaluateBinaryExpression(binaryExpression);
                    break;
                case BoundAverageMethod averageMethod:
                    return EvaluateAverageMethod(averageMethod);
                    break;
                case BoundMinMethod minimumMethod:
                    return EvaluateMinMethod(minimumMethod);
                    break;
                case BoundMaxMethod maximumMethod:
                    return EvaluateMaxMethod(maximumMethod);
                    break;
                default:
                    // throws an exception if no nodes matched and it is unexpected
                    throw new Exception("Unexpected node: " + node.Type);
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression literalExpression)
        {
            return literalExpression.Value;
        }

        private static object EvaluateFloatExpression(BoundFloatExpression floatExpression)
        {
            return floatExpression.Value;
        }

        // returns variable
        private object EvaluateVariableExpression(BoundVariableExpression variableExpression)
        {
            return _variables[variableExpression.Variable];
        }

        // returns variable value
        private object EvaluateAssignmentExpression(BoundAssignmentExpression assignmentExpression)
        {
            var value = EvaluateExpression(assignmentExpression.Expression);
            _variables[assignmentExpression.Variable] = value;
            return value;
        }

        // returns result of unary calculation
        private object EvaluateUnaryExpression(BoundUnaryExpression unaryExpression)
        {
            var operand = EvaluateExpression(unaryExpression.Operand);
            switch (unaryExpression.Op.Type)
            {
                case BoundUnaryOperatorType.Identity:
                    return Convert.ToDouble(operand);

                case BoundUnaryOperatorType.Negation:
                    return -Convert.ToDouble(operand);

                case BoundUnaryOperatorType.LogicalReverse:
                    return !(bool)operand;

                default:
                    throw new Exception("Unexpected unary operator: " + unaryExpression.Op);
            }
        }

        // returns result of binary calculation
        private object EvaluateBinaryExpression(BoundBinaryExpression binaryExpression)
        {
            // gets the values of the left and right side of the binary expression
            var left = EvaluateExpression(binaryExpression.Left);
            var right = EvaluateExpression(binaryExpression.Right);
            // does the correct binary calculation depending on the operator
            switch (binaryExpression.Op.OpType)
            {
                case BoundBinaryOperatorType.Add:
                    return Convert.ToDouble(left) + Convert.ToDouble(right);

                case BoundBinaryOperatorType.Subtract:
                    return Convert.ToDouble(left) - Convert.ToDouble(right);

                case BoundBinaryOperatorType.Multiply:
                    return Convert.ToDouble(left) * Convert.ToDouble(right);

                case BoundBinaryOperatorType.Divide:
                    return Convert.ToDouble(left) / Convert.ToDouble(right);

                case BoundBinaryOperatorType.Modulo:
                    return Convert.ToDouble(left) % Convert.ToDouble(right);

                case BoundBinaryOperatorType.Power:
                    return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));

                case BoundBinaryOperatorType.LogicalAND:
                    return (bool)left && (bool)right;

                case BoundBinaryOperatorType.LogicalOR:
                    return (bool)left || (bool)right;

                case BoundBinaryOperatorType.TrueEquals:
                    return Equals(left, right);

                case BoundBinaryOperatorType.NOTEquals:
                    return !Equals(left, right);

                case BoundBinaryOperatorType.LessThan:
                    return Convert.ToSingle(left) < Convert.ToSingle(right);

                case BoundBinaryOperatorType.LessThanOrEquals:
                    return Convert.ToSingle(left) <= Convert.ToSingle(right);

                case BoundBinaryOperatorType.GreaterThan:
                    return Convert.ToSingle(left) > Convert.ToSingle(right);

                case BoundBinaryOperatorType.GreaterThanOrEquals:
                    return Convert.ToSingle(left) >= Convert.ToSingle(right);
                default:
                    // throws an exception of an unexpected binary operator if one appears
                    throw new Exception("Unexpected binary operator: " + binaryExpression.Op);
            }
        }

        // Method to evaluate the average number out of the numbers added
        // Done by summing up all the numbers, dividing them and returning the result
        private static object EvaluateAverageMethod(BoundAverageMethod averageMethod)
        {
            double sum = 0;
            var totalNumbers = 0;
            foreach (var number in averageMethod.Numbers)
            {

                sum += Convert.ToDouble(number.Text);
                totalNumbers++;
            }
            double result = Convert.ToDouble(sum / totalNumbers);
            return result;
        }

        // Method to evaluate the smallest number entered
        // Done by assigining the first number value, as number change will be false
        // Then only changes smallest number, if a smaller number is found and returns the smallest number
        private object EvaluateMinMethod(BoundMinMethod minimumMethod)
        {
            double smallestNumber = 0;
            double numb = 0;
            bool numberChange = false;
            foreach (var number in minimumMethod.Numbers)
            {
                numb = Convert.ToDouble(number.Text);
                if (smallestNumber > numb)
                {
                    smallestNumber = numb;
                }

                if (!numberChange)
                {
                    smallestNumber = numb;
                    numberChange = true;
                }
            }
            return smallestNumber;
        }

        // Method to evalaute the largest number
        // Done by assigining the larest number value if any number is larger it will assign
        private object EvaluateMaxMethod(BoundMaxMethod maximumMethod)
        {
            double largestNumber = 0;
            double numb = 0;
            foreach (var number in maximumMethod.Numbers)
            {
                numb = Convert.ToDouble(number.Text);
                if (largestNumber < numb)
                {
                    largestNumber = numb;
                }
            }
            return largestNumber;
        }

        // Evaluates the statement from the parameter and returns the value of the statement method executed
        private void EvaluateStatement(BoundStatement statement)
        {
            // checks if it is a number expression
            switch (statement)
            {
                case BoundBlockStatement block:
                    EvaluateBlockStatement(block);
                    break;
                case BoundExpressionStatement expression:
                    EvaluateExpressionStatement(expression);
                    break;
                case BoundVariableDeclaration variable:
                    EvaluateVariableDeclaration(variable);
                    break;
                case BoundIfStatement ifStatement:
                    EvaluateIfStatement(ifStatement);
                    break;
                case BoundWhileStatement whileStatement:
                    EvaluateWhileStatement(whileStatement);
                    break;
                case BoundForStatement forStatement:
                    EvaluateForStatement(forStatement);
                    break;
                default:
                    // throws an exception if no nodes matched and it is unexpected
                    throw new Exception("Unexpected statement: " + statement.Type);
            }
        }

        private void EvaluateBlockStatement(BoundBlockStatement block)
        {
            foreach (var statement in block.Statements)
            {
                EvaluateStatement(statement);
            }
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement expression)
        {
            _lastValue = EvaluateExpression(expression.Expression);
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration variable)
        {
            // returns the value of the declared variable
            var value = EvaluateExpression(variable.Initaliser);
            _variables[variable.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateIfStatement(BoundIfStatement ifStatement)
        {
            var condition = (bool)EvaluateExpression(ifStatement.Condition);
            if (condition)
            {
                EvaluateStatement(ifStatement.ThenStatement);
            }
            else if (ifStatement.ElseStatement != null)
            {
                EvaluateStatement(ifStatement.ElseStatement);
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement whileStatement)
        {
            while ((bool)EvaluateExpression(whileStatement.Condition))
                EvaluateStatement(whileStatement.Body);
        }


        private void EvaluateForStatement(BoundForStatement forStatement)
        {
            // gets value of the lower and upper bound
            var lowerBound = (int)EvaluateExpression(forStatement.LowerBound);
            var upperBound = (int)EvaluateExpression(forStatement.UpperBound);

            for (var i = lowerBound; i <= upperBound; i++)
            {
                _variables[forStatement.Variable] = i;
                EvaluateStatement(forStatement.Body);
            }
        }
    }
}
