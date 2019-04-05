using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace StandPoint.Abstractions.Builder.Feature
{
    public class ApplicationBaseFeature : ApplicationFeature
    {
        private readonly List<IDisposable> _disposableResources = new List<IDisposable>();

        public ApplicationBaseFeature()
        {

        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            foreach (var disposable in _disposableResources)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Makes the application use all the required features - <see cref="ApplicationBaseFeature"/>.
        /// </summary>
        /// <param name="applicationBuilder">Builder responsible for creating the application.</param>
        /// <returns>Application builder's interface to allow fluent code.</returns>
        public static IApplicationBuilder UseBaseFeature(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<ApplicationBaseFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IApplicationLifeTime, ApplicationLifeTime>();
                        services.AddSingleton<ApplicationFeatureExecutor>();
                        services.AddSingleton<Application>();
                        services.AddSingleton<IDateTimeProvider>(DateTimeProvider.Default);
                        services.AddSingleton<IAsyncLoopFactory, AsyncLoopFactory>();
                    });
            });

            return applicationBuilder;
        }
    }
}
