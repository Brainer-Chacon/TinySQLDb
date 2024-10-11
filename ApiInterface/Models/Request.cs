namespace ApiInterface.InternalModels
{
    public enum RequestType // Cambiado a public
    {
        SQLSentence = 0
    }

    public class Request
    {
        public string RequestBody { get; set; }
        public RequestType RequestType { get; set; } // Esto está bien

        public Request(string requestBody, RequestType requestType)
        {
            RequestBody = requestBody;
            RequestType = requestType;
        }
    }
}
