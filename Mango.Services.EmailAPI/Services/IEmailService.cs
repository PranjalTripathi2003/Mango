using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Model.Dto;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        public Task EmailCartAndLog(CartDto cartDto);
        public Task RegisterUserEmailAndLog(string email);

        public Task LogOrderPlaced(RewardMessage rewardsDto);
       
    }
}
