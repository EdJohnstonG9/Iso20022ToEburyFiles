namespace EburyMPIsoFiles.Services
{
    public interface IObjectToPropertiesService
    {
        T GetCurrent<T>();
        bool SaveCurrent<T>(T settngs);
    }
}