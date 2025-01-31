using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy;
using System.Net;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public delegate void TunnelConnectHandler(TunnelConnectSessionEventArgs args);
    public delegate void WebHandler(SessionEventArgs args);

    public class HttpSniffer : IDisposable
    {
        private readonly ProxyServer _proxyServer;
        private readonly ExplicitProxyEndPoint _explicitEndPoint;

        private TunnelConnectHandler _connects;
        private WebHandler _requests;
        private WebHandler _responses;

        public HttpSniffer(int port, TunnelConnectHandler handleTunnelConnect, WebHandler handleRequest, WebHandler handleResponse)
        {
            _connects += handleTunnelConnect;
            _requests += handleRequest;
            _responses += handleResponse;

            _explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true);
            _explicitEndPoint.BeforeTunnelConnectRequest += ExplicitEndPoint_BeforeTunnelConnectRequest;

            _proxyServer = new ProxyServer();
            _proxyServer.BeforeRequest += OnRequest;
            _proxyServer.BeforeResponse += OnResponse;
            _proxyServer.AddEndPoint(_explicitEndPoint);

            _proxyServer.Start();
            _proxyServer.SetAsSystemHttpProxy(_explicitEndPoint);
            _proxyServer.SetAsSystemHttpsProxy(_explicitEndPoint);
        }

        public void Dispose()
        {
            if (_proxyServer.ProxyRunning)
            {
                _explicitEndPoint.BeforeTunnelConnectRequest -= ExplicitEndPoint_BeforeTunnelConnectRequest;
                _proxyServer.BeforeResponse -= OnResponse;
                _proxyServer.BeforeRequest -= OnRequest;
                _proxyServer.Stop();
            }

            _proxyServer.DisableAllSystemProxies();
            _proxyServer.Dispose();
        }

        private async Task ExplicitEndPoint_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            try
            {
                await Task.Run(() => _connects(e));
            }
            catch (Exception exception)
            {
                exception.Write<HttpSniffer>();
            }
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            try
            {
                await Task.Run(() => _requests(e));
            }
            catch (Exception exception)
            {
                exception.Write<HttpSniffer>();
            }
        }

        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            try
            {
                await Task.Run(() => _responses(e));
            }
            catch (Exception exception)
            {
                exception.Write<HttpSniffer>();
            }
        }
    }
}
