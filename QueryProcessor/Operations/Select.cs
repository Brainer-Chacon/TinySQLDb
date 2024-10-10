using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        // Execute method without parameters - modify this method to include default parameters
        public OperationStatus Execute(string tableName = "", string condition = "")
        {
            // Validate table existence if tableName is provided
            if (!string.IsNullOrEmpty(tableName) && !Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            // If no table name is provided, you might want to return an error or handle it appropriately
            if (string.IsNullOrEmpty(tableName))
            {
                return OperationStatus.InvalidTableName; // Assumes this status exists
            }

            // Read data from the table and apply conditions if provided
            return Store.GetInstance().Select(tableName, condition);
        }
    }
}

