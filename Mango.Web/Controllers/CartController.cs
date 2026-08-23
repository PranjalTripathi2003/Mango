using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.FirstName = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = response?.Message ?? "Unable to create the order.";
                return View(cart);
            }

            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if (orderHeaderDto == null)
            {
                TempData["error"] = "Order API returned an invalid order response.";
                return View(cart);
            }

            if (response.IsSuccess)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                RazorpayRequestDto razorpayRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                var razorpayResponse = await _orderService.CreateRazorpayOrder(razorpayRequestDto);
                if (razorpayResponse == null || !razorpayResponse.IsSuccess)
                {
                    TempData["error"] = razorpayResponse?.Message ?? "Unable to create the Razorpay order.";
                    return View(cart);
                }

                RazorpayRequestDto razorpayResponseResult = JsonConvert.DeserializeObject<RazorpayRequestDto>(Convert.ToString(razorpayResponse.Result));
                if (razorpayResponseResult == null || string.IsNullOrWhiteSpace(razorpayResponseResult.RazorpayOrderId))
                {
                    TempData["error"] = "Razorpay returned an invalid order response.";
                    return View(cart);
                }
                ViewBag.RazorpayKeyId = razorpayResponseResult.RazorpayKeyId;
                ViewBag.RazorpayOrderId = razorpayResponseResult.RazorpayOrderId;
                ViewBag.RazorpayAmount = razorpayResponseResult.RazorpayAmount;
                ViewBag.OrderHeaderId = orderHeaderDto.OrderHeaderId;
            }
            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var razorpayPaymentId = Request.Query["razorpay_payment_id"].ToString();
            var razorpayOrderId = Request.Query["razorpay_order_id"].ToString();
            var razorpaySignature = Request.Query["razorpay_signature"].ToString();

            if (string.IsNullOrWhiteSpace(razorpayPaymentId) ||
                string.IsNullOrWhiteSpace(razorpaySignature))
            {
                TempData["error"] = "Razorpay did not return complete payment details.";
                return RedirectToAction(nameof(Checkout));
            }

            RazorpayRequestDto razorpayRequestDto = new()
            {
                OrderHeader = new OrderHeaderDto { OrderHeaderId = orderId },
                RazorpayOrderId = razorpayOrderId,
                RazorpayPaymentId = razorpayPaymentId,
                RazorpaySignature = razorpaySignature
            };

            ResponseDto? response = await _orderService.ValidateRazorpayPayment(razorpayRequestDto);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if (orderHeader != null && orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }

            TempData["error"] = response?.Message ?? "Payment verification failed.";
            return RedirectToAction(nameof(Checkout));
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["error"] = response?.Message ?? "Could not apply coupon.";
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.EmailCart(cart);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsnyc(userId);
            if (response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
    }
}
