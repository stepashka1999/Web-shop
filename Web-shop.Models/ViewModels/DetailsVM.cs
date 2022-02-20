namespace Web_shop.Models.ViewModels
{
    public class DetailsVM
    {
        public Product Product { get; set; }

        public bool ExistsInCart { get; set; } = false;
    }
}
