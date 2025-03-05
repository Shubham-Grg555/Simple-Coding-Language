using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1.0)]
        [InlineData("-1", -1.0)]
        [InlineData("24 + 7", 31.0)]
        [InlineData("14 - 3", 11.0)]
        [InlineData("1 * 2", 2.0)]
        [InlineData("5 / 2", 2.5)]
        [InlineData("(10)", 10)]
        [InlineData("5 % 2", 1.0)]
        [InlineData("5 ^ 2", 25.0)]
        [InlineData("4 ^ 0.5", 2.0)]
        [InlineData("9 < 25", true)]
        [InlineData("58 < 3", false)]
        [InlineData("4 <= 4", true)]
        [InlineData("2 <= 54", true)]
        [InlineData("17 <= 8", false)]
        [InlineData("29 > 5", true)]
        [InlineData("20 > 87", false)]
        [InlineData("24 >= 24", true)]
        [InlineData("55 >= 7", true)]
        [InlineData("11 >= 25", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var x = 0 if x == 0 { x = 10 } x }", 10)]
        [InlineData("{ var x = 0 if x == 7 { x = 20 } x }", 0)]
        [InlineData("{ var x = 5 if x == 0 { x = 10 } else { x = 8 } x }", 8)]
        [InlineData("{ var x = 4 if x == 4 { x = 12 } else { x = 5 } x }", 12)]
        [InlineData("{ var i = 10 var result = 0 while i > 0 { result = result + i i = i - 1} result }", 55.0)]
        [InlineData("{ var result = 0 for i = 1 to 10 { result = result + i } result }", 55.0)]
        [InlineData("avg(1, 3)", 2.0)]
        [InlineData("avg(2.5, 2.6)", 2.55)]
        [InlineData("avg(3, 5.5)", 4.25)]
        [InlineData("min(1, 3)", 1.0)]
        [InlineData("min(3, 1)", 1.0)]
        [InlineData("min(2.5, 2.6)", 2.5)]
        [InlineData("min(2.6, 2.5)", 2.5)]
        [InlineData("min(3, 5.5)", 3.0)]
        [InlineData("min(5.5, 3)", 3.0)]
        [InlineData("max(1, 3)", 3.0)]
        [InlineData("max(3, 1)", 3.0)]
        [InlineData("max(2.5, 2.6)", 2.6)]
        [InlineData("max(2.6, 2.5)", 2.6)]
        [InlineData("max(5.5, 3)", 5.5)]
        [InlineData("max(3, 5.5)", 5.5)]
        public void Evaluator_Gives_Result(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            var text = @"
                {
                [)][]
            ";

            var diagnostics = @"
                Unexpected token: RBracket expected: Int
                Unexpected token: EndOfFile expected: RBrace
                ";
            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            var text = @"
                {
                    var x = 0
                    if [10]
                        {
                           x = 10
                        }
                }
            ";

            var diagnostics = @"
                Cannot convert type: System.Int32 to type: System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            var text = @"
                {
                    var x = 0
                    while [10]
                        x = 10
                }
            ";

            var diagnostics = @"
                Cannot convert type: System.Int32 to type: System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            var text = @"
                {
                    var result = 0
                    for i = [false] to 5
                        result = result + i
                }
            ";

            var diagnostics = @"
                Cannot convert type: System.Boolean to type: System.Int32
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            var text = @"
                {
                    var result = 0
                    for i = 1 to [true]
                        result = result + i
                }
            ";

            var diagnostics = @"
                Cannot convert type: System.Boolean to type: System.Int32
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 10
                    }
                    var [x] = 5
                }
            ";

            var diagnostics = @"
                Variable: x is already declared
            ";
            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_Undefined_Variable()
        {
            var text = @"[x] * 10";

            var diagnostics = @"
                Variable name: x does not exit
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_NothingInserted()
        {
            var text = @"[]";

            var diagnostics = @"
                Unexpected token: EndOfFile expected: Int
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_UnaryExpression_Reports_Undefined()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator: + is not defined for type: System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_BinaryExpression_Reports_Undefined()
        {
            var text = @"10 [*] false";

            var diagnostics = @"
                Binary operator: * is not defined for type: System.Int32 and System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotAssign()
        {
            var text = @"
                {
                    let x = 10
                    [x] = 0
                }
            ";

            var diagnostics = @"
                Variable is read only and has already been assigned: x
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotConvert()
        {
            var text = @"
                {
                    var x = 10
                    x = [true]
                }
            ";

            var diagnostics = @"
                Cannot convert type: System.Boolean to type: System.Int32
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
            {
                throw new Exception("Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
