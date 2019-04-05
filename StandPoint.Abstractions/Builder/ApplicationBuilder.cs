using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StandPoint.Abstractions.Builder.Feature;
using StandPoint.Abstractions.Configuration;
using StandPoint.Utilities;

namespace StandPoint.Abstractions.Builder
{
    /// <summary>
    /// A builder for IApplication />
    /// </summary>
    public class ApplicationBuilder : IApplicationBuilder
    {
        private readonly List<Action<IServiceProvider>> _configureDelegates;
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;
        private readonly List<Action<IFeatureCollection>> _featuresRegistrationDelegates;
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates;
        private readonly IConfiguration _configuration;
        private ILoggerFactory _loggerFactory;
        private bool _applicationBuilt;



        public ApplicationBuilder() :
            this(new List<Action<IServiceCollection>>(),
                new List<Action<IServiceProvider>>(),
                new List<Action<IFeatureCollection>>(),
                new List<Action<ILoggerFactory>>())
        {

        }

        internal ApplicationBuilder(List<Action<IServiceCollection>> configureServicesDelegates, List<Action<IServiceProvider>> configureDelegates,
            List<Action<IFeatureCollection>> featuresRegistrationDelegates, List<Action<ILoggerFactory>> configureLoggingDelegates)
        {
            Guard.NotNull(configureServicesDelegates, nameof(configureServicesDelegates));
            Guard.NotNull(configureDelegates, nameof(configureDelegates));
            Guard.NotNull(featuresRegistrationDelegates, nameof(featuresRegistrationDelegates));
            Guard.NotNull(configureLoggingDelegates, nameof(configureLoggingDelegates));

            this._configureDelegates = configureDelegates;
            this._configureServicesDelegates = configureServicesDelegates;
            this._featuresRegistrationDelegates = featuresRegistrationDelegates;
            this._configureLoggingDelegates = configureLoggingDelegates;
            this._configuration = (IConfiguration) new ConfigurationBuilder()
                .AddEnvironmentVariables("FULLNODE_")
                .Build();
        }

        /// <summary>
        /// Specify the ILoggerFactory to be used by the web host.
        /// </summary>
        /// <param name="loggerFactory">The ILoggerFactory to be used.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public IApplicationBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            this._loggerFactory = loggerFactory;
            return (IApplicationBuilder)this;
        }

        /// <summary>
        /// Adds a delegate for configuring the provided ILoggerFactory. This may be called multiple times.
        /// </summary>
        /// <param name="configureLogging">The delegate that configures the ILoggerFactory.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public IApplicationBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            Guard.NotNull(configureLogging, nameof(configureLogging));

            this._configureLoggingDelegates.Add(configureLogging);
            return (IApplicationBuilder)this;
        }

        /// <summary>
        /// Adds features to the builder. 
        /// </summary>
        /// <param name="configureFeatures">A method that adds features to the collection</param>
        /// <returns>An IApplicationBuilder</returns>
        public IApplicationBuilder ConfigureFeature(Action<IFeatureCollection> configureFeatures)
        {
            Guard.NotNull(configureFeatures, nameof(configureFeatures));

            _featuresRegistrationDelegates.Add(configureFeatures);
            return (IApplicationBuilder)this;
        }

        /// <summary>
		/// Add configurations for the service provider.
		/// </summary>
		/// <param name="configure">A method that configures the service provider.</param>
		/// <returns>An IApplicationBuilder</returns>
		public IApplicationBuilder ConfigureServiceProvider(Action<IServiceProvider> configure)
        {
            Guard.NotNull(configure, nameof(configure));

            _configureDelegates.Add(configure);
            return (IApplicationBuilder)this;
        }

        /// <summary>Add or replace a setting in the configuration.</summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public IApplicationBuilder UseSetting(string key, string value)
        {
            this._configuration[key] = value;
            return (IApplicationBuilder)this;
        }

        // <summary>Get the setting value from the configuration.</summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        public string GetSetting(string key)
        {
            return this._configuration[key];
        }

        /// <summary>
		/// Adds services to the builder. 
		/// </summary>
		/// <param name="configureServices">A method that adds services to the builder</param>
		/// <returns>An IFullNodebuilder</returns>
        public IApplicationBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            Guard.NotNull(configureServices, nameof(configureServices));

            _configureServicesDelegates.Add(configureServices);
            return (IApplicationBuilder)this;
        }

        public IApplication Build()
        {
            if (_applicationBuilt)
                throw new InvalidOperationException("The Application already built");
            _applicationBuilt = true;

            (var services, var features) = this.BuildServicesAndFeatures();

            var serviceProvider = services.BuildServiceProvider();
            this.ConfigureServices(serviceProvider);

            var application = serviceProvider.GetService<Application>();
            if (application == null)
                throw new InvalidOperationException($"{nameof(Application)} not registered with provider");

            application.Initialize(new ApplicationServiceProvider(serviceProvider,
                features.FeatureRegistrations.Select(s => s.FeatureType).ToList()));

            return application;
        }

        /// <summary>
        /// Constructs and configures services ands features to be used by the application.
        /// </summary>
        /// <returns>Collection of registered services and features.</returns>
        private (IServiceCollection services, IFeatureCollection features) BuildServicesAndFeatures()
        {
            var services = (IServiceCollection) new ServiceCollection();

            if (this._loggerFactory == null)
            {
                this._loggerFactory = (ILoggerFactory) new LoggerFactory();
            }
            services.AddSingleton<ILoggerFactory>(this._loggerFactory);

            // configure logger before services
            foreach (var configureLogging in this._configureLoggingDelegates)
                configureLogging(this._loggerFactory);

            services.AddLogging();

            services.AddSingleton<IConfiguration>(this._configuration);

            //configure options
            services.AddOptions();
            services.AddSingleton(new ApplicationOptions(this._configuration));

            // register services before features 
            // as some of the features may depend on independent services
            foreach (var configureServices in this._configureServicesDelegates)
                configureServices(services);

            var features = (IFeatureCollection)new FeatureCollection();
            // configure features
            foreach (var configureFeature in this._featuresRegistrationDelegates)
                configureFeature(features);

            // configure features startup
            foreach (var featureRegistration in features.FeatureRegistrations)
                featureRegistration.BuildFeature(services);

            return (services, features);
        }

        /// <summary>
        /// Configure registered services.
        /// </summary>
        /// <param name="serviceProvider"></param>
        private void ConfigureServices(IServiceProvider serviceProvider)
        {
            // configure registered services
            foreach (var configure in _configureDelegates)
                configure(serviceProvider);
        }
    }
}
