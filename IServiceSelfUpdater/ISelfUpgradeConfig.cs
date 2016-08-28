using SelfUpdate.Contract;

namespace SelfUpdate.Interface
{
    public interface IServiceSelfUpdateConfig
    {
        UpgradeSetting GetUpgradeSetting();
    }
}