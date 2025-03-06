using Coding_Language.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;
using static System.Net.Mime.MediaTypeNames;

namespace Coding_Language.Syntax
{
    internal sealed class Lexer
    {
        private SourceText text;
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();

        private int position;
        private int start;
        private TokenType type;
        private object value;

        public Lexer(SourceText text)
        {
            this.text = text;
            position = 0;
        }

        public DiagnosticBag Diagnostics => diagnostics;

        public char Peek(int offset)
        {
            var index = position + offset;

            if (index >= text.Length)
            {
                return '\0';
            }
            else
            {
                return text[index];
            }
        }

        private char currentCharacter => Peek(0);

        private Dictionary<string, TokenType> mapInputToToken = new Dictionary<string, TokenType>
        {
            {"+", TokenType.Add},
            {"-", TokenType.Subtract},
            {"*", TokenType.Multiply},
            {"/", TokenType.Divide},
            {"(", TokenType.LBracket},
            {")", TokenType.RBracket},
            {"{", TokenType.LBrace},
            {"}", TokenType.RBrace},
            {"%", TokenType.Modulo},
            {"^", TokenType.Power},
            {",", TokenType.Comma},
            {"=", TokenType.Equal},
            {"<", TokenType.LessThan},
            {">", TokenType.GreaterThan},
            {"!", TokenType.NOT},
            {"==", TokenType.TrueEquals},
            {"<=", TokenType.LessThanOrEquals},
            {">=", TokenType.GreaterThanOrEquals},
            {"!=", TokenType.NOTEquals},
            {"&&", TokenType.LogicalAND},
            {"||", TokenType.LogicalOR}
        };

        /// <summary>
        /// Function that checks what kind of character the current character is to get the right token
        /// Also able to get the correct token for words or multiple number ints etc
        /// Done by running the correct method
        /// </summary>
        /// <returns></ The correct token corresponding to the input>
        public Token NextTokens()
        {

            start = position;
            // Assign error token to throw error, or assigned proper value and work as expected
            type = TokenType.Error;
            value = null;

            switch (currentCharacter)
            {
                // special characters that require unique logic are in the switch case
                case '\0':
                    type = TokenType.EndOfFile;
                    break;
                case '=':
                case '<':
                case '>':
                case '!':
                    string checkSecondChar = currentCharacter.ToString();
                    position++;
                    // checks an ='s follows, because it will the change the logic and token type
                    if (currentCharacter != '=')
                    {
                        Console.WriteLine(currentCharacter);
                        type = mapInputToToken[checkSecondChar];
                        break;
                    }
                    else
                    {
                        checkSecondChar += currentCharacter.ToString();
                        type = mapInputToToken[checkSecondChar];
                        position++;
                        break;
                    }
                // No valid tokens for single characters, only valid tokens for two duplicate characters
                case '&':
                    position++;
                    if (currentCharacter == '&')
                    {
                        type = TokenType.LogicalAND;
                        position++;
                        break;
                    }
                    diagnostics.ReportInvalidCharacter(position, currentCharacter);
                    break;

                case '|':
                    position++;
                    if (currentCharacter == '|')
                    {
                        type = (TokenType.LogicalOR);
                        position++;
                        break;
                    }
                    diagnostics.ReportInvalidCharacter(position, currentCharacter);
                    break;
                default:
                    if (char.IsWhiteSpace(currentCharacter))
                    {
                        ReadWhiteSpace();
                    }
                    else if (char.IsDigit(currentCharacter))
                    {
                        ReadNumberOrFloat();
                    }
                    else if (char.IsLetter(currentCharacter))
                    {
                        ReadText();
                    }
                    // maps regular tokens that don't need special logic
                    else if (mapInputToToken.ContainsKey(currentCharacter.ToString()))
                    {
                        type = mapInputToToken[currentCharacter.ToString()];
                        position++;
                    }
                    else
                    {
                        diagnostics.ReportInvalidCharacter(position, currentCharacter);
                        position++;
                    }
                    break;
            }
            var length = position - start;
            var text = SyntaxFacts.GetText(type);
            if (text == null)       // the text is dynamic
            {
                text = this.text.ToString(start, length);
            }
            return new Token(type, text, start, value);
        }



        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(currentCharacter))
            {
                position++;
            }
            type = TokenType.WhiteSpace;
        }

        private void ReadNumberOrFloat()
        {
            while (char.IsDigit(currentCharacter) || currentCharacter == '.')
            {
                position++;
            }
            var length = position - start;
            var text = this.text.ToString(start, length);

            // Handles logic for decimal values, as requires a different token from int
            if (text.Contains('.'))
            {
                if (!float.TryParse(text, out var floatValue))
                {
                    diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(float));
                }
                value = floatValue;
                type = TokenType.Float;
            }
            // Handles int logic
            else
            {
                if (!int.TryParse(text, out var intValue))
                {
                    diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
                }
                value = intValue;
                type = TokenType.Int;
            }
        }

        private void ReadText()
        {
            while (char.IsLetter(currentCharacter))
            {
                position++;
            }
            var length = position - start;
            var text = this.text.ToString(start, length);
            type = SyntaxFacts.GetKeyWordType(text);
        }
    }
}
