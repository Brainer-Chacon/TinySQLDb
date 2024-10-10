using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        // Execute method with table name and column definitions
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

            // Check if the table already exists in the catalog
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

        // If you still need a method without parameters, you can add one with different logic
        internal OperationStatus Execute()
        {
            string defaultTableName = "DefaultTable"; // Asigna un nombre predeterminado o usa uno dinámico
            string defaultColumnDefinitions = "Column1;Column2"; // Definiciones predeterminadas

            return Execute(defaultTableName, defaultColumnDefinitions);
        }
    }
}
