namespace IntuitionApp.Models.Items
{
    public interface IItem
    {
        decimal Price { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
    }
}