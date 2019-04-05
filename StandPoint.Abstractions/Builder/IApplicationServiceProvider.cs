using System;
using System.Collections.Generic;
using StandPoint.Abstractions.Builder.Feature;

namespace StandPoint.Abstractions.Builder
{
    /// <summary>
    /// Provider of access to services and features registered with the application.
    /// </summary>
    public interface IApplicationServiceProvider
    {
        /// <summary>List of registered features.</summary>
        IEnumerable<IFeature> Features { get; }

        /// <summary>Provider to registered services.</summary>
        IServiceProvider ServiceProvider { get; }
    }
}
