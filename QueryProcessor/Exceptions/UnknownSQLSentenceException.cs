using System;

namespace QueryProcessor.Exceptions
{
    public class UnknownSQLSentenceException : Exception
    {
        public UnknownSQLSentenceException() : base("The SQL sentence is unknown.")
        {
        }
    }
}
