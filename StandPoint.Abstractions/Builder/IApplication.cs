using System;
using Microsoft.Extensions.Configuration;

namespace StandPoint.Abstractions.Builder
{
    /// <inheritdoc />
    /// <summary>Represents a configured Application.</summary>
    public interface IApplication : IDisposable
    {
        /// <summary>Global application life cycle control - triggers when application shuts down.</summary>
        IApplicationLifeTime ApplicationLifeTime { get; }

        /// <summary>
        /// The IConfigurationRoot of the Application.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// The IApplicationServiceProvider for the Application.
        /// </summary>
        IApplicationServiceProvider Services { get; }

        /// <summary>
        /// Start Application and its features.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the full node and all its features.
        /// </summary>
        void Stop();
    }
}
