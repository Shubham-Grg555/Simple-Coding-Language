using Coding_Language.Text;
using System.Collections.Immutable;
using static Coding_Language.Syntax.Token;
using static System.Net.Mime.MediaTypeNames;

namespace Coding_Language.Syntax
{
    // syntax tree to get the information of the input e.g error, expression syntax and end of file token
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();
            Text = text;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        // parses the source text and creates the whole syntax tree
        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        /// <summary>
        /// Used to test if tokens are being parsed correctly
        /// Done by getting the tokens ot parse
        /// </summary>
        /// <param name="text"></ input from user>
        /// <returns></ all tokens that can be parsed to test>
        public static IEnumerable<Token> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }

        /// <summary>
        /// Used to test parser and lexer tokens
        /// Done by having lexer get the tokens from here and parser
        /// As it runs this method get tokens to parse
        /// </summary>
        /// <param name="text"></ input from user>
        /// <returns></all tokens that can be lexed to test>
        public static IEnumerable<Token> ParseTokens(SourceText text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.NextTokens();
                if (token.Type == TokenType.EndOfFile)
                {
                    break;
                }
                yield return token;
            }
        }
    }

}
