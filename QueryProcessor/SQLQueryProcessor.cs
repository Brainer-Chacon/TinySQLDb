// SQLQueryProcessor.cs
using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;
using System.IO;

namespace QueryProcessor
{

    /// Procesador para manejar y ejecutar sentencias SQL.

    public class SQLQueryProcessor
    {
        private static string currentDatabase = string.Empty; // Base de datos actual

  
        /// Ejecuta una sentencia SQL dada.

        public static OperationStatus Execute(string sentence)
        {
            if (sentence.StartsWith("CREATE DATABASE"))
            {
                var databaseName = sentence.Substring(17).Trim();
                return CreateDatabase(databaseName);
            }
            else if (sentence.StartsWith("SET DATABASE"))
            {
                var databaseName = sentence.Substring(14).Trim();
                return SetDatabaseContext(databaseName);
            }
            else if (sentence.StartsWith("CREATE TABLE"))
            {
                var parts = sentence.Substring(12).Trim().Split(" AS ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string columnDefinitions = parts[1].Trim();
                return new CreateTable().Execute(tableName, columnDefinitions);
            }
            else if (sentence.StartsWith("INSERT INTO"))
            {
                var parts = sentence.Substring(12).Trim().Split(" VALUES ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim().Split(" ")[2].Trim();
                string values = parts[1].Trim('(', ')');
                return new Insert().Execute(tableName, values);
            }
            else if (sentence.StartsWith("SELECT"))
            {
                var parts = sentence.Substring(6).Trim().Split(" FROM ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string condition = parts.Length > 1 ? parts[1].Trim() : "";
                string tableName = parts[0].Trim();
                return new Select().Execute(tableName, condition);
            }
            else if (sentence.StartsWith("UPDATE"))
            {
                var parts = sentence.Substring(7).Trim().Split(" SET ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string setClause = parts[1].Trim();
                string condition = "";
                if (setClause.Contains(" WHERE "))
                {
                    var setParts = setClause.Split(" WHERE ");
                    setClause = setParts[0].Trim();
                    condition = setParts[1].Trim();
                }
                return new Update().Execute(tableName, setClause, condition);
            }
            else if (sentence.StartsWith("DELETE"))
            {
                var parts = sentence.Substring(6).Trim().Split(" FROM ");
                if (parts.Length < 2) throw new InvalidSQLFormatException();
                string tableName = parts[0].Trim();
                string condition = parts.Length > 1 ? parts[1].Trim() : "";
                return new Delete().Execute(tableName, condition);
            }
            else
            {
                throw new UnknownSQLSentenceException(); // Lanza excepción si la sentencia no es válida
            }
        }


        /// Crea una nueva base de datos con el nombre especificado.

        private static OperationStatus CreateDatabase(string databaseName)
        {
            string databasePath = Path.Combine(Store.GetInstance().DataPath, databaseName);
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
                File.AppendAllText(Store.SystemDatabasesFile, $"{databaseName}\n"); // Registra la base de datos
                return OperationStatus.Success;
            }
            return OperationStatus.TableAlreadyExists; // Devuelve estado de existencia
        }


        /// Establece el contexto de la base de datos actual.

        private static OperationStatus SetDatabaseContext(string databaseName)
        {
            if (Store.GetInstance().DatabaseExists(databaseName))
            {
                currentDatabase = databaseName; // Actualiza la base de datos actual
                return OperationStatus.Success;
            }
            return OperationStatus.TableNotFound; // Devuelve estado de no encontrado
        }
    }
}