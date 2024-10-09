using Entities;
using System.IO;
using System.Linq;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();

        public static Store GetInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}Data";
        private const string SystemCatalogPath = $@"{DataPath}SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}SystemTables.table";

        public Store()
        {
            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            // Ensure that the system catalog directory exists
            Directory.CreateDirectory(SystemCatalogPath);
            // Create the SystemTables file if it doesn't exist
            if (!File.Exists(SystemTablesFile))
            {
                File.Create(SystemTablesFile).Dispose();
            }
        }

        public OperationStatus CreateTable(string tableName, string columnDefinitions)
        {
            // Create the directory for the new table if it doesn't exist
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return OperationStatus.InvalidColumnDefinitions;
            }

            // Create the table file
            string tableDirectory = Path.Combine(DataPath, "TESTDB");
            Directory.CreateDirectory(tableDirectory);
            string tablePath = Path.Combine(tableDirectory, $"{tableName}.Table");

            // Parse the column definitions
            var columns = ParseColumnDefinitions(columnDefinitions);
            if (columns == null || columns.Length == 0)
            {
                return OperationStatus.InvalidColumnDefinitions;
            }

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new(stream))
            {
                // Write column definitions and initial data to the file
                foreach (var column in columns)
                {
                    writer.Write(column);
                }
            }

            // Log the table creation
            LogTableCreation(tableName, columnDefinitions);
            return OperationStatus.Success;
        }

        public void LogTableCreation(string tableName, string columnDefinitions)
        {
            // Log the table creation in the system catalog
            string logEntry = $"{tableName};{columnDefinitions};{DateTime.Now}\n";
            File.AppendAllText(SystemTablesFile, logEntry);
        }

        public bool TableExists(string tableName)
        {
            // Check if the table file exists in the system catalog
            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            return File.Exists(tablePath);
        }

        public OperationStatus Select(string tableName, string condition = null)
        {
            // Check if the table exists
            if (!TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");

            // Read data from the table file
            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length)
                {
                    int id = reader.ReadInt32();
                    string nombre = reader.ReadString();
                    string apellido = reader.ReadString();

                    // Apply condition if specified
                    if (string.IsNullOrEmpty(condition) || EvaluateCondition(id, nombre, apellido, condition))
                    {
                        Console.WriteLine($"ID: {id}, Nombre: {nombre}, Apellido: {apellido}");
                    }
                }
            }

            return OperationStatus.Success;
        }

        private bool EvaluateCondition(int id, string nombre, string apellido, string condition)
        {
            if (condition.StartsWith("ID ="))
            {
                return id == int.Parse(condition.Split('=')[1].Trim());
            }
            if (condition.StartsWith("Nombre LIKE"))
            {
                string value = condition.Split("LIKE")[1].Trim().Trim('"'); // Cambiar de ' a "
                return nombre.Contains(value);
            }
            return true; // Default to true if no valid condition is found
        }


        public OperationStatus InsertIntoTable(string tableName, string[] values)
        {
            // Check if the table exists
            if (!TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");

            using (FileStream stream = File.Open(tablePath, FileMode.Append))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var value in values)
                {
                    writer.Write(value);
                }
            }

            return OperationStatus.Success;
        }

        private string[]? ParseColumnDefinitions(string columnDefinitions)
        {
            var columns = columnDefinitions.Split(';');
            return columns.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
        }

        public OperationStatus UpdateTable(string tableName, string[] setValues, string condition)
        {
            if (!TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            var updatedRecords = new List<string>();

            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length)
                {
                    int id = reader.ReadInt32();
                    string nombre = reader.ReadString();
                    string apellido = reader.ReadString();

                    if (EvaluateCondition(id, nombre, apellido, condition))
                    {
                        foreach (var setValue in setValues)
                        {
                            var parts = setValue.Split('=');
                            string columnName = parts[0].Trim();
                            string newValue = parts[1].Trim().Trim('\''); // Remove quotes if present

                            if (columnName.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                            {
                                nombre = newValue;
                            }
                            else if (columnName.Equals("Apellido", StringComparison.OrdinalIgnoreCase))
                            {
                                apellido = newValue;
                            }
                        }
                    }
                    updatedRecords.Add($"{id};{nombre};{apellido}");
                }
            }

            using (FileStream stream = File.Open(tablePath, FileMode.Create))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var record in updatedRecords)
                {
                    var fields = record.Split(';');
                    writer.Write(int.Parse(fields[0]));
                    writer.Write(fields[1]);
                    writer.Write(fields[2]);
                }
            }

            return OperationStatus.Success;
        }

        public OperationStatus DeleteFromTable(string tableName, string condition)
        {
            if (!TableExists(tableName))
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            var remainingRecords = new List<string>();

            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length)
                {
                    int id = reader.ReadInt32();
                    string nombre = reader.ReadString();
                    string apellido = reader.ReadString();

                    if (!EvaluateCondition(id, nombre, apellido, condition))
                    {
                        remainingRecords.Add($"{id};{nombre};{apellido}");
                    }
                }
            }

            using (FileStream stream = File.Open(tablePath, FileMode.Create))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var record in remainingRecords)
                {
                    var fields = record.Split(';');
                    writer.Write(int.Parse(fields[0]));
                    writer.Write(fields[1]);
                    writer.Write(fields[2]);
                }
            }

            return OperationStatus.Success;
        }
    }
}
