using Coding_Language.Binding;
using Coding_Language.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Syntax
{
    internal sealed class Parser
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly SourceText _text;
        private readonly ImmutableArray<Token> _tokens;

        private int _position;

        public Parser(SourceText text)
        {
            var tokens = new List<Token>();

            Lexer lexer = new Lexer(text);
            Token token;
            // runs till end of file token is found and uses do to avoid trying to use token.Type value before assignment
            // done like this so that the correct current token is checked
            do
            {
                token = lexer.NextTokens();

                // checks if token is not a whitespace or error
                if (token.Type != TokenType.WhiteSpace && token.Type != TokenType.Error)
                {
                    tokens.Add(token);  // adds token into list
                }

            } while (token.Type != TokenType.EndOfFile);
            _text = text;
            _tokens = tokens.ToImmutableArray(); // puts the tokens into an array
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        /// <summary>
        /// Used to peek ahead into the input (see characters ahead of the current character)
        /// </summary>
        /// <param name="offset"></ offset is how far you peek ahead>
        /// <returns></the token at desired offset> 
        /// </ if offset is higher than the total number of tokens, it shows the token before the end of file>
        private Token Peek(int offset)
        {
            int index = _position + offset;
            // checks if index is larger than the total amount of tokens
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1]; // -1 after token.Length as index for array starts at 0 but .Length starts at 1

            return _tokens[index];
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private Token Current => Peek(0);

        /// <summary>
        /// Gets the next token and increments position
        /// </summary>
        /// <returns></the current token value>
        private Token NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        /// <summary>
        /// Checks if the current token matches the specified token type
        /// </summary>
        /// <param name="type"></Token type>
        /// <returns></ next tokem, so that it consumes the current token
        /// a token with little info, as an error is created, so code won't run in the evaluator>
        private Token Match(TokenType type)
        {
            // checks if the token entered as the arguement is the same the current token
            if (Current.Type == type)
            {
                return NextToken();
            }
            else    // creates a token of the specified type
            {
                _diagnostics.ReportUnexpectedToken(Current.Span, Current.Type, type);
                return new Token(type, null, Current.Position, null);
            }
        }


        /// <summary>
        /// Parses compilation unit
        /// </summary>
        /// <returns></ Instance of the compilation syntax with parameters to make the compilation unit syntax>
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            // runs method first to see if there is a factor operator, else runs as normal as the math will be done correctly
            var statement = ParseStatement();
            var endOfFile = Match(TokenType.EndOfFile);
            return new CompilationUnitSyntax(statement, endOfFile);
        }

        /// <summary>
        /// Runs correct parse statement method depending on current the token type
        /// </summary>
        /// <returns></ an instance of the specific statement syntax created>
        private StatementSyntax ParseStatement()
        {
            if (Current.Type == TokenType.LBrace)
            {
                return ParseBlockStatement();
            }
            else if (Current.Type == TokenType.LetKeyWord || Current.Type == TokenType.VarKeyWord)
            {
                return ParseVariableDeclaration();
            }
            else if (Current.Type == TokenType.IfKeyWord)
            {
                return ParseIfStatement();
            }
            else if (Current.Type == TokenType.WhileKeyWord)
            {
                return ParseWhileStatement();
            }
            else if (Current.Type == TokenType.ForKeyWord)
            {
                return ParseForStatement();
            }
            return ParseExpressionStatement();
        }

        /// <summary>
        /// Adds statement block and generates block statement syntax if it is a proper statement (parses block statement)
        /// Will create an error if the necesssary things have not been entered
        /// </summary>
        /// <returns></ instance of Block statement syntax with parameters made in the method>
        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var leftBraceToken = Match(TokenType.LBrace);

            while (Current.Type !=  TokenType.EndOfFile && Current.Type != TokenType.RBrace)
            {
                var startToken = Current;
                var statement = ParseStatement();
                statements.Add(statement);
                if (Current == startToken)
                {
                    NextToken();
                }
            }

            var rightBraceToken = Match(TokenType.RBrace);
            return new BlockStatementSyntax(leftBraceToken, statements.ToImmutable(), rightBraceToken);
        }

        /// <summary>
        /// Used to parse variable declaration by checking if the necessary things have been entered first
        /// If the necessary things have not been eneted, it will create an error
        /// </summary>
        /// <returns></ an instance of variable declaration syntax, using the parameters as the tokens entered>
        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Type == TokenType.LetKeyWord ? TokenType.LetKeyWord : TokenType.VarKeyWord;
            var keyWord = Match(expected);
            var identifier = Match(TokenType.Identifier);
            var equals = Match(TokenType.Equal);
            var initialiser = ParseExpression();
            return new VariableDeclarationSyntax(keyWord, identifier, equals, initialiser);
        }

        /// <summary>
        /// Parses expression syntax
        /// </summary>
        /// <returns></ instance of Expression statement syntax with parameters made in the method>
        private StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        /// <summary>
        /// Parses the if statement, as long as the necessary things have been entered
        /// Done by running the match token to see if the correct token types have been entered
        /// Also creates the necessary variables needed to create an instance of the constructor
        /// </summary>
        /// <returns></ instance of the if statement syntax>
        private StatementSyntax ParseIfStatement()
        {
            var keyWord = Match(TokenType.IfKeyWord);
            var condition = ParseExpression();
            Match(TokenType.LBrace);
            var statement = ParseStatement();
            Match(TokenType.RBrace);
            var elseClause = ParseElseClause();
            return new IfStatementSyntax(keyWord, condition, statement, elseClause);
        }

        /// <summary>
        /// Runs after if statement made, to see if there is an else statement
        /// Creates an erorr if the tokens fail the match method
        /// Also creates the necessary variables needed to create an instance of the constructor
        /// </summary>
        /// <returns></ instance of the else clause syntax>
        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Type != TokenType.ElseKeyWord)
            {
                return null;
            }
            var keyWord = NextToken();
            Match(TokenType.LBrace);
            var statement = ParseStatement();
            Match(TokenType.RBrace);
            return new ElseClauseSyntax(keyWord, statement);
        }

        /// <summary>
        /// Parses whle statement
        /// Done by checking if the match method fails or not, as it will create an error
        /// Also creates the necessary variables needed to create an instance of the constructor
        /// </summary>
        /// <returns></ an instance of the while statement syntax>
        private StatementSyntax ParseWhileStatement()
        {
            var keyWord = Match(TokenType.WhileKeyWord);
            var condition = ParseExpression();
            var body = ParseStatement();
            return new WhileStatementSyntax(keyWord, condition, body);
        }

        /// <summary>
        /// Parses for statement
        /// Done by checking if the match method fails or not, as it will create an error
        /// Also creates the necessary variables needed to create an instance of the constructor
        /// </summary>
        /// <returns></ an instance of the for statement syntax>
        private StatementSyntax ParseForStatement()
        {
            var keyWord = Match(TokenType.ForKeyWord);
            var identifier = Match(TokenType.Identifier);
            var equalsToken = Match(TokenType.Equal);
            var lowerBound = ParseExpression();
            var toKeyword = Match(TokenType.ToKeyWord);
            var upperBound = ParseExpression();
            var body = ParseStatement();
            return new ForStatementSyntax(keyWord, identifier, equalsToken, lowerBound, toKeyword, upperBound, body);
        }

        /// <summary>
        /// Just returns parse assignment expression
        /// </summary>
        /// <returns></ parse assignment expression>
        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        /// <summary>
        /// Allows the computer to parse an assignment expression
        /// </summary>
        /// <returns></ returns a new instance of an assignment expression if it is one>
        /// <returns></ else it returns the value of the parse binary expression method>
        private ExpressionSyntax ParseAssignmentExpression()
        {

            if (Peek(0).Type == TokenType.Identifier && Peek(1).Type == TokenType.Equal)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }
            return ParseBinaryExpression();
        }
        /// <summary>
        /// parses expression and gets the precedence so the mathematical operations are done correctly
        /// </summary>
        /// <param name="parentPrecedence"></ the precedence of the parent node>
        /// <returns></ left value>
        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();
            // checks if there are unary operators
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                // gets the necessary variables to create the expression instance
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                // does the normal binary operator precedence
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precendence = Current.Type.GetBinaryOperatorPrecedence();
                // if there is no current precedence it breaks
                // or if the precedence is lower or equal to current precedence as it will be parsed later 
                if (precendence == 0 || precendence <= parentPrecedence)
                {
                    break;
                }
                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precendence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }


        /// <summary>
        /// Used to check what type of token it is and returns the correct expression syntax
        /// </summary>
        /// <returns></ creates the correct instance of the expression syntax of the token>
        private ExpressionSyntax ParsePrimaryExpression()
        {
            // checks if there are any brackets and runs them first)
            switch (Current.Type)
            {
                case TokenType.LBracket:
                    return ParseBracketExpression();
                case TokenType.TrueKeyWord:
                case TokenType.FalseKeyWord:
                    return ParseBooleanLiteralExpression();
                case TokenType.AvgKeyWord:
                    return ParseAverageMethod();
                case TokenType.MinKeyWord:
                    return ParseMinMethod();
                case TokenType.MaxKeyWord:
                    return ParseMaxMethod();
                case TokenType.Identifier:
                    return ParseNameExpression();
                case TokenType.Float:
                    return ParseLiteralFloatExpression();
                default:
                    return ParseLiteralExpression();
            }
        }

        /// <summary>
        /// Parses the bracket expression
        /// Done by creating the correct values and assigining them to variables
        /// Creates errors by checking the tokens using the match method
        /// Returns an error if they do not match
        /// </summary>
        /// <returns></ instance of the bracket expression syntax>
        private ExpressionSyntax ParseBracketExpression()
        {
            var left = NextToken();
            var expression = ParseExpression();   // used to parse the expression
            var right = Match(TokenType.RBracket);  // checks if a right bracket was entered
            return new BracketExpressionSyntax(left, expression, right);
        }

        /// <summary>
        /// Parses the boolean expression
        /// Done by creating the correct values and assigining them to variables
        /// Creates errors by checking the tokens using the match method
        /// Returns an error if they do not match
        /// </summary>
        /// <returns></ instance of the boolean expression syntax>
        private ExpressionSyntax ParseBooleanLiteralExpression()
        {
            var keyWordToken = NextToken();
            var value = keyWordToken.Type == TokenType.TrueKeyWord;
            return new LiteralExpressionSyntax(keyWordToken, value);
        }

        /// <summary>
        /// Parses the average method
        /// Done by creating the correct values and assigining them to variables
        /// Creates errors by checking the tokens using the match method
        /// Returns an error if they do not match
        /// </summary>
        /// <returns></ instance of the average method syntax>
        private ExpressionSyntax ParseAverageMethod()
        {
            List<Token> numbers = new List<Token>();
            var keyWord = Match(TokenType.AvgKeyWord);
            Match(TokenType.LBracket);
            if (Current.Type != TokenType.Int && Current.Type != TokenType.Float && Current.Type != TokenType.Comma)
            {
                Match(TokenType.Int);
            }
            while (Current.Type == TokenType.Int || Current.Type == TokenType.Float || Current.Type == TokenType.Comma)
            {
                if (Current.Type == TokenType.Int)
                {
                    numbers.Add(Match(TokenType.Int));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Match(TokenType.Comma);
                    }
                }
                else if (Current.Type == TokenType.Float)
                {
                    numbers.Add(Match(TokenType.Float));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Match(TokenType.Comma);
                    }
                }
                else
                {
                    return new AverageMethodSyntax(keyWord, numbers);
                }
            }
            Match(TokenType.RBracket);
            return new AverageMethodSyntax(keyWord, numbers);
        }

        /// <summary>
        /// Parses the minimum method
        /// Done by creating the correct values and assigining them to variables
        /// Creates errors by checking the tokens using the match method
        /// Returns an error if they do not match
        /// </summary>
        /// <returns></ instance of the minimum method syntax>
        private ExpressionSyntax ParseMinMethod()
        {
            List<Token> numbers = new List<Token>();
            var keyWord = Match(TokenType.MinKeyWord);
            Match(TokenType.LBracket);
            if (Current.Type != TokenType.Int && Current.Type != TokenType.Float && Current.Type != TokenType.Comma)
            {
                Match(TokenType.Int);
            }
            while (Current.Type == TokenType.Int || Current.Type == TokenType.Float || Current.Type == TokenType.Comma)
            {
                if (Current.Type == TokenType.Int)
                {
                    numbers.Add(Match(TokenType.Int));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Match(TokenType.Comma);
                    }
                }
                else if (Current.Type == TokenType.Float)
                {
                    numbers.Add(Match(TokenType.Float));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Console.WriteLine(Current.Text);
                        Match(TokenType.Comma);
                    }
                }
                else
                {
                    return new MinMethodSyntax(keyWord, numbers);
                }
            }
            Match(TokenType.RBracket);
            return new MinMethodSyntax(keyWord, numbers);
        }

        /// <summary>
        /// Parses the maximum method
        /// Done by creating the correct values and assigining them to variables
        /// Creates errors by checking the tokens using the match method
        /// Returns an error if they do not match
        /// </summary>
        /// <returns></ instance of the maximum method syntax>
        private ExpressionSyntax ParseMaxMethod()
        {
            List<Token> numbers = new List<Token>();
            var keyWord = Match(TokenType.MaxKeyWord);
            Match(TokenType.LBracket);
            if (Current.Type != TokenType.Int && Current.Type != TokenType.Float && Current.Type != TokenType.Comma)
            {
                Match(TokenType.Int);
            }
            while (Current.Type == TokenType.Int || Current.Type == TokenType.Float || Current.Type == TokenType.Comma)
            {
                if (Current.Type == TokenType.Int)
                {
                    numbers.Add(Match(TokenType.Int));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Match(TokenType.Comma);
                    }
                }
                else if (Current.Type == TokenType.Float)
                {
                    numbers.Add(Match(TokenType.Float));
                    if (Peek(1).Type == TokenType.Int || Peek(1).Type == TokenType.Float && Peek(1).Type != TokenType.RBrace)
                    {
                        Console.WriteLine(Current.Text);
                        Match(TokenType.Comma);
                    }
                }
                else
                {
                    return new MaxMethodSyntax(keyWord, numbers);
                }
            }
            Match(TokenType.RBracket);
            return new MaxMethodSyntax(keyWord, numbers);
        }

        // just returns an instance of the name expression
        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = NextToken();
            return new NameExpressionSyntax(identifierToken);
        }

        // just returns an instance of the float expression
        private ExpressionSyntax ParseLiteralFloatExpression()
        {
            var floatToken = Match(TokenType.Float);
            return new FloatExpressionSyntax(floatToken);
        }

        // just returns an instance of the literal expression
        private ExpressionSyntax ParseLiteralExpression()
        {
            var numberToken = Match(TokenType.Int);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}
