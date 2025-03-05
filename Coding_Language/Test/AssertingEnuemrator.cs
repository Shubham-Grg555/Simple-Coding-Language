namespace Test
{
    internal sealed class AssertingEnuemrator : IDisposable
    {
        private IEnumerator<SyntaxNode> enumerator;
        private bool hasErrors;
        public AssertingEnuemrator(SyntaxNode node)
        {
            enumerator = Flatten(node).GetEnumerator();
        }

        // runs if the test fails
        private bool markFailed()
        {
            hasErrors = true;
            return false;
        }

        /// <summary>
        /// remove unecessary things like _enumerator, if there are no errors in the test
        /// </summary>
        public void Dispose()
        {
            if (!hasErrors)
            {
                Assert.False(enumerator.MoveNext());
            }
            enumerator.Dispose();
        }

        /// <summary>
        /// flatten the tree structure by using a stack
        /// </summary>
        /// <param name="node"></ syntax node of the tree>
        /// <returns></ >
        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                yield return n;

                // reverse as it is a stack
                foreach (var child in n.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }



        /// <summary>
        /// Checks if the current token is the expected token and passes the test if it is
        /// </summary>
        /// <param name="type"></ current token type>
        /// <param name="text"></ current text information>
        public void AssertToken(TokenType type, string text)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(type, enumerator.Current.Type);
                var token = Assert.IsType<Token>(enumerator.Current);
                Assert.Equal(text, token.Text);

            }
            catch when (markFailed())
            {
                hasErrors = true;
                throw;
            }
        }

        /// <summary>
        /// Checks if the current token is the expected node and passes the test if it is
        /// </summary>
        /// <param name="type"></ type of node>
        public void AssertNode(TokenType type)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(type, enumerator.Current.Type);
                Assert.IsNotType<Token>(enumerator.Current);
            }
            catch when (markFailed())
            {
                throw;
            }
        }
    }
}