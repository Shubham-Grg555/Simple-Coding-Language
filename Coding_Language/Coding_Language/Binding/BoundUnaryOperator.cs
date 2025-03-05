using Coding_Language.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Coding_Language.Syntax.Token;

namespace Coding_Language.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(TokenType tokenType, BoundUnaryOperatorType type, Type operandType)
            : this(tokenType, type, operandType, operandType)
        {
        }

        private BoundUnaryOperator(TokenType tokenType, BoundUnaryOperatorType type, Type operandType,
            Type resultType)
        {
            TokenType = tokenType;
            Type = type;
            OperandType = operandType;
            ResultType = resultType;
        }

        public TokenType TokenType { get; }
        public BoundUnaryOperatorType Type { get; }
        public Type OperandType { get; }
        public Type ResultType { get; }

        private static BoundUnaryOperator[] _operators =
        {
            // bool unary operators
            new BoundUnaryOperator(TokenType.NOT, BoundUnaryOperatorType.LogicalReverse, typeof(bool)),

            // int unary operators
            new BoundUnaryOperator(TokenType.Add, BoundUnaryOperatorType.Identity, typeof(int)),
            new BoundUnaryOperator(TokenType.Subtract, BoundUnaryOperatorType.Negation, typeof(int)),

            // float unary operators
            new BoundUnaryOperator(TokenType.Add, BoundUnaryOperatorType.Identity, typeof(float)),
            new BoundUnaryOperator(TokenType.Subtract, BoundUnaryOperatorType.Negation, typeof(float))
        };


        /// <summary>
        /// Used to bind operators together if it meets the if statement criteria, else it returns null
        /// </summary>
        /// <param name="tokenType"></ used to get the token type>
        /// <param name="operandType"></ used to get the operand type>
        /// <returns></ op if the if statement conditions are met>
        /// <returns></ null if it is not met and creates an error in the method that calls it>
        public static BoundUnaryOperator Bind(TokenType tokenType, Type operandType)
        {
            // Uses a foreach loop to return the currect operator if it meets the if statement conditions
            foreach (var op in _operators)
            {
                if (op.TokenType == tokenType && op.OperandType == operandType)
                {
                    return op;
                }
            }
            return null!;
        }
    }
}
