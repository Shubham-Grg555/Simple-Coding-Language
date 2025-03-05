using Coding_Language.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Binding
{
    internal sealed class BoundBinaryOperator
    {
        // multiple constructors made, so there can be different amount of parameters
        private BoundBinaryOperator(TokenType tokenType, BoundBinaryOperatorType Type, Type type)
            : this(tokenType, Type, type, type, type)
        {
        }

        private BoundBinaryOperator(TokenType tokenType, BoundBinaryOperatorType Type, Type operandType, Type resultType)
    : this(tokenType, Type, operandType, operandType, resultType)
        {
        }

        // all binary operators initalised by this constructor
        private BoundBinaryOperator(TokenType tokenType, BoundBinaryOperatorType opType, Type leftType,
            Type rightType, Type resultType)
        {
            TokenType = tokenType;
            OpType = opType;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public TokenType TokenType { get; }
        public BoundBinaryOperatorType OpType { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }

        // binary operators and the types it can be e.g int then int meaning int + int
        private static readonly BoundBinaryOperator[] _operators =
        {
            // type int calculation
            new BoundBinaryOperator(TokenType.Add, BoundBinaryOperatorType.Add, typeof(int)),
            new BoundBinaryOperator(TokenType.Subtract, BoundBinaryOperatorType.Subtract, typeof(int)),
            new BoundBinaryOperator(TokenType.Multiply, BoundBinaryOperatorType.Multiply, typeof(int)),
            new BoundBinaryOperator(TokenType.Divide, BoundBinaryOperatorType.Divide, typeof(int)),
            new BoundBinaryOperator (TokenType.Modulo, BoundBinaryOperatorType.Modulo, typeof(int)),
            new BoundBinaryOperator (TokenType.Power, BoundBinaryOperatorType.Power, typeof(int)),

            // type float calculation
            new BoundBinaryOperator(TokenType.Add, BoundBinaryOperatorType.Add, typeof(float)),
            new BoundBinaryOperator(TokenType.Subtract, BoundBinaryOperatorType.Subtract, typeof(float)),
            new BoundBinaryOperator(TokenType.Multiply, BoundBinaryOperatorType.Multiply, typeof(float)),
            new BoundBinaryOperator(TokenType.Divide, BoundBinaryOperatorType.Divide, typeof(float)),
            new BoundBinaryOperator (TokenType.Modulo, BoundBinaryOperatorType.Modulo, typeof(float)),
            new BoundBinaryOperator (TokenType.Power, BoundBinaryOperatorType.Power, typeof(float)),

            // type float then int calculation
            new BoundBinaryOperator(TokenType.Add, BoundBinaryOperatorType.Add, typeof(float), typeof(int), typeof(int)),
            new BoundBinaryOperator(TokenType.Subtract, BoundBinaryOperatorType.Subtract, typeof(float), typeof(int), typeof(int)),
            new BoundBinaryOperator(TokenType.Multiply, BoundBinaryOperatorType.Multiply, typeof(float), typeof(int), typeof(int)),
            new BoundBinaryOperator(TokenType.Divide, BoundBinaryOperatorType.Divide, typeof(float), typeof(int), typeof(int)),
            new BoundBinaryOperator (TokenType.Modulo, BoundBinaryOperatorType.Modulo, typeof(float), typeof(int), typeof(int)),
            new BoundBinaryOperator (TokenType.Power, BoundBinaryOperatorType.Power, typeof(float), typeof(int), typeof(int)),

            // type int then float calculation
            new BoundBinaryOperator(TokenType.Add, BoundBinaryOperatorType.Add, typeof(int), typeof(float), typeof(int)),
            new BoundBinaryOperator(TokenType.Subtract, BoundBinaryOperatorType.Subtract, typeof(int), typeof(float), typeof(int)),
            new BoundBinaryOperator(TokenType.Multiply, BoundBinaryOperatorType.Multiply, typeof(int), typeof(float), typeof(int)),
            new BoundBinaryOperator(TokenType.Divide, BoundBinaryOperatorType.Divide, typeof(int), typeof(float), typeof(int)),
            new BoundBinaryOperator (TokenType.Modulo, BoundBinaryOperatorType.Modulo, typeof(int), typeof(float), typeof(int)),
            new BoundBinaryOperator (TokenType.Power, BoundBinaryOperatorType.Power, typeof(int), typeof(float), typeof(int)),

            // type int for bool operators
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(int), typeof(int), typeof(bool)),
            new BoundBinaryOperator (TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.LessThan, BoundBinaryOperatorType.LessThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator (TokenType.LessThanOrEquals, BoundBinaryOperatorType.LessThanOrEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThan, BoundBinaryOperatorType.GreaterThan, typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThanOrEquals, BoundBinaryOperatorType.GreaterThanOrEquals, typeof(int), typeof(bool)),

            // type bool for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(bool)),

            //type bool then int for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(bool), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(bool), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(bool), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(bool), typeof(int), typeof(bool)),

            // type int then bool for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(int), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(int), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(int), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(int), typeof(bool), typeof(bool)),

            // type float for bool operators
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(float), typeof(bool)),
            new BoundBinaryOperator (TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.LessThan, BoundBinaryOperatorType.LessThan, typeof(float), typeof(bool)),
            new BoundBinaryOperator (TokenType.LessThanOrEquals, BoundBinaryOperatorType.LessThanOrEquals, typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThan, BoundBinaryOperatorType.GreaterThan, typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThanOrEquals, BoundBinaryOperatorType.GreaterThanOrEquals, typeof(float), typeof(bool)),

            // type bool then float for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(bool), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(bool), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(bool), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(bool), typeof(float), typeof(bool)),

            // type float then bool for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(float), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(float), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(float), typeof(bool), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(float), typeof(bool), typeof(bool)),

            // type int then float for bool operators
            new BoundBinaryOperator(TokenType.LogicalAND, BoundBinaryOperatorType.LogicalAND, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.LogicalOR, BoundBinaryOperatorType.LogicalOR, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.LessThan, BoundBinaryOperatorType.LessThan, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator (TokenType.LessThanOrEquals, BoundBinaryOperatorType.LessThanOrEquals, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThan, BoundBinaryOperatorType.GreaterThan, typeof(int), typeof(float), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThanOrEquals, BoundBinaryOperatorType.GreaterThanOrEquals, typeof(int), typeof(float), typeof(bool)),

            // type float then int for bool operators
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator (TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.TrueEquals, BoundBinaryOperatorType.TrueEquals, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator (TokenType.NOTEquals, BoundBinaryOperatorType.NOTEquals, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.LessThan, BoundBinaryOperatorType.LessThan, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator (TokenType.LessThanOrEquals, BoundBinaryOperatorType.LessThanOrEquals, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThan, BoundBinaryOperatorType.GreaterThan, typeof(float), typeof(int), typeof(bool)),
            new BoundBinaryOperator(TokenType.GreaterThanOrEquals, BoundBinaryOperatorType.GreaterThanOrEquals, typeof(float), typeof(int), typeof(bool)),
        };

        /// <summary>
        /// Used to bind operators together
        /// Creates null if an invalid token was binded
        /// </summary>
        /// <param name="tokenType"></ Used to get the token type>
        /// <param name="leftType"></ used to get the variable type on the left>
        /// <param name="rightType"></ used to get the variable type on the right>
        /// <returns></ op if the if statement conditions are met>
        /// <returns></ null if it is not met and creates an error in the method that calls it>
        public static BoundBinaryOperator Bind(TokenType tokenType, Type leftType, Type rightType)
        {
            foreach (var op in _operators)
            {
                bool isValidBinaryOp = op.TokenType == tokenType && op.LeftType == leftType && op.RightType == rightType;
                if (isValidBinaryOp)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
