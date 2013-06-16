using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode.Conformist;
using LocalizationWithCaching.Models;
using NHibernate.Mapping.ByCode;

namespace LocalizationWithCaching.ModelMappings
{
    public class ProductMapping : ClassMapping<Product>
    {
        public ProductMapping()
        {
            Table("product");
            Id(x => x.ProductId, c =>
                {
                    c.Column("product_id");
                    c.Generator(Generators.Identity);
                });


            // Need to be turned on, so N+1 won't happen
            // http://stackoverflow.com/questions/8761249/how-do-i-make-nhibernate-cache-fetched-child-collections
            Cache(x => x.Usage(CacheUsage.ReadWrite));


            Property(x => x.YearIntroduced, c => c.Column("year_introduced"));
        }
    }
}