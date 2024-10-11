// CreateTable.cs
using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{

    /// Clase para manejar operaciones de creación de tablas.

    internal class CreateTable
    {

        /// Ejecuta la operación de creación de una tabla con el nombre y definiciones de columnas.

        internal OperationStatus Execute(string tableName, string columnDefinitions)
        {
            // Validar nombre de la tabla
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return OperationStatus.InvalidTableName; // Devuelve estado de nombre de tabla inválido
            }

            // Validar definiciones de columnas
            if (string.IsNullOrWhiteSpace(columnDefinitions))
            {
                return OperationStatus.InvalidColumnDefinitions; // Devuelve estado de definiciones de columnas inválidas
            }

            // Verificar si la tabla ya existe en el catálogo
            if (Store.GetInstance().TableExists(tableName))
            {
                return OperationStatus.TableAlreadyExists; // Devuelve estado de existencia
            }

            // Crea la tabla en la base de datos
            var createStatus = Store.GetInstance().CreateTable(tableName, columnDefinitions);

            // Opcionalmente, registra esta operación en el catálogo del sistema
            if (createStatus == OperationStatus.Success)
            {
                Store.GetInstance().LogTableCreation(tableName, columnDefinitions);
            }

            return createStatus; // Devuelve el estado de la creación
        }



        internal OperationStatus Execute()
        {
            string defaultTableName = "DefaultTable"; // Asigna un nombre predeterminado o usa uno dinámico
            string defaultColumnDefinitions = "Column1;Column2"; // Definiciones predeterminadas

            return Execute(defaultTableName, defaultColumnDefinitions);
        }
    }
}