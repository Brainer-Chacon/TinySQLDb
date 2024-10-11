using ApiInterface.InternalModels;
using ApiInterface.Models;
using Entities;
using QueryProcessor;

namespace ApiInterface.Processors
{
    internal class SQLSentenceProcessor : IProcessor
    {
        public Request Request { get; }

        // Constructor principal que inicializa la propiedad Request
        public SQLSentenceProcessor(Request request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request)); // Asegúrate de que no sea nulo
        }

        // Implementación del método Process sin parámetros
        public Response Process()
        {
            throw new NotImplementedException("Este método no está implementado.");
        }

        // Implementación del método Process que acepta un parámetro Request
        public Response Process(Request request)
        {
            var sentence = request.RequestBody;
            var result = SQLQueryProcessor.Execute(sentence);
            var response = this.ConvertToResponse(result);
            return response;
        }

        private Response ConvertToResponse(OperationStatus result)
        {
            return new Response
            {
                Status = result,
                Request = this.Request,
                ResponseBody = string.Empty
            };
        }

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
