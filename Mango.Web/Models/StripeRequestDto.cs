namespace Mango.Web.Models
{
    public class RazorpayRequestDto
    {
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
    public string? RazorpayKeyId { get; set; }
        public string? RazorpayAmount { get; set; }    
        public string? ApprovedUrl { get; set; }    
        public string? CancelUrl { get; set; }    
        public OrderHeaderDto OrderHeader { get; set; }    

    }
}
