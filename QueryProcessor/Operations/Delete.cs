// Delete.cs
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{

    /// Clase para manejar operaciones de eliminación de registros en una tabla.

    internal class Delete
    {

        /// Ejecuta la operación de eliminación en la tabla especificada.

        internal OperationStatus Execute(string tableName, string condition)
        {
            // Validar existencia de la tabla
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound; // Devuelve estado de no encontrado
            }

            // Realiza la operación de eliminación
            return Store.GetInstance().DeleteFromTable(tableName, condition);
        }
    }
}