using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

using LocalizationWithCaching.Models;

namespace LocalizationWithCaching.ModelMappings
{
    public class GetProductSoldMapping : ClassMapping<GetProductSold>
    {
        public GetProductSoldMapping()
        {
            Table("dbo.tvf_get_product_sold()");

            Cache(x => x.Usage(CacheUsage.ReadOnly));

            Synchronize(new[] { "ordered_product" });

            Id(x => x.ProductId, c => c.Column("product_id"));
            Property(x => x.Sold, c => c.Column("ordered_count"));
        }
    }
}