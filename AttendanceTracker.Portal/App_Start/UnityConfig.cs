using Common.DataAccess.Mongo.Connection;
using Microsoft.Practices.Unity;
using SfdcConnector.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SfdcConnector.App_Start
{
    public class UnityConfig
    {

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            // Configurações do UnityContainer

            container.RegisterType<IConnect, Connect>(new InjectionConstructor());

            container.RegisterType<RepositoryCaseContract, RepositoryCase>();
        }
    }
}