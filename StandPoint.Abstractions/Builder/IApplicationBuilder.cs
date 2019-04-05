﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StandPoint.Abstractions.Builder.Feature;

namespace StandPoint.Abstractions.Builder
{
    /// <summary>
    /// A builder for IApplication.
    /// </summary>
    public interface IApplicationBuilder
    {
        /// <summary>
        /// Builds an IApplication.
        /// </summary>
        IApplication Build();

        /// <summary>
        /// Specify the delegate that is used to configure the services of the application.
        /// </summary>
        /// <param name="configureServices">The delegate that configures the IServiceCollection.</param>
        /// <returns>The IApplicationBuilder.</returns>
        IApplicationBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IApplicationBuilder ConfigureServiceProvider(Action<IServiceProvider> configure);

        IApplicationBuilder ConfigureFeature(Action<IFeatureCollection> configureFeatures);

        /// <summary>
        /// Adds a delegate for configuring the provided ILoggerFactory. This may be called multiple times.
        /// </summary>
        /// <param name="configureLogging">The delegate that configures the ILoggerFactory.</param>
        /// <returns>The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" />.</returns>
        IApplicationBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);

        /// <summary>
        /// Specify the <see cref="T:Microsoft.Extensions.Logging.ILoggerFactory" /> to be used by the web host.
        /// </summary>
        /// <param name="loggerFactory">The ILoggerFactory to be used.</param>
        /// <returns>The IApplicationBuilder.</returns>
        IApplicationBuilder UseLoggerFactory(ILoggerFactory loggerFactory);

        /// <summary>Add or replace a setting in the configuration.</summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The IApplicationBuilder.</returns>
        IApplicationBuilder UseSetting(string key, string value);

        /// <summary>Get the setting value from the configuration.</summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        string GetSetting(string key);
    }
}
