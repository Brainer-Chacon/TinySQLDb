// Insert.cs
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{

    /// Clase para manejar operaciones de inserción de registros en una tabla.

    internal class Insert
    {

        /// Ejecuta la operación de inserción en la tabla especificada.

        internal OperationStatus Execute(string tableName, string values)
        {
            // Validar existencia de la tabla
            if (!Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableNotFound; // Devuelve estado de no encontrado
            }

            // Parsear y validar los valores
            var parsedValues = ParseValues(values);
            if (parsedValues == null || parsedValues.Length == 0)
            {
                return OperationStatus.InvalidValues; // Verifica que este valor esté en la enumeración
            }

            // Inserta valores en la tabla
            return Store.GetInstance().InsertIntoTable(tableName, parsedValues);
        }


        /// Parsea los valores a insertar.

        private string[]? ParseValues(string values)
        {
            // Divide los valores por comas y devuelve como un array
            return values.Split(',').Select(v => v.Trim()).ToArray(); // Filtra valores vacíos
        }
    }
}