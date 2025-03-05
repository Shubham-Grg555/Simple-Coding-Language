namespace Test
{

    public class LexerTests
    {

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(TokenType type, string input)
        {
            var tokens = SyntaxTree.ParseTokens(input);

            var token = Assert.Single(tokens);
            Assert.Equal(type, token.Type);
            Assert.Equal(input, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokensPairsData))]
        public void Lexer_Lexes_TokenPairs(TokenType type1, string input1,
            TokenType type2, string input2)
        {
            var input = input1 + input2;
            var tokens = SyntaxTree.ParseTokens(input).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(tokens[0].Type, type1);
            Assert.Equal(tokens[0].Text, input1);
            Assert.Equal(tokens[1].Type, type2);
            Assert.Equal(tokens[1].Text, input2);
        }

        [Theory]
        [MemberData(nameof(GetTokensPairsWithSeparatorData))]
        public void Lexer_Lexes_TokenPairsWithSeparator(TokenType type1, string input1,
            TokenType separatorType, string separatorInput,
            TokenType type2, string input2)
        {
            var input = input1 + separatorInput + input2;
            var tokens = SyntaxTree.ParseTokens(input).ToArray();

            Assert.Equal(3, tokens.Length);
            Assert.Equal(tokens[0].Type, type1);
            Assert.Equal(tokens[0].Text, input1);
            Assert.Equal(tokens[1].Type, separatorType);
            Assert.Equal(tokens[1].Text, separatorInput);
            Assert.Equal(tokens[2].Type, type2);
            Assert.Equal(tokens[2].Text, input2);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens().Concat(GetSeparators()))
            {
                yield return new object[] { t.type, t.input };
            }
        }

        public static IEnumerable<object[]> GetTokensPairsData()
        {
            foreach (var t in GetTokenPairs())
            {
                yield return new object[] { t.type1, t.input1, t.type2, t.input2 };
            }
        }

        public static IEnumerable<object[]> GetTokensPairsWithSeparatorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
            {
                yield return new object[] { t.type1, t.input1, t.separatorType, t.separatorInput, t.type2, t.input2 };
            }
        }

        private static IEnumerable<(TokenType type, string input)> GetTokens()
        {
            return new[]
            {
            (TokenType.Add, "+"),
            (TokenType.Subtract, "-"),
            (TokenType.Multiply, "*"),
            (TokenType.Divide, "/"),
            (TokenType.LBracket, "("),
            (TokenType.RBracket, ")"),
            (TokenType.Int, "1"),
            (TokenType.Int, "348"),
            (TokenType.Float, "2.1"),
            (TokenType.Modulo, "%"),
            (TokenType.Identifier, "a"),
            (TokenType.Identifier, "abc"),
            (TokenType.Equal, "="),
            (TokenType.NOT, "!"),
            (TokenType.LogicalAND, "&&"),
            (TokenType.LogicalOR, "||"),
            (TokenType.TrueEquals, "=="),
            (TokenType.NOTEquals, "!="),
            (TokenType.TrueKeyWord, "true"),
            (TokenType.FalseKeyWord, "false"),
            };
        }

        private static IEnumerable<(TokenType type, string input)> GetSeparators()
        {
            return new[]
            {
            (TokenType.WhiteSpace, " "),
            (TokenType.WhiteSpace, "   "),
            (TokenType.WhiteSpace, "\r"),
            (TokenType.WhiteSpace, "\n"),
            (TokenType.WhiteSpace, "\r\n"),
            };
        }

        private static bool RequiresSeparator(TokenType tokenType1, TokenType tokenType2)
        {
            var type1KeyWordCheck = tokenType1.ToString().EndsWith("KeyWord");
            var type2KeyWordCheck = tokenType2.ToString().EndsWith("KeyWord");
            if (type1KeyWordCheck && type2KeyWordCheck)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Identifier && tokenType2 == TokenType.Identifier)
            {
                return true;
            }
            else if (type1KeyWordCheck && tokenType2 == TokenType.Identifier)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Identifier && type2KeyWordCheck)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Int && tokenType2 == TokenType.Int)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Float && tokenType2 == TokenType.Float)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Int && tokenType2 == TokenType.Float)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Float && tokenType2 == TokenType.Int)
            {
                return true;
            }
            else if (tokenType1 == TokenType.NOT && tokenType2 == TokenType.Equal)
            {
                return true;
            }
            else if (tokenType1 == TokenType.NOT && tokenType2 == TokenType.TrueEquals)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Equal && tokenType2 == TokenType.Equal)
            {
                return true;
            }
            else if (tokenType1 == TokenType.Equal && tokenType2 == TokenType.TrueEquals)
            {
                return true;
            }
            else if (tokenType1 == TokenType.TrueEquals && tokenType1 == TokenType.Equal)
            {
                return true;
            }
            else if (tokenType1 == TokenType.LessThan && tokenType1 == TokenType.Equal)
            {
                return true;
            }
            else if (tokenType1 == TokenType.LessThan && tokenType1 == TokenType.TrueEquals)
            {
                return true;
            }
            else if (tokenType1 == TokenType.GreaterThan && tokenType1 == TokenType.Equal)
            {
                return true;
            }
            else if (tokenType1 == TokenType.GreaterThan && tokenType1 == TokenType.TrueEquals)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static IEnumerable<(TokenType type1, string input1, TokenType type2, string input2)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.type, t2.type))
                    {
                        yield return (t1.type, t1.input, t2.type, t2.input);
                    }
                }
            }
        }

        private static IEnumerable<(TokenType type1, string input1,
            TokenType separatorType, string separatorInput,
            TokenType type2, string input2)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.type, t2.type))
                    {
                        foreach (var s in GetSeparators())
                        {
                            yield return (t1.type, t1.input, s.type, s.input, t2.type, t2.input);
                        }
                    }
                }
            }
        }
    }
}