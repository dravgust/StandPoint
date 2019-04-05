using System;
using System.Collections.Generic;
using StandPoint.Abstractions.Builder.Feature;
using StandPoint.Utilities;

namespace StandPoint.Abstractions.Builder
{
    /// <summary>
    /// Provider of access to services and features registered with the application.
    /// </summary>
    public class ApplicationServiceProvider : IApplicationServiceProvider
    {
        /// <summary>List of registered feature types.</summary>
        private readonly List<Type> _featureTypes;

        /// <inheritdoc />
        public IEnumerable<IFeature> Features
        {
            get
            {
                // features are enumerated in the same order 
                // they where registered with the provider
                foreach (var featureDescriptor in this._featureTypes)
                    yield return this.ServiceProvider.GetService(featureDescriptor) as IFeature;
            }
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Initializes a new instance of the object with service provider and list of registered feature types.
        /// </summary>
        /// <param name="serviceProvider">Provider to registered services.</param>
        /// <param name="featureTypes">List of registered feature types.</param>
        public ApplicationServiceProvider(IServiceProvider serviceProvider, List<Type> featureTypes)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));
            Guard.NotNull(featureTypes, nameof(featureTypes));

            this.ServiceProvider = serviceProvider;
            _featureTypes = featureTypes;
        }
    }
}
