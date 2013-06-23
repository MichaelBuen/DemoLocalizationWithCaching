using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LocalizationWithCaching.Models;

using NHibernate.Linq;
using LocalizationWithCaching.ViewModel;

using LocalizationWithCaching.NHibernateHelper;

namespace LocalizationWithCaching.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            TestCachingSecond();

            return View();
            
        }


        
        void TestCachingSecond()
        {
            TestQueryCache("en"); // database hit
            TestQueryCache("zh"); // database hit
            TestQueryCache("en"); // cached query hit
            TestQueryCache("zh"); // cached query hit
            TestQueryCache("ca"); // database hit
            
            TestTvfCache("en"); // database hit
            TestTvfCache("en"); // cached query hit
            TestTvfCache("zh"); // database hit
            TestTvfCache("zh"); // cached query hit
            
            TestTvfCache("en"); // cached query hit
            UpdateProduct(productId: 1, languageCode: "en"); // database hit. refresh entity cache
            TestTvfCache("en"); // database hit
            TestTvfCache("en"); // cached query hit
            UpdateProductLanguage(productId: 1, languageCode: "zh"); // database hit. refresh entity cache
            TestTvfCache("en"); // database hit
            TestTvfCache("en"); // cached query hit


            TestProductEntityCache(productId: 1,languageCode: "en"); // cached entity hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "en"); // cached entity hit
            TestProductEntityCache(productId: 2, languageCode: "zh"); // cached entity hit
            TestProductLanguageEntityCache(productId: 2, languageCode: "en"); // cached entity hit

            UpdateProduct(productId: 1, languageCode: "en"); // database hit. refresh entity cache
            TestProductEntityCache(productId: 1, languageCode: "en"); // cached entity hit

            UpdateProduct(productId: 1, languageCode: "en"); // database hit. refresh entity cache. invalidates cached query
            TestQueryCache("en"); // no cached query. database hit
            TestQueryCache("en"); // cache query hit

            UpdateProduct(productId: 1, languageCode: "en"); // database hit. refresh entity cache. invalidates cached query 
            TestProductEntityCache(productId: 1, languageCode: "en"); // cached entity hit
            TestQueryCache("en"); // no cached query. database hit
             
            
            // cached entity hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "ca");
            
            // database hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "es");

            // cached entity hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "es");

            // database hit. entity is cached
            UpdateProductLanguage(productId: 1, languageCode: "es");
            
            // cached entity hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "es");

            

        }

        void TestQueryCache(string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var query =
                            (from q in
                                 from p in session.Query<Product>()
                                 join l in session.Query<ProductLanguage>() on p.ProductId equals l.ProductLanguageCompositeKey.ProductId
                                 select new { p, l }                             
                             select q).Cacheable();

                var t = query.ToList();
            }
        }


        private void TestTvfCache(string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Query<GetOrdersInfo>().Cacheable();

                var l = x.ToList();
            }
        }

        private void UpdateProductLanguage(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode, readOnly: false))
            {
                var x = session.Get<ProductLanguage>(new ProductLanguageCompositeKey { ProductId = productId, LanguageCode = languageCode });
                
                x.ProductName = string.Concat(x.ProductName.ToCharArray().Reverse());

                session.Save(x);
                tx.Commit();
            }
        }

        private void TestProductLanguageEntityCache(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Get<ProductLanguage>(new ProductLanguageCompositeKey { ProductId = productId, LanguageCode = languageCode } );                
            }
        }

        private void UpdateProduct(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode, readOnly: false))
            {
                var x = session.Get<Product>(productId);
                x.YearIntroduced = x.YearIntroduced + 1;
                session.Save(x);
                tx.Commit();
            }
        }

        private void TestProductEntityCache(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Get<Product>(productId);
            }
        }


        public string GetCount()
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, "en"))
            {
                return session.Query<Product>().Cacheable().Count().ToString() + " " + session.Query<ProductLanguage>().Cacheable().Count().ToString();
            }
        }
      



    }//Index
}
