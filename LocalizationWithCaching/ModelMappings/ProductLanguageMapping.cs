using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

using LocalizationWithCaching.Models;

namespace LocalizationWithCaching.ModelMappings
{
    public class ProductLanguageMapping : ClassMapping<ProductLanguage>
    {
        
        string save =
@"
merge product_i18n as dst
using( values(?,?,?, ?,?) ) 
	as src(product_name, product_description, actual_language_code, pk_product_id, pk_language_code)
on
	src.pk_product_id = dst.product_id and src.pk_language_code = dst.language_code

when matched then
	update set dst.product_name = src.product_name, dst.product_description = src.product_description
when not matched then
	insert (product_id, language_code, product_name, product_description)
	values (src.pk_product_id, src.pk_language_code, src.product_name, src.product_description);";

        
        public static string TableName = "dbo.get_product_i18n(:lf.LanguageCode)";

        public ProductLanguageMapping()
        {            
            // When the query from this mapping is run on different languages, they will have their isolated copy of query caching.
            // That behavior comes from NHibernate filters. 
            Table(ProductLanguageMapping.TableName);            
            // Hence the above table mapping will have this behavior:
                // TestQueryCache("en"); // database hit
                // TestQueryCache("zh"); // database hit
                // TestQueryCache("en"); // cached query hit
                // TestQueryCache("zh"); // cached query hit
                // TestQueryCache("ca"); // database hit
            // If we don't use NHibernate filters(e.g. using CONTEXT_INFO technique instead), identical queries run from different languages will get the same query cache.
            // Thus this mapping:
            //      Table("dbo.get_product_i18n(convert(nvarchar, substring(context_info(), 5, convert(int, substring(context_info(), 1, 4)) )  ))");
            // Will have this behavior:            
                // TestQueryCache("en"); // database hit
                // TestQueryCache("zh"); // cached query hit
                // TestQueryCache("en"); // cached query hit
                // TestQueryCache("zh"); // cached query hit
                // TestQueryCache("ca"); // cached query hit

            
            // Need to be turned on, so N+1 won't happen
            // http://stackoverflow.com/questions/8761249/how-do-i-make-nhibernate-cache-fetched-child-collections
            Cache(x => x.Usage(CacheUsage.ReadWrite));


            ComponentAsId(key => key.ProductLanguageCompositeKey, m =>
                {
                    m.Property(x => x.ProductId, c => c.Column("product_id"));
                    m.Property(x => x.LanguageCode, c => c.Column("language_code"));
                });


            SqlInsert(save);
            SqlUpdate(save);

            Property(x => x.ProductName, c => c.Column("product_name"));
            Property(x => x.ProductDescription, c => c.Column("product_description"));
            Property(x => x.ActualLanguageCode, c => c.Column("actual_language_code"));

            
        }
    }
}