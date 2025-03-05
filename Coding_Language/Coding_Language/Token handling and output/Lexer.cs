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
        private SourceText _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;
        private int _start;
        private TokenType _type;
        private object _value;

        public Lexer(SourceText text)
        {
            // taking the input into the lexer and assiging it to the private _input variable
            // This is so I can use the string input thorught the class
            _text = text;
            _position = 0;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        /// <summary>
        /// Method to update character
        /// </summary>
        /// <returns>< \0 if _position is larger than input></ else returns current character>
        public char Peek(int offset)
        {
            var index = _position + offset;
            // checks if the index is larger than input length
            if (index >= _text.Length)
            {
                return '\0';
            }
            else
            {
                return _text[index];
            }
        }

        private char _currentCharacter => Peek(0);

        /// <summary>
        /// Function that checks what kind of character the current character is to get the right token
        /// Also able to get the correct token for words or multiple number ints etc
        /// Done by running the correct method
        /// </summary>
        /// <returns></ The correct token corresponding to the input>
        public Token NextTokens()
        {

            _start = _position;
            // makes the token automatical bad, as I will asign a value if it is not
            _type = TokenType.Error;
            _value = null;

            // checks the character and assigns the correct token
            // also increments position by one, unless it is the end of file token
            switch (_currentCharacter)
            {
                case '\0':
                    _type = TokenType.EndOfFile;
                    break;
                case '+':
                    _type = TokenType.Add;
                    _position++;
                    break;
                case '-':
                    _type = TokenType.Subtract;
                    _position++;
                    break;
                case '*':
                    _type = TokenType.Multiply;
                    _position++;
                    break;
                case '/':
                    _type = TokenType.Divide;
                    _position++;
                    break;
                case '(':
                    _type = TokenType.LBracket;
                    _position++;
                    break;
                case ')':
                    _type = TokenType.RBracket;
                    _position++;
                    break;
                case '{':
                    _type = TokenType.LBrace;
                    _position++;
                    break;
                case '}':
                    _type = TokenType.RBrace;
                    _position++;
                    break;
                case '%':
                    _type = TokenType.Modulo;
                    _position++;
                    break;
                case '^':
                    _type = TokenType.Power;
                    _position++;
                    break;
                case ',':
                    _type = TokenType.Comma;
                    _position++;
                    break;
                // these characters can be one or two characters in length
                // so if statement used to get the right character, with the correct values being changed
                case '=':
                    _position++;
                    if (_currentCharacter == '=')
                    {
                        _type = TokenType.TrueEquals;
                        _position++;
                        break;
                    }
                    else
                    {
                        _type = TokenType.Equal;
                        break;
                    }
                case '<':
                    _position++;
                    if (_currentCharacter != '=')
                    {
                        _type = TokenType.LessThan;
                    }
                    else
                    {
                        _type = TokenType.LessThanOrEquals;
                        _position++;
                    }
                    break;
                case '>':
                    _position++;
                    if (_currentCharacter != '=')
                    {
                        _type = TokenType.GreaterThan;
                    }
                    else
                    {
                        _type = TokenType.GreaterThanOrEquals;
                        _position++;
                    }
                    break;
                case '!':
                    _position++;
                    if (_currentCharacter == '=')
                    {
                        _type = TokenType.NOTEquals;
                        _position++;
                        break;
                    }
                    else
                    {
                        _type = TokenType.NOT;
                        break;
                    }
                // for characters like these, where only one character is an error
                // it runs a report invalid character to create that error
                case '&':
                    _position++;
                    if (_currentCharacter == '&')
                    {
                        _type = TokenType.LogicalAND;
                        _position++;
                        break;
                    }
                    _diagnostics.ReportInvalidCharacter(_position, _currentCharacter);
                    break;

                case '|':
                    _position++;
                    if (_currentCharacter == '|')
                    {
                        _type = (TokenType.LogicalOR);
                        _position++;
                        break;
                    }
                    _diagnostics.ReportInvalidCharacter(_position, _currentCharacter);
                    break;
                    // runs if none of the other tokens were made
                default:
                    if (char.IsWhiteSpace(_currentCharacter))
                    {
                        // loops through the input until the character is no longer a whitespace
                        ReadWhiteSpace();
                    }
                    else if (char.IsDigit(_currentCharacter))
                    {
                        ReadNumberOrFloat();
                    }
                    else if (char.IsLetter(_currentCharacter))
                    {
                        ReadText();
                    }
                    else
                    {
                        _diagnostics.ReportInvalidCharacter(_position, _currentCharacter);
                        _position++;
                    }
                    break;
            }
            var length = _position - _start;
            var text = SyntaxFacts.GetText(_type);
            if (text == null)       // the text is dynamic
            {
                text = _text.ToString(_start, length);
            }
            return new Token(_type, text, _start, _value);
        }

        private void ReadWhiteSpace()
        {
            // gets all the white spaces and increments position accordingly
            while (char.IsWhiteSpace(_currentCharacter))
            {
                _position++;
            }
            _type = TokenType.WhiteSpace;
        }

        private void ReadNumberOrFloat()
        {
            // loops through the input until the character is no longer a digit or .
            while (char.IsDigit(_currentCharacter) || _currentCharacter == '.')
            {
                _position++;
            }
            var length = _position - _start;                    // finds the length of the number
            var text = _text.ToString(_start, length);        // creates a string value of the number

            if (text.Contains('.')) // checks if there is a decimal
            {
                // checks if it is a valid float
                if (!float.TryParse(text, out var floatValue))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(float));
                }
                _value = floatValue;
                _type = TokenType.Float;
            }
            else
            {
                // checks if it is a valid int
                if (!int.TryParse(text, out var intValue))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
                }
                _value = intValue;
                _type = TokenType.Int;
            }
        }

        private void ReadText()
        {
            while (char.IsLetter(_currentCharacter))
            {
                _position++;
            }
            var length = _position - _start;                    // finds the length of the word
            var text = _text.ToString(_start, length);        // creates the value of the word
            _type = SyntaxFacts.GetKeyWordType(text);       // used to create the key word token
        }
    }
}
