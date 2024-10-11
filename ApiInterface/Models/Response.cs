using ApiInterface.InternalModels;
using Entities;

namespace ApiInterface.Models
{
    internal class Response
    {
        public required Request Request { get; set; }
        public required OperationStatus Status { get; set; }
        public required string ResponseBody { get; set; }

        // Constructor para inicializar todos los campos requeridos
        public Response(Request request, OperationStatus status, string responseBody)
        {
            Request = request;
            Status = status;
            ResponseBody = responseBody;
        }
    }
}
