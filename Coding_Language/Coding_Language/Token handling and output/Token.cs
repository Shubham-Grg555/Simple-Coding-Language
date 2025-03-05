using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coding_Language.Text;

namespace Coding_Language.Syntax
{
    public sealed partial class Token : SyntaxNode // temproraily making the tokens node as you can pretend that they are the leaves in the binary tree
    {
        public enum TokenType // Token types and their names
        {
            // math
            Add,
            Subtract,
            Multiply,
            Divide,
            LBracket,
            RBracket,
            Int,
            Float,
            Modulo,
            LBrace,
            RBrace,
            Power,

            // tokens
            WhiteSpace,
            EndOfFile,
            Error,
            Identifier,

            // boolean operators
            NOT,
            LogicalAND,
            LogicalOR,
            TrueEquals,
            NOTEquals,
            Equal,
            LessThan,
            LessThanOrEquals,
            GreaterThan,
            GreaterThanOrEquals,
            Comma,

            // key words
            TrueKeyWord,
            FalseKeyWord,
            LetKeyWord,
            VarKeyWord,
            IfKeyWord,
            ElseKeyWord,
            WhileKeyWord,
            ForKeyWord,

            // expressions
            LiteralExpression,
            BinaryExpression,
            BracketExpression,
            UnaryExpression,
            NameExpression,
            AssignmentExpression,
            VariableExpression,
            FloatExpression,

            // Nodes
            CompilationUnit,

            // Statements
            BlockStatement,
            ExpressionStatement,
            VariableDeclaration,
            IfStatement,
            ElseClause,
            WhileStatement,
            ForStatement,
            ToKeyWord,
            AvgKeyWord,

            // Pre built methods
            AvgMethod,
            MinKeyWord,
            MaxKeyWord,
            MinMethod,
            MaxMethod,
        }

        public override TokenType Type { get; }
        public string Text { get; }
        public int Position { get; }
        public object Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text.Length);

        public Token(TokenType type, string text, int position, object value)
        {
            // assigning the token information into public variables so they can be used outside this class
            Type = type;
            Text = text;
            Position = position;
            Value = value;
        }
    }
}
