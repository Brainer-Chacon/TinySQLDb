using ApiInterface.Exceptions;
using ApiInterface.InternalModels;

namespace ApiInterface.Processors
{
    internal static class ProcessorFactory // Asegúrate de que sea una clase
    {
        internal static IProcessor Create(Request request) // Agrega el modificador de acceso
        {
            if (request.RequestType == RequestType.SQLSentence) // Usa comparación correcta
            {
                return new SQLSentenceProcessor(request);
            }
            throw new UnknowRequestTypeException();
        }
    }
}
