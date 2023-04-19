namespace IntuitionApp.Models.Items
{
    public class Item : IItem
    {
        
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
