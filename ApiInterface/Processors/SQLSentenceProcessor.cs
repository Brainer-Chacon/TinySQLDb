using ApiInterface.InternalModels;
using ApiInterface.Models;
using Entities;
using QueryProcessor;

namespace ApiInterface.Processors
{

    /// Procesador de sentencias SQL.

    internal class SQLSentenceProcessor : IProcessor
    {
        public Request Request { get; } // Propiedad que almacena la solicitud

        // Constructor principal que inicializa la propiedad Request
        public SQLSentenceProcessor(Request request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request)); // Asegúrate de que no sea nulo
        }

        // Implementación del método Process sin parámetros
        public Response Process()
        {
            throw new NotImplementedException("Este método no está implementado."); // Método no implementado
        }

        // Implementación del método Process que acepta un parámetro Request
        public Response Process(Request request)
        {
            var sentence = request.RequestBody; // Extrae el cuerpo de la solicitud
            var result = SQLQueryProcessor.Execute(sentence); // Ejecuta la consulta SQL
            var response = this.ConvertToResponse(result); // Convierte el resultado a una respuesta
            return response; // Devuelve la respuesta procesada
        }


        /// Convierte el estado de operación en un objeto Response.

        private Response ConvertToResponse(OperationStatus result)
        {
            return new Response
            {
                Status = result, // Estado de la operación
                Request = this.Request, // Solicitud original
                ResponseBody = string.Empty // Cuerpo de la respuesta vacío
            };
        }


        /// Maneja una solicitud SQL y devuelve una respuesta.

        public Response HandleSqlRequest(string sqlQuery)
        {
            // Crea el objeto Request
            Request request = new Request(sqlQuery, RequestType.SQLSentence); // Asegúrate de que el constructor esté correctamente definido

            // Crea el procesador de sentencia SQL
            SQLSentenceProcessor processor = new SQLSentenceProcessor(request);

            // Procesa la solicitud
            Response response = processor.Process(request);

            return response; // Devuelve la respuesta procesada
        }
    }
}