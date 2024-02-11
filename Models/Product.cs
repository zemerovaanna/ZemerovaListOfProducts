using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppListOfProducts.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string? ProductImagePath { get; set; }

        [StringLength(250)]
        public string? ProductDescription { get; set; }

        public int ProductQuantity { get; set; }

        public long ProductPrice { get; set; }

        public byte ProductDiscount { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}