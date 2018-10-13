using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Serilog.Core;
using Unity;
using Serilog.Events;

namespace DemoWebApp
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            var storage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["storagekey"]);

            var log = new LoggerConfiguration().ReadFrom.KeyValuePairs(new KeyValuePair<string, string>[]
                {
                   new KeyValuePair<string, string>("OutputTemplate" ,"{Timestamp:yyyy-MM-dd HH:mm:ss zzz} [{Level}] {RequestId}-{SourceContext}: {Message}{NewLine}{Exception} {ActivityId}")
                })
                .WriteTo.AzureTableStorageWithProperties(storage, storageTableName: "LogEventTable", propertyColumns: new[] { "ActivityId" }).Enrich.With(new SeriLogEncricher())
                .MinimumLevel.Warning()
                .CreateLogger();
            container.RegisterInstance<ILogger>(log);

        }
    }

    public class SeriLogEncricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ActivityId", Trace.CorrelationManager.ActivityId, false));
        }
    }
}