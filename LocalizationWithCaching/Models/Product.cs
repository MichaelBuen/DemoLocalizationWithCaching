using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalizationWithCaching.Models
{
    public class Product
    {
        public virtual int ProductId { get; set; }
        public virtual int YearIntroduced { get; set; }    
    }
}