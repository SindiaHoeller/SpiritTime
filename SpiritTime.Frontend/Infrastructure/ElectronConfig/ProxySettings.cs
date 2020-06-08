using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public static class ProxySettings
    {
        public static HttpClientHandler GetClientHandler(IConfiguration config)
        {
            var proxyAuth = new ProxyAuth
            {
                Enabled = true,
                ProxyUrl           = config["ProxyAuth:ProxyUrl"],
                ProxyUsername      = config["ProxyAuth:ProxyUsername"],
                ProxyPassword      = config["ProxyAuth:ProxyPassword"],
                ProxyDomain        = config["ProxyAuth:ProxyDomain"],
                DefaultNetworkCred = Convert.ToBoolean(config["ProxyAuth:DefaultNetworkCred"]),
            };


            return proxyAuth.GetHandler();
        }
    }
}