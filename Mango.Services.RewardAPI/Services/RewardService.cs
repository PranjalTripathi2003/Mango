
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {

        private DbContextOptions<AppDbContext> _dbOptions;
        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }



        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            Rewards rewards = new()
            {
                OrderId = rewardMessage.OrderId,
                RewardsActivity = rewardMessage.RewardsActivity,
                RewardsDate = DateTime.Now,
                UserId = rewardMessage.UserId
            };

            await using var _db = new AppDbContext(_dbOptions);

            await _db.Rewards.AddAsync(rewards);
            await _db.SaveChangesAsync();
        }

       
    }
}
