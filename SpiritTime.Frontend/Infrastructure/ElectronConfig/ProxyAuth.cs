using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public class ProxyAuth
    {
        public string ProxyUrl { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyDomain { get; set; }
        public string AuthType { get; set; }
        public bool Enabled { get; set; }
        public bool DefaultNetworkCred { get; set; }

        public async Task<(bool, string)> TestProxy(string targetUrl)
        {
            try
            {
                HttpClient httpClient;
                if (Enabled)
                {
                    var handler = new HttpClientHandler();
                    handler.UseProxy = true;
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri(ProxyUrl)
                    };
                    httpClient = new HttpClient(handler);
                }
                else
                {
                    httpClient = new HttpClient();
                }
                
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, targetUrl);
                using (var httpResponse = httpClient.SendAsync(httpRequest).GetAwaiter().GetResult())
                {
                    var code = httpResponse.StatusCode;
                    if (code == HttpStatusCode.OK)
                    {
                        return (true, "");
                    }

                    return (false, code.ToString());
                }
            }
            catch (Exception exception)
            {
                return (false, exception.Message);
            }
        }

        public HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            handler.UseProxy = true;
            handler.Proxy = new WebProxy
            {
                Address = new Uri(ProxyUrl)
            };
            if (DefaultNetworkCred)
            {
                handler.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                // var credCache = new CredentialCache();
                var credentials = new NetworkCredential(ProxyUsername, ProxyPassword, ProxyDomain);
                // credCache.Add(new Uri(proxyUrl), authType, credentials);
                handler.Proxy.Credentials = credentials;
            }
            return handler;
        }
    }
}