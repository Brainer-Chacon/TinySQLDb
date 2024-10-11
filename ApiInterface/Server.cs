using System.Net.Sockets;
using System.Net;
using System.Text;
using ApiInterface.InternalModels;
using System.Text.Json;
using ApiInterface.Exceptions;
using ApiInterface.Processors;
using ApiInterface.Models;

namespace ApiInterface
{

    /// Clase para gestionar el servidor de la API.

    public class Server
    {
        // Punto final del servidor (dirección IP y puerto) para escuchar solicitudes
        private static IPEndPoint serverEndPoint = new(IPAddress.Loopback, 11000);
        // Número de conexiones paralelas soportadas
        private static int supportedParallelConnections = 1;

 
        /// Método estático para iniciar el servidor.

        public static async Task Start()
        {
            // Crear un socket para escuchar conexiones entrantes
            using Socket listener = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(serverEndPoint); // Vincula el socket al punto final
            listener.Listen(supportedParallelConnections); // Empieza a escuchar
            Console.WriteLine($"Server ready at {serverEndPoint.ToString()}"); // Indica que el servidor está listo

            // Bucle infinito para aceptar conexiones
            while (true)
            {
                // Aceptar una nueva conexión
                var handler = await listener.AcceptAsync();
                try
                {
                    // Leer el mensaje recibido del cliente
                    var rawMessage = GetMessage(handler);
                    // Convertir el mensaje crudo a un objeto Request
                    var requestObject = ConvertToRequestObject(rawMessage);
                    // Procesar la solicitud y obtener la respuesta
                    var response = ProcessRequest(requestObject);
                    // Enviar la respuesta de vuelta al cliente
                    SendResponse(response, handler);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex); // Manejar excepciones imprimiendo el error
                    await SendErrorResponse("Unknown exception", handler); // Enviar una respuesta de error
                }
                finally
                {
                    handler.Close(); // Cerrar el manejador de socket
                }
            }
        }


        /// Método para recibir un mensaje del cliente.
    
        private static string GetMessage(Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadLine() ?? String.Empty; // Lee una línea del flujo y devuelve, o cadena vacía si es nulo
            }
        }


        /// Convierte un mensaje crudo a un objeto Request.
 
        private static Request ConvertToRequestObject(string rawMessage)
        {
            return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new InvalidRequestException(); // Deserializa el mensaje o lanza excepción
        }


        /// Procesa la solicitud y devuelve una respuesta.
  
        private static Response ProcessRequest(Request requestObject)
        {
            var processor = ProcessorFactory.Create(requestObject); // Crea el procesador para la solicitud
            return processor.Process(); // Procesa la solicitud y devuelve la respuesta
        }


        /// Envía una respuesta al cliente.

        private static void SendResponse(Response response, Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(JsonSerializer.Serialize(response)); // Serializa la respuesta a JSON y la envía
            }
        }


        /// Envía una respuesta de error al cliente.

        private static Task SendErrorResponse(string reason, Socket handler)
        {
            throw new NotImplementedException(); // Método no implementado para manejar errores
        }
    }
}