using IntuitionApp.Models.Items;

namespace IntuitionApp.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
        public Decimal TotalAmount { get; set; }
    }
}
