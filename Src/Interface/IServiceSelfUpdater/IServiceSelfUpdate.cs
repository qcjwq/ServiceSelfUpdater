using SelfUpdate.Contract;

namespace SelfUpdate.Interface
{
    public interface IServiceSelfUpdate
    {
        void Execute(UpgradeSetting upgradeSetting);
    }
}
