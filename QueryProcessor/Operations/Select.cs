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
        public OperationStatus Execute()
        {
            // This is only doing the query but not returning results.
            return Store.GetInstance().Select();
        internal OperationStatus Execute(string tableName, string condition = null)
        {
            // Validate table existence
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            // Read data from the table and apply conditions if provided
            return Store.GetInstance().Select(tableName, condition);
        }

    }
    
    }

