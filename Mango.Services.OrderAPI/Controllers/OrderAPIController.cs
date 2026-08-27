using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mango.Services.OrderAPI.Controllers
{

    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {

        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IMessageBus _messageBus;



        public OrderAPIController(AppDbContext db, IProductService productService, IMapper mapper, IConfiguration configuration, IHttpClientFactory httpClientFactory, IMessageBus messageBus)
        {
            _db = db;
            this._response = new ResponseDto();
            _productService = productService;
            _mapper = mapper;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _messageBus = messageBus;

        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {

                OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }

            return _response;
        }



        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;

                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreateRazorpayOrder")]
        public async Task<ResponseDto> CreateRazorpayOrder([FromBody] RazorpayRequestDto razorpayRequestDto)
        {
            try
            {
                var keyId = _configuration["Razorpay:KeyId"];
                var keySecret = _configuration["Razorpay:KeySecret"];
                var amountInPaise = Convert.ToInt32(Math.Round(razorpayRequestDto.OrderHeader.OrderTotal * 100, MidpointRounding.AwayFromZero));
                var payload = JsonSerializer.Serialize(new
                {
                    amount = amountInPaise,
                    currency = "INR",
                    receipt = $"order_{razorpayRequestDto.OrderHeader.OrderHeaderId}"
                });

                var client = _httpClientFactory.CreateClient();
                var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyId}:{keySecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                var response = await client.PostAsync(
                    "https://api.razorpay.com/v1/orders",
                    new StringContent(payload, Encoding.UTF8, "application/json"));

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseBody);
                }

                using var document = JsonDocument.Parse(responseBody);
                var orderId = document.RootElement.GetProperty("id").GetString();

                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == razorpayRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.RazorpayOrderId = orderId;
                _db.SaveChanges();

                razorpayRequestDto.RazorpayOrderId = orderId;
                razorpayRequestDto.RazorpayKeyId = keyId;
                razorpayRequestDto.RazorpayAmount = amountInPaise.ToString();
                _response.Result = razorpayRequestDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }
            return _response;
        }

        private static string GetRazorpaySignature(string? orderId, string? paymentId, string? secret)
        {
            if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(paymentId) || string.IsNullOrWhiteSpace(secret))
            {
                return string.Empty;
            }

            var payload = $"{orderId}|{paymentId}";
            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        }

        [Authorize]
        [HttpPost("ValidateRazorpayPayment")]
        public async Task<ResponseDto> ValidateRazorpayPayment([FromBody] RazorpayRequestDto razorpayRequestDto)
        {
            try
            {
                if (razorpayRequestDto.OrderHeader == null || razorpayRequestDto.OrderHeader.OrderHeaderId <= 0 ||
                    string.IsNullOrWhiteSpace(razorpayRequestDto.RazorpayPaymentId) ||
                    string.IsNullOrWhiteSpace(razorpayRequestDto.RazorpaySignature))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Razorpay payment details are incomplete.";
                    return _response;
                }

                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == razorpayRequestDto.OrderHeader.OrderHeaderId);
                var secret = _configuration["Razorpay:KeySecret"];
                if (string.IsNullOrWhiteSpace(orderHeader.RazorpayOrderId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "The server order is missing its Razorpay order ID.";
                    return _response;
                }

                var expectedSignature = GetRazorpaySignature(orderHeader.RazorpayOrderId, razorpayRequestDto.RazorpayPaymentId, secret);

                if (string.Equals(expectedSignature, razorpayRequestDto.RazorpaySignature, StringComparison.OrdinalIgnoreCase))
                {
                    orderHeader.PaymentIntentId = razorpayRequestDto.RazorpayPaymentId;
                    orderHeader.RazorpayPaymentId = razorpayRequestDto.RazorpayPaymentId;
                    orderHeader.RazorpaySignature = razorpayRequestDto.RazorpaySignature;
                    orderHeader.Status = SD.Status_Approved;
                    _db.SaveChanges();

                    RewardsDto rewardsDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId

                    };

                    string? topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    if (string.IsNullOrWhiteSpace(topicName))
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Order-created topic name is missing from configuration.";
                        return _response;
                    }

                    await _messageBus.PublishMessage(rewardsDto, topicName);
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Payment signature verification failed.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderId);
                if (orderHeader != null)
                {
                    if (newStatus == SD.Status_Cancelled)
                    {
                        // we will give refund
                        var paymentId = orderHeader.RazorpayPaymentId ?? orderHeader.PaymentIntentId;
                        if (string.IsNullOrWhiteSpace(paymentId))
                        {
                            throw new InvalidOperationException("Cannot refund this order because Razorpay payment id is missing.");
                        }

                        var keyId = _configuration["Razorpay:KeyId"];
                        var keySecret = _configuration["Razorpay:KeySecret"];
                        if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(keySecret))
                        {
                            throw new InvalidOperationException("Razorpay credentials are missing from configuration.");
                        }

                        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.razorpay.com/v1/payments/{paymentId}/refund")
                        {
                            Content = new StringContent("{}", Encoding.UTF8, "application/json")
                        };

                        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyId}:{keySecret}"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
                        request.Headers.Add("X-Refund-Idempotency", $"order-{orderHeader.OrderHeaderId}-refund");

                        var client = _httpClientFactory.CreateClient();
                        var refundResponse = await client.SendAsync(request);
                        var refundResponseBody = await refundResponse.Content.ReadAsStringAsync();
                        if (!refundResponse.IsSuccessStatusCode)
                        {
                            throw new Exception(refundResponseBody);
                        }
                    }
                    orderHeader.Status = newStatus;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException != null ? $"{ex.Message} --> {ex.InnerException.Message}" : ex.Message;
            }
            return _response;
        }


    }
}
