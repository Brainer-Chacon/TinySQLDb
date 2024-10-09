using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            /// The following is example code. Parser should be called
            /// on the sentence to understand and process what is requested
            if (sentence.StartsWith("CREATE TABLE"))
            {
                return new CreateTable().Execute();
            }
            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute();
            // Determine the type of SQL statement and execute the corresponding operation
            if (sentence.StartsWith("CREATE TABLE"))
            {
                // Extract table name and column definitions
                var parts = sentence.Substring(12).Trim().Split(" AS ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string columnDefinitions = parts[1].Trim();
                return new CreateTable().Execute(tableName, columnDefinitions);
            }
            else if (sentence.StartsWith("INSERT INTO"))
            {
                // Extract table name and values
                var parts = sentence.Substring(12).Trim().Split(" VALUES ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim().Split(" ")[2].Trim(); // Get table name
                string values = parts[1].Trim('(', ')');
                return new Insert().Execute(tableName, values);
            }
            else if (sentence.StartsWith("SELECT"))
            {
                // Extract table name and condition
                var parts = sentence.Substring(6).Trim().Split(" FROM ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string condition = parts.Length > 1 ? parts[1].Trim() : null;
                string tableName = parts[0].Trim();
                return new Select().Execute(tableName, condition);
            }
            else if (sentence.StartsWith("UPDATE"))
            {
                // Extract table name, set clause, and condition
                var parts = sentence.Substring(7).Trim().Split(" SET ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string setClause = parts[1].Trim();
                string condition = ""; // You might want to extract the condition as well
                if (setClause.Contains(" WHERE "))
                {
                    var setParts = setClause.Split(" WHERE ");
                    setClause = setParts[0].Trim();
                    condition = setParts[1].Trim();
                }
                return new Update().Execute(tableName, setClause, condition);
            }
            else if (sentence.StartsWith("DELETE"))
            {
                // Extract table name and condition
                var parts = sentence.Substring(6).Trim().Split(" FROM ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string condition = parts.Length > 1 ? parts[1].Trim() : null;
                return new Delete().Execute(tableName, condition);
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
}