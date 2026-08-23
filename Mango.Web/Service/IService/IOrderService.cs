using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateRazorpayOrder(RazorpayRequestDto razorpayRequestDto);
        Task<ResponseDto?> ValidateRazorpayPayment(RazorpayRequestDto razorpayRequestDto);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto> GetOrder(int orderId);
        Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus);
      

    }
}
