using ElectronNET.API.Entities;

namespace SpiritTime.Frontend.Infrastructure.Config
{
    public class ElectronProxyConfig
    {
        public string PacScript { get; set; }
        public string ProxyRules { get; set; }
        public string ProxyBypassRules { get; set; }

        public ProxyConfig GetProxy()
        {
            return new ProxyConfig(PacScript, ProxyRules, ProxyBypassRules);
        }
    }
}