using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Exceptions
{
    public class InvalidSQLFormatException : Exception
    {
        public InvalidSQLFormatException() : base("Invalid SQL format.")
        {
        }
    }
}