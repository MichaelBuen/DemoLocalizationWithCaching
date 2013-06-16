using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LocalizationWithCaching.Models;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace LocalizationWithCaching.ModelMappings
{
    public class ProductLanguageMapping : ClassMapping<ProductLanguage>
    {
        static string context = "convert(nvarchar, substring(context_info(), 5, convert(int, substring(context_info(), 1, 4)) )  )";
        string save =
string.Format(@"
merge product_i18n as dst
using(values(?,?,?, {0} )) 
	as src(product_name, product_description, product_id, language_code)
on
	src.product_id = dst.product_id and src.language_code = dst.language_code

when matched then
	update set dst.product_name = src.product_name, dst.product_description = src.product_description
when not matched then
	insert (product_id, language_code, product_name, product_description)
	values (src.product_id, src.language_code, src.product_name, src.product_description);", context);


        public ProductLanguageMapping()
        {
            Table(string.Format("dbo.get_product_i18n({0})", context));


            // Need to be turned on, so N+1 won't happen
            // http://stackoverflow.com/questions/8761249/how-do-i-make-nhibernate-cache-fetched-child-collections
            Cache(x => x.Usage(CacheUsage.ReadWrite));

            ComponentAsId(key => key.ProductLanguageCompositeKey, m =>
                {                                      
                    m.Property(x => x.ProductId, c => c.Column("product_id"));
                    m.Property(x => x.LanguageCode, c => c.Column("language_code"));                    
                });            

            //Property(x => x.ProductId, c => c.Column("product_id"));
            //Property(x => x.LanguageCode, c => c.Column("language_code"));

            SqlInsert(save);
            SqlUpdate(save);

            Property(x => x.ProductName, c => c.Column("product_name"));
            Property(x => x.ProductDescription, c => c.Column("product_description"));
            
        }
    }
}