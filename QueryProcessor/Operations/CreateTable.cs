using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute()
        {
            return Store.GetInstance().CreateTable();
        internal OperationStatus Execute(string tableName, string columnDefinitions)
        {
            // Validate table name
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return OperationStatus.InvalidTableName;
            }

            // Validate column definitions
            if (string.IsNullOrWhiteSpace(columnDefinitions))
            {
                return OperationStatus.InvalidColumnDefinitions;
            }

            // Check if table already exists in the catalog
            if (Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableAlreadyExists;
            }

            // Create the table in the database
            var createStatus = Store.GetInstance().CreateTable(tableName, columnDefinitions);

            // Optionally, log this operation in the system catalog
            if (createStatus == OperationStatus.Success)
            {
                Store.GetInstance().LogTableCreation(tableName, columnDefinitions);
            }

            return createStatus;

        }
    }
}
