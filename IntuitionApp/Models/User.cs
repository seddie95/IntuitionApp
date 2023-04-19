namespace IntuitionApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;

        public Basket Basket { get; set; }= new Basket();
    }
}
