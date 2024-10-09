using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Update
    {
        internal OperationStatus Execute(string tableName, string setClause, string condition)
        {
            // Validate table existence
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            // Validate the SET clause and condition
            var setValues = ParseSetClause(setClause);
            if (setValues == null || setValues.Length == 0)
            {
                return OperationStatus.InvalidSetClause;
            }

            // Perform the update operation
            return Store.GetInstance().UpdateTable(tableName, setValues, condition);
        }

        private string[]? ParseSetClause(string setClause)
        {
            // Split the SET clause into an array and validate
            return setClause.Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToArray();
        }
    }
}
