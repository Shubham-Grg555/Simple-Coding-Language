namespace Coding_Language.Text
{
    /// <summary>
    /// Used to get information of the error
    /// </summary>
    public struct TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        // creates the correct text spans for each token
        public static TextSpan FromBounds(int start, int end)
        {
            var length = end - start;
            return new TextSpan(start, length);
        }

        public override string ToString() => $"{Start}..{End}";
    }
}
