﻿using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using AcklenAvenue.Data.NHibernate;
using AutoMapper;
using BootstrapMvcSample;
using BootstrapSupport;
using FluentNHibernate.Cfg.Db;
using Galeria.Data;
using Galeria.Domain.Services;
using Galeria.Web.App_Start;
using Galeria.Web.Infrastructure;
using NHibernate;
using NHibernate.Context;
using Ninject;
using Ninject.Web.Common;


namespace Galeria.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : NinjectHttpApplication
    {
        //protected void Application_Start()
        //{
        //    AreaRegistration.RegisterAllAreas();
        //    WebApiConfig.Register(GlobalConfiguration.Configuration);
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BootstrapSupport.BootstrapBundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
        //    BootstrapMvcSample.ExampleLayoutsRouteConfig.RegisterRoutes(RouteTable.Routes);
        //}

        
        public static ISessionFactory SessionFactory = CreateSessionFactory();
        
        public MvcApplication()
        {
            BeginRequest += MvcApplication_BeginRequest;
            EndRequest += MvcApplication_EndRequest;
        }

        private void MvcApplication_EndRequest(object sender, EventArgs e)
        {//
            //CurrentSessionContext.Unbind(SessionFactory).Dispose();
        }

        private void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
            
        }

        public static ISessionFactory CreateSessionFactory()
        {
            
            
            MsSqlConfiguration databaseConfiguration = MsSqlConfiguration.MsSql2008.ShowSql().
                                                                          ConnectionString(
                                                                              x =>
                                                                              x.FromConnectionStringWithKey(
                                                                                  "Galeria.Remote"));
            ISessionFactory sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build();

            return sessionFactory;
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            AreaRegistration.RegisterAllAreas();
            FluentSecurityConfig.Configure();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BootstrapBundleConfig.RegisterBundles(BundleTable.Bundles);
            ExampleLayoutsRouteConfig.RegisterRoutes(RouteTable.Routes);
            AutoMapperConfiguration.Configure();
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Load(Assembly.GetCallingAssembly());

            kernel.Bind<IReadOnlyRepository>().To<ReadOnlyRepository>();
            kernel.Bind<IWriteOnlyRepository>().To<WriteOnlyRepository>();
            kernel.Bind<ISession>().ToMethod(x => SessionFactory.GetCurrentSession());
            kernel.Bind<IMappingEngine>().ToConstant(Mapper.Engine);
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

            return kernel;
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (Context.User != null)
            {
                string cookieName = FormsAuthentication.FormsCookieName;

                HttpCookie authCookie = Context.Request.Cookies[cookieName];
                if (authCookie == null)
                    return;


                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);


                string[] roles = authTicket.UserData.Split(new[] { '|' });
                var fi = (FormsIdentity)(Context.User.Identity);
                Context.User = new GenericPrincipal(fi, roles);

                
            }
        }
    }
}