using System.Security.Cryptography.X509Certificates;

namespace Test
{
    public class SyntaxFactsTests
    {
        [Theory]
        [MemberData(nameof(GetTokenTypeData))]
        public void SyntaxFacts_GetText_RoundTrips(TokenType type)
        {
            var text = SyntaxFacts.GetText(type);
            if (text == null)
            {
                return;
            }
            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(type, token.Type);
            Assert.Equal(text, token.Text);
        }

        /// <summary>
        /// Test used to get the token type and the data
        /// </summary>
        /// <returns></ an array of tokens>
        public static IEnumerable<object[]> GetTokenTypeData()
        {
            var types = (TokenType[]) Enum.GetValues(typeof(TokenType));
            foreach (var type in types)
            {
                yield return new object[] { type };
            }
        }
    }
}