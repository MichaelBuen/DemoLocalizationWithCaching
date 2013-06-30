using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LocalizationWithCaching.Models;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace LocalizationWithCaching.ModelMappings
{

    public class GetOrdersInfoMapping : ClassMapping<GetOrdersInfo>
    {
        public GetOrdersInfoMapping()
        {
            Table("dbo.tvf_get_orders_info(:lf.LanguageCode)");

            Cache(x => x.Usage(CacheUsage.ReadOnly));

            Synchronize(new[] { "product", ProductLanguageMapping.TableName });

            Id(x => x.OrderedProductId, c => c.Column("ordered_product_id"));

            Property(x => x.OrderedBy, c => c.Column("ordered_by"));
            Property(x => x.DateOrdered, c => c.Column("date_ordered"));
            Property(x => x.ProductName, c => c.Column("product_name"));
            Property(x => x.YearIntroduced, c => c.Column("year_introduced"));
            Property(x => x.Qty, c => c.Column("qty"));
        
        }
    }
}