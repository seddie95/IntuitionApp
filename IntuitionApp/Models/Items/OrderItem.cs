namespace IntuitionApp.Models.Items
{
    public class OrderItem : Item
    {
        public OrderItem(Item item)
        {
            Price = item.Price;
            Quantity = item.Quantity;
        }

        public OrderItem()
        {
            
        }
        public int Id { get; set; }
        public int OrderId { get; set; }

    }
}