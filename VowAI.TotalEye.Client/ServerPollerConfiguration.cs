﻿namespace VowAI.TotalEye.Client
{
    public class ServerPollerConfiguration : IServerPollerConfiguration
    {
        public string UserLoginUrl { get; set; } = "";
        public string GetInfoRequestUrl { get; set; } = "";
        public int UserId { get; set; } = 0;
        public string Password { get; set; } = "";
    }
}
