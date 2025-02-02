namespace VowAI.TotalEye.ServerShared
{
    public enum ServerRequestName
    {
        Client_Screenshot /* Take a screenshot on the client. */,
        Client_Command /* Run a command on client. */,
        HTTP_Logs /* Fetch HTTP sniffing logs from client. */,
    }
}
