using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalizationWithCaching.Models
{
    public class GetOrdersInfo
    {
        public virtual int OrderedProductId { get; set; }
        public virtual string OrderedBy { get; set; }
        public virtual DateTime DateOrdered { get; set; }
        public virtual string ProductName { get; set; }
        public virtual int YearIntroduced { get; set; }
        public virtual int Qty { get; set; }
    }
}