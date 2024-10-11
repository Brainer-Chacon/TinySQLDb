namespace Entities
{
    public enum OperationStatus
    {
        Success, // Operación exitosa
        TableNotFound, // Tabla no encontrada
        InvalidColumnDefinitions, // Definiciones de columnas inválidas
        Error, // Error genérico
        Warning, // Advertencia
        InvalidTableName, // Nombre de tabla inválido
        TableAlreadyExists, // La tabla ya existe
        InvalidSetClause, // Cláusula SET inválida
        InvalidValues, // Valores inválidos
    }
}
}