// Select.cs
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{

    /// Clase para manejar operaciones de selección de registros en una tabla.

    internal class Select
    {

        /// Ejecuta la operación de selección sobre la tabla especificada.
 
        public OperationStatus Execute(string tableName = "", string condition = "")
        {
            // Validar existencia de la tabla si se proporciona el nombre
            if (!string.IsNullOrEmpty(tableName) && !Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound; // Devuelve estado de no encontrado
            }

            // Si no se proporciona un nombre de tabla, se podría devolver un error o manejarlo apropiadamente
            if (string.IsNullOrEmpty(tableName))
            {
                return OperationStatus.InvalidTableName; // Asume que este estado existe
            }

            // Leer datos de la tabla y aplicar condiciones si se proporciona
            return Store.GetInstance().Select(tableName, condition);
        }
    }
}