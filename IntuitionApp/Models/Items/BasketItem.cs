namespace IntuitionApp.Models.Items
{
    public class BasketItem : Item
    {
        public BasketItem(Item item)
        {
            Price = item.Price;
            Quantity = item.Quantity;
        }

        public BasketItem()
        {

        }
        public int Id { get; set; }
        public int BasketId { get; set; }

    }
}