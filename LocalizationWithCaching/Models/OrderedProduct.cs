using System;

namespace LocalizationWithCaching.Models
{
    public class OrderedProduct
    {
        public virtual int OrderedProductId { get; set; }

        public virtual DateTime  DateOrdered { get; set; }
        public virtual Product Product { get; set; }
        public virtual int Quantity { get; set; }
    }
}