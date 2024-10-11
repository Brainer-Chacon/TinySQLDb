// Update.cs
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{

    /// Clase para manejar operaciones de actualización de registros en una tabla.

    internal class Update
    {
 
        /// Ejecuta la operación de actualización en la tabla especificada.

        internal OperationStatus Execute(string tableName, string setClause, string condition)
        {
            // Validar existencia de la tabla
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound; // Devuelve estado de no encontrado
            }

            // Validar la cláusula SET y condición
            var setValues = ParseSetClause(setClause);
            if (setValues == null || setValues.Length == 0)
            {
                return OperationStatus.InvalidSetClause; // Verifica que este valor esté en la enumeración
            }

            // Realiza la operación de actualización
            return Store.GetInstance().UpdateTable(tableName, setValues, condition);
        }

        /// Parsea la cláusula SET.

        private string[]? ParseSetClause(string setClause)
        {
            // Divide la cláusula SET en un array y valida
            return setClause.Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToArray(); // Devuelve los valores válidos
        }
    }
}