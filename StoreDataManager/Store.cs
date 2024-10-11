// Store.cs
using Entities;
using System.IO;
using System.Linq;

namespace StoreDataManager
{

    /// Clase Singleton que maneja la gestión de bases de datos y operaciones relacionadas.

    public sealed class Store
    {
        private static Store? instance = null; // Instancia Singleton
        private static readonly object _lock = new object(); // Lock para hilos


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


        private const string DatabaseBasePath = @"C:\TinySql\"; // Ruta base para las bases de datos
        private const string SystemCatalogPath = $@"{DatabaseBasePath}SystemCatalog"; // Ruta del catálogo del sistema
        public const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table"; // Archivo de bases de datos
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table"; // Archivo de tablas del sistema

        public string DataPath => Path.Combine(DatabaseBasePath, "Data"); // Propiedad para la ruta de datos

        // Constructor que inicializa el catálogo del sistema
        public Store()
        {
            InitializeSystemCatalog();
        }


        /// Inicializa el catálogo del sistema, creando las carpetas y archivos necesarios.

        private void InitializeSystemCatalog()
        {
            Directory.CreateDirectory(SystemCatalogPath); // Crea la carpeta del catálogo
            if (!File.Exists(SystemTablesFile)) // Si no existe el archivo de tablas, lo crea
            {
                File.Create(SystemTablesFile).Dispose();
            }
        }


        /// Crea una nueva base de datos con el nombre especificado.

        public OperationStatus CreateDatabase(string databaseName)
        {
            string databasePath = Path.Combine(DataPath, databaseName);
            if (!Directory.Exists(databasePath)) // Verifica si ya existe la base de datos
            {
                Directory.CreateDirectory(databasePath); // Crea la carpeta de la base de datos
                File.AppendAllText(SystemDatabasesFile, $"{databaseName}\n"); // Agrega el nombre al archivo de bases de datos
                return OperationStatus.Success;
            }
            return OperationStatus.TableAlreadyExists; // Devuelve estado de existencia
        }


        /// Verifica si una base de datos existe.

        public bool DatabaseExists(string databaseName)
        {
            string databasePath = Path.Combine(DataPath, databaseName);
            return Directory.Exists(databasePath); // Devuelve si existe la carpeta
        }


        /// Crea una tabla con las definiciones de columnas especificadas.

        public OperationStatus CreateTable(string tableName, string columnDefinitions)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return OperationStatus.InvalidColumnDefinitions; // Verifica que el nombre no esté vacío
            }

            string tableDirectory = Path.Combine(DataPath, "TESTDB");
            Directory.CreateDirectory(tableDirectory); // Crea la carpeta de la tabla
            string tablePath = Path.Combine(tableDirectory, $"{tableName}.Table"); // Define la ruta del archivo de la tabla

            var columns = ParseColumnDefinitions(columnDefinitions); // Parsea las definiciones de columnas
            if (columns == null || columns.Length == 0)
            {
                return OperationStatus.InvalidColumnDefinitions; // Verifica que existan columnas
            }

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var column in columns)
                {
                    writer.Write(column); // Escribe cada columna en el archivo
                }
            }

            LogTableCreation(tableName, columnDefinitions); // Registra la creación de la tabla
            return OperationStatus.Success;
        }


        /// Registra la creación de una tabla en el catálogo del sistema.

        public void LogTableCreation(string tableName, string columnDefinitions)
        {
            string logEntry = $"{tableName};{columnDefinitions};{DateTime.Now}\n"; // Formato del registro
            File.AppendAllText(SystemTablesFile, logEntry); // Agrega el registro al archivo de tablas
        }


        /// Verifica si una tabla existe en la base de datos.

        public bool TableExists(string tableName)
        {
            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            return File.Exists(tablePath); // Devuelve si existe el archivo de la tabla
        }


        /// Realiza una consulta SELECT sobre la tabla especificada.

        public OperationStatus Select(string tableName, string condition = "")
        {
            if (!TableExists(tableName)) // Verifica que la tabla exista
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");

            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length) // Lee hasta el final del archivo
                {
                    int id = reader.ReadInt32(); // Lee el ID
                    string nombre = reader.ReadString(); // Lee el nombre
                    string apellido = reader.ReadString(); // Lee el apellido

                    // Evalúa si el registro cumple con la condición
                    if (string.IsNullOrEmpty(condition) || EvaluateCondition(id, nombre, apellido, condition))
                    {
                        Console.WriteLine($"ID: {id}, Nombre: {nombre}, Apellido: {apellido}"); // Muestra el registro
                    }
                }
            }

            return OperationStatus.Success; // Devuelve estado de éxito
        }


        /// Evalúa una condición sobre los registros.

        private bool EvaluateCondition(int id, string nombre, string apellido, string condition)
        {
            if (condition.StartsWith("ID ="))
            {
                return id == int.Parse(condition.Split('=')[1].Trim()); // Evalúa condición por ID
            }
            if (condition.StartsWith("Nombre LIKE"))
            {
                string value = condition.Split("LIKE")[1].Trim().Trim('"'); // Evalúa condición por nombre
                return nombre.Contains(value);
            }
            return true; // Devuelve true si no se encontró condición válida
        }


        /// Inserta valores en la tabla especificada.

        public OperationStatus InsertIntoTable(string tableName, string[] values)
        {
            if (!TableExists(tableName)) // Verifica que la tabla exista
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");

            using (FileStream stream = File.Open(tablePath, FileMode.Append))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var value in values) // Escribe cada valor en la tabla
                {
                    writer.Write(value);
                }
            }

            return OperationStatus.Success; // Devuelve estado de éxito
        }


        /// Parsea las definiciones de columnas de una cadena.

        private string[]? ParseColumnDefinitions(string columnDefinitions)
        {
            var columns = columnDefinitions.Split(';'); // Separa las definiciones por punto y coma
            return columns.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray(); // Filtra y devuelve columnas válidas
        }


        /// Actualiza los registros de una tabla según los valores y condiciones especificados.

        public OperationStatus UpdateTable(string tableName, string[] setValues, string condition)
        {
            if (!TableExists(tableName)) // Verifica que la tabla exista
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            var updatedRecords = new List<string>();

            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length) // Lee hasta el final del archivo
                {
                    int id = reader.ReadInt32();
                    string nombre = reader.ReadString();
                    string apellido = reader.ReadString();

                    if (EvaluateCondition(id, nombre, apellido, condition)) // Evalúa condición para la actualización
                    {
                        foreach (var setValue in setValues)
                        {
                            var parts = setValue.Split('='); // Separa el valor del nombre de la columna
                            string columnName = parts[0].Trim();
                            string newValue = parts[1].Trim().Trim('\''); // Quita comillas si están presentes

                            if (columnName.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                            {
                                nombre = newValue; // Actualiza el nombre
                            }
                            else if (columnName.Equals("Apellido", StringComparison.OrdinalIgnoreCase))
                            {
                                apellido = newValue; // Actualiza el apellido
                            }
                        }
                    }
                    updatedRecords.Add($"{id};{nombre};{apellido}"); // Guarda el registro actualizado
                }
            }

            using (FileStream stream = File.Open(tablePath, FileMode.Create))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var record in updatedRecords) // Escribe los registros actualizados en el archivo
                {
                    var fields = record.Split(';');
                    writer.Write(int.Parse(fields[0]));
                    writer.Write(fields[1]);
                    writer.Write(fields[2]);
                }
            }

            return OperationStatus.Success; // Devuelve estado de éxito
        }


        /// Elimina registros de una tabla según la condición especificada.

        public OperationStatus DeleteFromTable(string tableName, string condition)
        {
            if (!TableExists(tableName)) // Verifica que la tabla exista
            {
                return OperationStatus.TableNotFound;
            }

            string tablePath = Path.Combine(DataPath, "TESTDB", $"{tableName}.Table");
            var remainingRecords = new List<string>();

            using (FileStream stream = File.Open(tablePath, FileMode.Open))
            using (BinaryReader reader = new(stream))
            {
                while (stream.Position < stream.Length) // Lee hasta el final del archivo
                {
                    int id = reader.ReadInt32();
                    string nombre = reader.ReadString();
                    string apellido = reader.ReadString();

                    if (!EvaluateCondition(id, nombre, apellido, condition)) // Evalúa condición para la eliminación
                    {
                        remainingRecords.Add($"{id};{nombre};{apellido}"); // Guarda el registro si no cumple la condición
                    }
                }
            }

            using (FileStream stream = File.Open(tablePath, FileMode.Create))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var record in remainingRecords) // Escribe los registros restantes en el archivo
                {
                    var fields = record.Split(';');
                    writer.Write(int.Parse(fields[0]));
                    writer.Write(fields[1]);
                    writer.Write(fields[2]);
                }
            }

            return OperationStatus.Success; // Devuelve estado de éxito
        }
    }
}