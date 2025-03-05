using System.Security.AccessControl;

namespace Test
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void Parser_BinaryExpression_Precedence(TokenType op1, TokenType op2)
        {
            var op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            var op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
            var op1Text = SyntaxFacts.GetText(op1);
            var op2Text = SyntaxFacts.GetText(op2);
            var text = "a " + op1Text + " b " + op2Text + " c";
            var expression = ParseExpression(text);

            if (op1Precedence >= op2Precedence) // does a op1 b calc, then op2 c
            {
                using (var e = new AssertingEnuemrator(expression))
                {
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "a");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "c");
                }
            }
            else      // does b op2 c, then op1 a
            {
                using (var e = new AssertingEnuemrator(expression))
                {
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "a");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "c");

                }
            }
        }
        
        /// <summary>
        /// Gets the binary operators to test
        /// </summary>
        /// <returns></ two binary operators to test>
        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (var op1 in SyntaxFacts.GetBinaryOperatorTypes())
            {
                foreach (var op2 in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object [] { op1, op2 };
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void Parser_UnaryExpression_Precedence(TokenType unaryType1, TokenType binaryType)
        {
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unaryType1);
            var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binaryType);
            var unaryText = SyntaxFacts.GetText(unaryType1);
            var binaryText = SyntaxFacts.GetText(binaryType);
            var text = unaryText + " a " + binaryText + " b";
            ExpressionSyntax expression = ParseExpression(text);

            if (unaryPrecedence >= binaryPrecedence)
            {
                using (var e = new AssertingEnuemrator(expression))
                {
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.UnaryExpression);
                    e.AssertToken(unaryType1, unaryText);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "a");
                    e.AssertToken(binaryType, binaryText);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "b");
                }
            }
            else
            {
                using (var e = new AssertingEnuemrator(expression))
                {
                    e.AssertNode(TokenType.UnaryExpression);
                    e.AssertToken(unaryType1, unaryText);
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "a");
                    e.AssertToken(binaryType, binaryText);
                    e.AssertNode(TokenType.BinaryExpression);
                    e.AssertNode(TokenType.NameExpression);
                    e.AssertToken(TokenType.Identifier, "b");
                }
            }
        }

        private static ExpressionSyntax ParseExpression(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var root = syntaxTree.Root;
            var statement = root.Statement;
            return Assert.IsType<ExpressionStatementSyntax>(statement).Expression;
        }

        /// <summary>
        /// Gets the unary and binary operators to test
        /// </summary>
        /// <returns></ a unary and binary operators to test>
        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (var unary in SyntaxFacts.GetUnaryOperatorTypes())
            {
                foreach (var binary in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { unary, binary };
                }
            }
        }
    }

}