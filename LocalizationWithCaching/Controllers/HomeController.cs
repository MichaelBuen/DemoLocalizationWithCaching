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
        //
        // GET: /Home/


        public string GetCount()
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, "en"))
            {
                return session.Query<Product>().Cacheable().Count().ToString() + " " + session.Query<ProductLanguage>().Cacheable().Count().ToString();
            }
        }


        public ActionResult Index()
        {
            TestCachingSecond();

            return View();
            
        }


        void TestQueryCache(string languageCode)
        {            
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                // query first run. database roundtrip
                var query =
                            (from q in
                                 from p in session.Query<Product>()
                                 join l in session.Query<ProductLanguage>() on p.ProductId equals l.ProductLanguageCompositeKey.ProductId
                                 select new { p, l }
                             where q.l.ProductLanguageCompositeKey.LanguageCode == languageCode
                             select q).Cacheable();

                var t = query.ToList();
            }
        }

        void TestCachingSecond()
        {
            TestQueryCache("en"); // database hit
            TestQueryCache("zh"); // database hit
            TestQueryCache("en"); // query cache cache hit
            TestQueryCache("zh"); // query cache hit

            TestProductEntityCache(productId: 1,languageCode: "en"); // cache hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "en"); // cache hit
            TestProductEntityCache(productId: 2, languageCode: "zh"); // cache hit
            TestProductLanguageEntityCache(productId: 2, languageCode: "en"); // cache hit

            UpdateProduct(productId: 1, languageCode: "en"); // database hit. invalidates query cache, refresh entity cache
            TestProductEntityCache(productId: 1, languageCode: "en"); // cache hit
            
            UpdateProduct(productId: 1, languageCode: "en"); // database hit. invalidates query cache, refresh entity cache
            TestQueryCache("en"); // database hit
            TestQueryCache("en"); // query cache hit

            // not in database, hence no entity cache can be cached by query. database hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "ca");
            // cache hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "ca"); 
            
            

        }

        private void UpdateProductLanguage(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
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
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
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



      



    }//Index
}
