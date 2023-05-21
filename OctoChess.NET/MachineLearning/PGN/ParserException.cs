using System;

namespace MachineLearning.PGN
{
    public class ParserException : Exception
    {
        public ParserException() { }

        public ParserException(string message)
            : base(message) { }
    }
}
