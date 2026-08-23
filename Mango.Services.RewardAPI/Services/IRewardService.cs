using Mango.Services.RewardAPI.Message;

namespace Mango.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        public Task UpdateRewards(RewardMessage rewardMessage);
       
    }
}
