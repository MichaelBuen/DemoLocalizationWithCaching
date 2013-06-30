using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalizationWithCaching.NHibernateHelper
{
    public static class TheNHibernateHelper
    {
        public static NHibernate.ITransaction SetLanguage(this NHibernate.ITransaction tx, NHibernate.ISession session, string languageCode)
        {
            session.EnableFilter("lf").SetParameter("LanguageCode", languageCode);
            
            return tx;
        }
    }
}