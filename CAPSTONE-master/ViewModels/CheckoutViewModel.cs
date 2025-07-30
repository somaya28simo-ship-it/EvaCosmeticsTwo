using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class CheckoutViewModel
    {
        public string CustomerName { get; set; }   // ✅ كده هيتوافق مع الكود
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
