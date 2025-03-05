using System.Collections;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Coding_Language.Binding;
using Coding_Language.Text;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostics>
    {
        public readonly List<Diagnostics> _diagnostics = new List<Diagnostics>();

        /// <summary>
        /// Used to add up all instances of the diagnostics into one variable
        /// </summary>
        /// <param name="diagnostics"></ diganostics to add>
        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        /// <summary>
        /// Creates an error message for the invalid character and runs report to centralise and standardise the error report
        /// </summary>
        /// <param name="position"></ position of the invalid character>
        /// <param name="currentCharacter"></ the invalid character entered>
        public void ReportInvalidCharacter(int position, char currentCharacter)
        {
            var message = "Invalid character entered: " + currentCharacter;
            var textSpan = new TextSpan(position, 1);
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error message for the invalid number an runs report to centralise and standardise the error report
        /// </summary>
        /// <param name="textSpan"></ gets position, length etc>
        /// <param name="text"></ the number>
        /// <param name="type"></ not a valid type of int>
        public void ReportInvalidNumber(TextSpan textSpan, string text, Type type)
        {
            var message = "The number: " + text + " is not a valid: " + type;
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error for the undefined binary operator an runs report to centralise and standardise the error report
        /// </summary>
        /// <param name="textSpan"></ gets position, length etc>
        /// <param name="operatorText"></ operator text (equation)>
        /// <param name="leftType"></ value of the left side of the operator>
        /// <param name="rightType"></ value of the right side of the operator>
        public void ReportUndefinedBinaryOperator(TextSpan textSpan, string operatorText, Type leftType, Type rightType)
        {
            var message = "Binary operator: " + operatorText + " is not defined for type: " + leftType + " and " + rightType;
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error for the undefined variable
        /// </summary>
        /// <param name="textSpan"></ information about the input>
        /// <param name="name"></ name of the variable that was trying to be used>
        public void ReportUndefinedVariable(TextSpan textSpan, string name)
        {
            var message = "Variable name: " + name + " does not exit";
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error for the undefined unary operator an runs reprot to centralise and standardise the error report
        /// </summary>
        /// <param name="textSpan"></ gets position, length etc>
        /// <param name="operatorText"></ operator text (equation)>
        /// <param name="operandType"></ type of operand used e.g +>
        public void ReportUndefinedUnaryOperator(TextSpan textSpan, string operatorText, Type operandType)
        {
            var message = "Unary operator: " + operatorText + " is not defined for type: " + operandType;
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error for the unexpected token an runs report to centralise and standardise the error report
        /// </summary>
        /// <param name="textSpan"></ gets position, length etc>
        /// <param name="actualType"></ actual token type that the computer got>
        /// <param name="expectedType"></ expected token type>
        public void ReportUnexpectedToken(TextSpan textSpan, TokenType actualType, TokenType expectedType)
        {
            var message = "Unexpected token: " + actualType + " expected: " + expectedType;
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error if the user tries to declare a variable that has already been declared
        /// </summary>
        /// <param name="textSpan"></ inforamtion about the varialbe>
        /// <param name="name"></ variable name>
        public void ReportVariableAlreadyDeclared(TextSpan textSpan, string name)
        {
            var message = "Variable: " + name + " is already declared";
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error if the user tries to assign a variable that is read only
        /// </summary>
        /// <param name="textSpan"></ information about the variable>
        /// <param name="name"></ name of the variable>
        internal void ReportCannotAssign(TextSpan textSpan, string name)
        {
            var message = "Variable is read only and has already been assigned: " + name;
            Report(textSpan, message);
        }

        /// <summary>
        /// Creates an error if a user tries to convert from a type to a type they cannot convert to
        /// </summary>
        /// <param name="textSpan"></ information about the variable>
        /// <param name="fromType"></ type it tried to convert from>
        /// <param name="toType"></ type it tried to convert to>
        internal void ReportCannotConvert(TextSpan textSpan, Type fromType, Type toType)
        {
            var message = "Cannot convert type: " + fromType + " to type: " + toType;
            Report(textSpan, message);
        }

        IEnumerator<Diagnostics> IEnumerable<Diagnostics>.GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _diagnostics.GetEnumerator();

        /// <summary>
        /// Method ot standardise how the errors are addded and handled
        /// </summary>
        /// <param name="span"></ allows for the creation of the diagnostic class and get info on position etc>
        /// <param name="message"></ actual error message to output>
        private void Report(TextSpan span, string message)
        {
            var diagnostics = new Diagnostics(span, message);
            _diagnostics.Add(diagnostics);
        }
    }
}
