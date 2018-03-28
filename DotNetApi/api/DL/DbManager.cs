using System;
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Environment = NHibernate.Cfg.Environment;

namespace DL
{
    public static class DbManager
    {
        private static ISessionFactory _sessionFactory;

        public static void StartDb()
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008
                                  .ConnectionString(Tools.Constants.GetConnection()).ShowSql())
                    .Mappings(m =>
                                  {
                                      m.FluentMappings.AddFromAssemblyOf<DM.Models.System.Code>();
                                      m.FluentMappings.Conventions.AddFromAssemblyOf<DM.Models.System.Code>();
                                  })
                    .ExposeConfiguration(c => c.SetProperty(Environment.TransactionStrategy,
                                                            "NHibernate.Transaction.AdoNetTransactionFactory"))
                    .BuildSessionFactory();

            }
        }

        public static ISession GetSession()
        {
            if (_sessionFactory == null)
            {
                StartDb();
            }
            return SessionFactory.OpenSession();
        }

        public static DateTime GetServerDateTime(bool withTime = true)
        {
            ISession session;
            using (session = GetSession())
            {
                var q = session.CreateSQLQuery("select getdate()");
                var toRet = q.UniqueResult<DateTime>();
                session.Flush();
                return !withTime ? toRet.Date : toRet;
            }
        }

        public static long GetNextId(string Id)
        {
            ISession session;
            using (session = GetSession())
            {
                var q = session.CreateSQLQuery("select Sequence from _Identity Where Id = '" + Id + "'");
                var toRet = q.UniqueResult<long>();
                q = session.CreateSQLQuery("Update _Identity set Sequence = Sequence +1 Where Id = '" + Id + "'");
                q.ExecuteUpdate();
                session.Flush();
                return toRet;
            }
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory != null) return _sessionFactory;
                _sessionFactory = DbConfiguration.BuildSessionFactory();
                return _sessionFactory;
            }
        }

        private static Configuration DbConfiguration
        {
            get
            {
                var cfg = new Configuration();
                cfg.Configure();
                var a = typeof(DM.Models.System.Code).Assembly;
                cfg.AddAssembly(a);
                return cfg;
            }
        }
    }
}