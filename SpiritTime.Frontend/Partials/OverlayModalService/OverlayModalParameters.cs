using System.Collections.Generic;

namespace SpiritTime.Frontend.Partials.OverlayModalService
{
    public class OverlayModalParameters
    {
        public Dictionary<string, object> _parameters { get; set; }
        public OverlayModalParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        public void Add(string parameterName, object value)
        {
            _parameters[parameterName] = value;
        }

        public T Get<T>(string parameterName)
        {
            if (!_parameters.ContainsKey(parameterName))
            {
                throw new KeyNotFoundException("The key named " + parameterName + " does not exist.");
            }
            return (T)_parameters[parameterName];
        }

        public T TryGet<T>(string parameterName)
        {
            if (_parameters.ContainsKey(parameterName))
            {
                return (T)_parameters[parameterName];
            }
            return default;
        }
    }
}
