using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using LocalizationWithCaching.ModelMappings;
using NHibernate.Engine;
using NHibernate.Cfg.MappingSchema;

using LocalizationWithCaching.NHibernateHelper;

namespace LocalizationWithCaching.Mapper
{
    public class TheMapper
    {
        static ISessionFactory _sessionFactory;
        public static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory != null)
                return _sessionFactory;

            var cfg = new Configuration();
            var mapper = new ModelMapper();

            mapper.AddMappings(
                new[]
			    {
                    // Entities
					typeof(ProductMapping), 
                    typeof(ProductLanguageMapping),
                    typeof(OrderedProductMapping),
                    

                    // TVFs
                    typeof(GetOrdersInfoMapping), 
                    typeof(GetProductSoldMapping)
				});



            cfg.DataBaseIntegration(c =>
            {
                c.Driver<NHibernate.Driver.Sql2008ClientDriver>();
                c.Dialect<NHibernate.Dialect.MsSql2012Dialect>();
                c.ConnectionString = "Server=localhost; Database=good_db_x; Trusted_Connection=true;";

                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
            });




            cfg.Cache(x =>
                {
                    x.Provider<NHibernate.Caches.SysCache.SysCacheProvider>();

                    // http://stackoverflow.com/questions/2365234/how-does-query-caching-improves-performance-in-nhibernate

                    // Need to be explicitly turned on so the .Cacheable directive on Linq will work:                    
                    x.UseQueryCache = true;
                });




            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(domainMapping);


            var filterDef =
                new FilterDefinition(
                    "lf", null,
                    new Dictionary<string, NHibernate.Type.IType> 
                    { 
                        { "LanguageCode", NHibernateUtil.String }                        
                    }, useManyToOne: false);

            cfg.AddFilterDefinition(filterDef);

      


            _sessionFactory = cfg.BuildSessionFactory();

            return _sessionFactory;
        }


    }


    
}