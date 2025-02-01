using System.ComponentModel;

namespace VowAI.TotalEye.ServerShared
{
    public enum CentreRequestName
    {
        Client_Screenshot /* Take a screenshot on the client. */,
        Client_Command /* Run a command on client. */,
        HTTP_Sniffer /* Fetch HTTP sniffing logs from client. */,
    }
}
