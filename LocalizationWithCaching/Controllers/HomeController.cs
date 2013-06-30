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
            TestProductAndLanguageQueryCache("en"); // database hit
            TestProductAndLanguageQueryCache("zh"); // database hit
            TestProductAndLanguageQueryCache("en"); // cached query hit
            TestProductAndLanguageQueryCache("zh"); // cached query hit
            TestProductAndLanguageQueryCache("ca"); // database hit
            

            TestTvfGetOrderInfoQueryCache("en"); // database hit
            TestTvfGetOrderInfoQueryCache("en"); // cached query hit
            TestTvfGetOrderInfoQueryCache("zh"); // database hit
            TestTvfGetOrderInfoQueryCache("zh"); // cached query hit

                        
            TestTvfGetOrderInfoQueryCache("en"); // cached query hit
            UpdateProduct(productId: 1, languageCode: "en"); // cached entity hit on entity get. database hit on update. refresh entity cache. Invalidates GetOrderInfo query cache
            TestTvfGetOrderInfoQueryCache("en"); // database hit
            TestTvfGetOrderInfoQueryCache("en"); // cached query hit
            UpdateProductLanguage(productId: 1, languageCode: "zh"); // cached entity hit on entity get. database hit on update. refresh entity cache. Invalidates GetOrderInfo query cache
            TestTvfGetOrderInfoQueryCache("en"); // database hit. even we only touch the Chinese language above
            TestTvfGetOrderInfoQueryCache("en"); // cached query hit



            TestTvfGetProductSoldQueryCache("en"); // database hit
            UpdateProduct(productId: 1, languageCode: "en"); // cached entity hit on entity get. database hit on update. refresh entity cache. does not invalidates GetProductSold query cache
            TestTvfGetProductSoldQueryCache("en"); // was not invalidated. cached query hit. GetProductSold query cache is Synchronized with ordered_product only
            TestTvfGetProductSoldQueryCache("en"); // cached query hit
            UpdateProductLanguage(productId: 1, languageCode: "zh"); // cached hit on entity get. database hit on update. refresh entity cache. invalidates GetProductSold query cache as it joins on ProductLanguage entity
            TestTvfGetProductSoldQueryCache("en"); // cached query was invalidated. database hit
            TestTvfGetProductSoldQueryCache("en"); // cached query hit


            TestTvfGetProductSoldQueryCache("en"); // cached query hit
            UpdateOrderedProduct(orderedProductId: 1, languageCode: "en"); // database hit on entity get. database hit on update. refresh entity cache. invalidates GetProductSold query cache
            TestOrderedProductEntityCache(orderedProductId: 1, languageCode: "en"); // cached entity hit on entity get
            TestTvfGetProductSoldQueryCache("en"); // database hit
            TestTvfGetProductSoldQueryCache("en"); // cached query hit
            UpdateOrderedProduct(orderedProductId: 1, languageCode: "zh"); // cached entity hit on entity get. database hit on update. refresh entity cache. invalidates GetProductSold query cache
            TestTvfGetProductSoldQueryCache("en"); // database hit. even we only touch the Chinese language above
            TestTvfGetProductSoldQueryCache("en"); // cached query hit
            UpdateOrderedProduct(orderedProductId: 1, languageCode: "en"); // cached entity hit on entity get. database hit on update. refresh entity cache
            TestOrderedProductEntityCache(orderedProductId: 1, languageCode: "en"); // cached entity hit on entity get
            

            
            TestProductEntityCache(productId: 1,languageCode: "en"); // cached entity hit
            TestProductLanguageEntityCache(productId: 1, languageCode: "en"); // cached entity hit
            TestProductEntityCache(productId: 2, languageCode: "zh"); // cached entity hit
            TestProductLanguageEntityCache(productId: 2, languageCode: "en"); // cached entity hit

            UpdateProduct(productId: 1, languageCode: "en"); // cached entity hit on entity get. database hit on update. refresh entity cache
            TestProductEntityCache(productId: 1, languageCode: "en"); // cached entity hit

            UpdateProduct(productId: 1, languageCode: "en"); // cached entity hit on entity get. database hit. refresh entity cache. invalidates cached query
            TestProductAndLanguageQueryCache("en"); // cached query was invalidated. database hit
            TestProductAndLanguageQueryCache("en"); // cached query hit

            UpdateProduct(productId: 1, languageCode: "en"); // cached entity hit on entity get. database hit on update. refresh entity cache. invalidates cached query 
            TestProductEntityCache(productId: 1, languageCode: "en"); // cached entity hit
            TestProductAndLanguageQueryCache("en"); // cached query was invalidated. database hit

            TestProductLanguageEntityCache(productId: 1, languageCode: "ca"); // cached entity hit

            TestProductLanguageEntityCache(productId: 1, languageCode: "es"); // database hit

            TestProductLanguageEntityCache(productId: 1, languageCode: "es"); // cached entity hit

            // cached entity hit on entity get. database hit on update. entity cache is refreshed. invalidates *ALL* language version of ProductLanguage query cache
            UpdateProductLanguage(productId: 1, languageCode: "es");


            TestProductAndLanguageQueryCache("zh"); // was invalidated. database hit
            TestProductAndLanguageQueryCache("es"); // was invalidated. database hit
            TestProductAndLanguageQueryCache("es"); // cached query hit           
            TestProductAndLanguageQueryCache("en"); // was invalidated. database hit            

            TestProductAndLanguageQueryCache("zh"); // cached query hit
            TestProductAndLanguageQueryCache("en"); // cached query hit



            TestProductLanguageEntityCache(productId: 1, languageCode: "es"); // cached entity hit

        }




        void TestProductAndLanguageQueryCache(string languageCode)
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


        private void TestTvfGetOrderInfoQueryCache(string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Query<GetOrdersInfo>().Cacheable();

                var l = x.ToList();
            }
        }

        private void TestTvfGetProductSoldQueryCache(string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = 
                        (from q in 
                             from ps in session.Query<GetProductSold>()
                             join pl in session.Query<ProductLanguage>() on ps.ProductId equals pl.ProductLanguageCompositeKey.ProductId
                             select new { ps, pl }
                        where q.pl.ProductLanguageCompositeKey.LanguageCode == languageCode
                         select q).Cacheable();

                var l = x.ToList();
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

        private void UpdateProductLanguage(int productId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Get<ProductLanguage>(new ProductLanguageCompositeKey { ProductId = productId, LanguageCode = languageCode });

                x.ProductName = string.Concat(x.ProductName.ToCharArray().Reverse());
                x.ActualLanguageCode = languageCode;

                session.Save(x);
                tx.Commit();
            }
        }

        private void UpdateOrderedProduct(int orderedProductId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Get<OrderedProduct>(orderedProductId);
                x.Quantity = x.Quantity + 1;
                session.Save(x);
                tx.Commit();
            }
        }


        private void TestOrderedProductEntityCache(int orderedProductId, string languageCode)
        {
            using (var session = Mapper.TheMapper.GetSessionFactory().OpenSession())
            using (var tx = session.BeginTransaction().SetLanguage(session, languageCode))
            {
                var x = session.Get<OrderedProduct>(orderedProductId);                
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
