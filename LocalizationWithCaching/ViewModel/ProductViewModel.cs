using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalizationWithCaching.ViewModel
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int YearIntroduced { get; set; }

        public string LanguageCode { get; set; }
    }
}