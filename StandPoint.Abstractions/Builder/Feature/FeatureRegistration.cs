using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StandPoint.Utilities;

namespace StandPoint.Abstractions.Builder.Feature
{
    public class FeatureRegistration<TImplementation> : IFeatureRegistration where TImplementation : class , IFeature
    {
        public readonly List<Action<IServiceCollection>> ConfigureServicesDelegates;

        public Type FeatureStartupType { get; private set; }
        public Type FeatureType { get; private set; }

        public FeatureRegistration()
        {
            this.ConfigureServicesDelegates = new List<Action<IServiceCollection>>();
            this.FeatureType = typeof(TImplementation);
        }

        public void BuildFeature(IServiceCollection serviceCollection)
        {
            Guard.NotNull(serviceCollection, nameof(serviceCollection));

            //feature can only be singleton
            serviceCollection
                .AddSingleton(FeatureType)
                .AddSingleton(typeof(IFeature), provider => provider.GetService(FeatureType));

            foreach (var configureServicesDelegate in this.ConfigureServicesDelegates)
                configureServicesDelegate(serviceCollection);

            if (FeatureStartupType != null)
                FeatureStartup(serviceCollection, FeatureStartupType);
        }

        public IFeatureRegistration FeatureServices(Action<IServiceCollection> configureServices)
        {
            Guard.NotNull(configureServices, nameof(configureServices));

            this.ConfigureServicesDelegates.Add(configureServices);

            return this;
        }

        public IFeatureRegistration UseStartup<TStartup>()
        {
            FeatureStartupType = typeof(TStartup);

            return this;
        }

        /// <summary>
        ///     A feature can use specified method to configure its services
        ///     The specified method needs to have the following signature to be invoked
        ///     void ConfigureServices(IServiceCollection serviceCollection)
        /// </summary>
        private void FeatureStartup(IServiceCollection serviceCollection, Type startupType)
        {
            var method = startupType.GetMethod("ConfigureServices");
            var parameters = method?.GetParameters();
            if (method != null && method.IsStatic && (parameters?.Length == 1) && (parameters.First().ParameterType == typeof(IServiceCollection)))
                method.Invoke(null, new object[] { serviceCollection });
        }
    }
}
