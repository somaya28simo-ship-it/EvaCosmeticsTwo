namespace WebApplication1.ViewModels
{
    public class UserOrdersViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public List<MyOrdersViewModel> Orders { get; set; }
    }
}
