namespace WebApplication1.ViewModels
{
    public class MyOrdersViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
