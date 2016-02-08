using System.ComponentModel.DataAnnotations;
using Authorization.Models;

namespace Authorization.ViewModels.Products
{
    public class EditProductViewModel
    {
        public Product Product { get; set; }

        [Range(1, 100)]
        public decimal Discount { get; set; } = 0;
    }
}
