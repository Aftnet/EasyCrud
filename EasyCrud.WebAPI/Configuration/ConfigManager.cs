using System.ComponentModel;
using System.Configuration;

namespace EasyCrud.WebAPI.Configuration
{
    public class ConfigManager : IConfigManager
    {
        public T GetParameter<T>(string key)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(ConfigurationManager.AppSettings[key]);
        }
    }
}
