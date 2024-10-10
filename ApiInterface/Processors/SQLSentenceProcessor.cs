using ApiInterface.InternalModels;
using ApiInterface.Models;
using Entities;
using QueryProcessor;

namespace ApiInterface.Processors
{
    internal class SQLSentenceProcessor : IProcessor
    {
        public Request Request { get; }

        // Constructor
        public SQLSentenceProcessor(Request request)
        {
            Request = request;
        }

        // Implementación del método Process sin parámetros de IProcessor
        public Response Process()
        {
            // Lógica para el caso sin parámetros, quizás puedas lanzar una excepción o retornar un error
            throw new NotImplementedException("Este método no está implementado.");
        }

        // Implementación del método Process que acepta un parámetro Request
        public Response Process(Request request)
        {
            var sentence = request.RequestBody; // Usar el argumento
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
    }
}
