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
            var cmd = session.Connection.CreateCommand();
            tx.Enlist(cmd);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SetLanguage";
            var prm = cmd.CreateParameter();
            prm.ParameterName = "@LanguageCode";
            prm.Value = languageCode;
            cmd.Parameters.Add(prm);
            cmd.ExecuteNonQuery();

            session.EnableFilter("lf").SetParameter("LanguageCode", languageCode);

            return tx;
        }
    }
}