using ServiceSelfUpdate.Contract;

namespace IServiceSelfUpdater
{
    public interface IServiceSelfUpdateConfig
    {
        UpgradeSetting GetUpgradeSetting();
    }
}