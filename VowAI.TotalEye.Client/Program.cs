using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VowAI.TotalEye.ClientShared;

namespace VowAI.TotalEye.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<IServerPollerConfiguration>(
                new ServerPollerConfiguration().Load((config, loaded) =>
                {
                    config.LoginUrl = loaded.LoginUrl;
                    config.AskUrl = loaded.AskUrl;
                    config.UserId = loaded.UserId;
                    config.Password = loaded.Password;
                    config.Pin = loaded.Pin;

                }, (config) =>
                {
                    config.LoginUrl = "http://plusai.site/Login";
                    config.AskUrl = "http://plusai.site/Ask";
                    config.UserId = 0;
                    config.Password = "";
                    config.Pin = "";
                }));

            builder.Services.AddSingleton<IHttpSnifferConfiguration>(
                new HttpSnifferConfiguration().Load((config, loaded) =>
                {
                    config.Port = loaded.Port;

                }, (config) =>
                {
                    config.Port = 53601;
                }));

            builder.Services.AddSingleton<IUpdateConfiguration>(
                new UpdateConfiguration().Load((config, loaded) =>
                {
                    config.Version = loaded.Version;
                    config.VersionUrl = loaded.VersionUrl;
                    config.FileUrl = loaded.FileUrl;

                }, (config) =>
                {
                    config.Version = 0;
                    config.VersionUrl = "http://plusai.site/Version";
                    config.FileUrl = "";
                }));

            builder.Services.AddSingleton<IClientControlPolicyProvider, ClientControlPolicyProvider>();

            builder.Services.AddSingleton<ServerPoller>();
            builder.Services.AddTransient<ConfiguredHttpSniffer>();
            builder.Services.AddTransient<ConfiguredComputerSniffer>();

            builder.Services.AddHttpClient();

            IHost host = builder.Build();

            using (_ = host.Services.GetRequiredService<ServerPoller>())
            {
                host.Run();
            }
        }
    }
}
