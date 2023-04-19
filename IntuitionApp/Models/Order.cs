using IntuitionApp.Models.Items;

namespace IntuitionApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string Address { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
    }

    public enum OrderStatus
    {
        Processing,
        ReadyForDelivery,
        OutForDelivery,
        Delivered,
        Canceled,
    }
}
