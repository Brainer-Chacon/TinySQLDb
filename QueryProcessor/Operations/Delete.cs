using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        internal OperationStatus Execute(string tableName, string condition)
        {
            // Validate table existence
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            // Perform the delete operation
            return Store.GetInstance().DeleteFromTable(tableName, condition);
        }
    }
}
