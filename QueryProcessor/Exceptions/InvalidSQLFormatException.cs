using System;

namespace QueryProcessor.Exceptions
{
    public class InvalidSQLFormatException : Exception
    {
        public InvalidSQLFormatException() : base("The SQL format is invalid.")
        {
        }
    }
}
