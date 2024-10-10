using System;

namespace QueryProcessor.Exceptions
{
    public class UnknownSQLSentenceException : Exception
    {
        public UnknownSQLSentenceException() : base("Unknown SQL sentence.")
        {
        }
    }
}