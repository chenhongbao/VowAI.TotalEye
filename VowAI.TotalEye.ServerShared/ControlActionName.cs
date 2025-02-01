namespace VowAI.TotalEye.ServerShared
{
    public enum ControlActionName
    {
        Client_Taskkill /* Kill a task on client. */,
        Client_Command /* Execute a command on client. */,
        HTTP_Connect_Deny /* Deny an HTTP connect. */,
        HTTP_Connect_Ignore /* Don't probe an HTTP session. */,
        HTTP_Session_Redirect /* Redirect a session to another url. */,
        HTTP_Session_OK /* Return an HTML content to client instead of the original response. */,
    }
}
