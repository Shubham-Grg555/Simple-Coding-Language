using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    public static class SyntaxFacts
    {

        /// <summary>
        /// Used to get the precedence for unary operators (for unit tests)
        /// </summary>
        /// <param name="type"></ unary operator type>
        /// <returns></ returns 3 if there is a unary operator, else it returns 0 if there are no operators>
        public static int GetUnaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Add:
                case TokenType.Subtract:

                case TokenType.NOT:
                    return 7;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Used to get the next binary operator and the precedence of the operator (for unit tests)
        /// </summary>
        /// <param name="type"></ operator type>
        /// <returns></ returns 1 if it is a + or -, 2 if it is * or / or 0 if it is not, higher the number = higher precedence>
        public static int GetBinaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Power:
                    return 6;

                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Modulo:
                    return 5;
                
                case TokenType.Add:
                case TokenType.Subtract:
                    return 4;
                
                case TokenType.TrueEquals:
                case TokenType.NOTEquals:
                case TokenType.LessThan:
                case TokenType.LessThanOrEquals:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEquals:
                    return 3;
                
                case TokenType.LogicalAND:
                    return 2;
                
                case TokenType.LogicalOR:
                    return 1;
                
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Used to get the keyword or identifer token from an input
        /// </summary>
        /// <param name="text"></ input text>
        /// <returns></ correct key work token>
        /// <returns></ correct identifier token>
        internal static TokenType GetKeyWordType(string text)
        {
            // so the language is not case sensitive, so it is easier to code
            text = text.ToLower();
            switch (text)
            {
                case "true":
                    return TokenType.TrueKeyWord;
                case "false":
                    return TokenType.FalseKeyWord;
                case "let":
                    return TokenType.LetKeyWord;
                case "var":
                    return TokenType.VarKeyWord;
                case "if":
                    return TokenType.IfKeyWord;
                case "else":
                    return TokenType.ElseKeyWord;
                case "while":
                    return TokenType.WhileKeyWord;
                case "for":
                    return TokenType.ForKeyWord;
                case "to":
                    return TokenType.ToKeyWord;
                case "and":
                    return TokenType.LogicalAND;
                case "or":
                    return TokenType.LogicalOR;
                case "mod":
                    return TokenType.Modulo;
                case "avg":
                    return TokenType.AvgKeyWord;
                case "min":
                    return TokenType.MinKeyWord;
                case "max":
                    return TokenType.MaxKeyWord;
                case "is":
                    return TokenType.TrueEquals;
                case "isnot":
                    return TokenType.NOTEquals;
                default:
                    return TokenType.Identifier;
            }
        }
        

        /// <summary>
        /// Used to test the parser and syntax facts
        /// </summary>
        /// <param name="type"></ token type>
        /// <returns></ returns a token type >
        public static string GetText(TokenType type)
        {
            switch (type)
            {
                case TokenType.Add:
                    return "+";
                case TokenType.Subtract:
                    return "-";
                case TokenType.Multiply:
                    return "*";
                case TokenType.Divide:
                    return "/";
                case TokenType.Modulo:
                    return "%";
                case TokenType.Power:
                    return "^";
                case TokenType.LBracket:
                    return "(";
                case TokenType.RBracket:
                    return ")";
                case TokenType.LBrace:
                    return "{";
                case TokenType.RBrace:
                    return "}";
                case TokenType.LessThan:
                    return "<";
                case TokenType.LessThanOrEquals:
                    return "<=";
                case TokenType.GreaterThan:
                    return ">";
                case TokenType.GreaterThanOrEquals:
                    return ">=";
                case TokenType.Equal:
                    return "=";
                case TokenType.NOT:
                    return "!";
                case TokenType.Comma:
                    return ",";
                case TokenType.LogicalAND:
                    return "&&";
                case TokenType.LogicalOR:
                    return "||";
                case TokenType.TrueEquals:
                    return "==";
                case TokenType.NOTEquals:
                    return "!=";
                case TokenType.TrueKeyWord:
                    return "true";
                case TokenType.FalseKeyWord:
                    return "false";
                case TokenType.LetKeyWord:
                    return "let";
                case TokenType.VarKeyWord:
                    return "var";
                case TokenType.IfKeyWord:
                    return "if";
                case TokenType.ElseKeyWord:
                    return "else";
                case TokenType.WhileKeyWord:
                    return "while";
                case TokenType.ForKeyWord:
                    return "for";
                case TokenType.ToKeyWord:
                    return "to";
                case TokenType.AvgKeyWord:
                    return "avg";
                case TokenType.MinKeyWord:
                    return "min";
                case TokenType.MaxKeyWord:
                    return "max";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Used to get binary operators
        /// </summary>
        /// <returns></ returns an array of tokens, if the precedence is greater than 0>
        public static IEnumerable<TokenType> GetBinaryOperatorTypes()
        {
            var types = (TokenType[])Enum.GetValues(typeof(TokenType));
            foreach (var type in types)
            {
                if (GetBinaryOperatorPrecedence(type) > 0)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Used to get unary operators
        /// </summary>
        /// <returns></ returns an array of tokens, if the precedence is greater than 0>
        public static IEnumerable<TokenType> GetUnaryOperatorTypes()
        {
            var types = (TokenType[])Enum.GetValues(typeof(TokenType));
            foreach (var type in types)
            {
                if (GetUnaryOperatorPrecedence(type) > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
