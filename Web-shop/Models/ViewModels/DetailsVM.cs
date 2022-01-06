namespace Web_shop.Models.ViewModels
{
    public class DetailsVM
    {
        public Product Product { get; set; }

        public bool ExistInCart { get; set; } = false;
    }
}
