using NHibernate.Mapping.ByCode.Conformist;

using LocalizationWithCaching.Models;
using NHibernate.Mapping.ByCode;


namespace LocalizationWithCaching.ModelMappings
{
    public class OrderedProductMapping : ClassMapping<OrderedProduct>
    {
        public OrderedProductMapping()
        {
            Table("ordered_product");

            Cache(x => x.Usage(CacheUsage.ReadWrite));

            Id(x => x.OrderedProductId, c => c.Column("ordered_product_id"));

            Property(x => x.DateOrdered, c => c.Column("date_ordered"));
            ManyToOne(x => x.Product, c => c.Column("product_id"));
            Property(x => x.Quantity, c => c.Column("qty"));
        }
    }
}