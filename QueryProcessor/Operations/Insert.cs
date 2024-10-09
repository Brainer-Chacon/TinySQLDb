using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(string tableName, string values)
        {
            // Validate table existence
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            // Parse and validate the values
            var parsedValues = ParseValues(values);
            if (parsedValues == null || parsedValues.Length == 0)
            {
                return OperationStatus.InvalidValues;
            }

            // Insert values into the table
            return Store.GetInstance().InsertIntoTable(tableName, parsedValues);
        }

        private string[]? ParseValues(string values)
        {
            // Split the values by comma and return as an array
            return values.Split(',').Select(v => v.Trim()).ToArray();
        }
    }
}
