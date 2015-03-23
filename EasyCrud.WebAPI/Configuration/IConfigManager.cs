namespace EasyCrud.WebAPI.Configuration
{
    public interface IConfigManager
    {
        T GetParameter<T>(string key);
    }
}
