using System;
using Microsoft.Extensions.DependencyInjection;

namespace StandPoint.Abstractions.Builder.Feature
{
    public interface IFeatureRegistration
    {
        Type FeatureStartupType { get; }
        Type FeatureType { get; }

        void BuildFeature(IServiceCollection serviceCollection);
        IFeatureRegistration FeatureServices(Action<IServiceCollection> configureServices);
        IFeatureRegistration UseStartup<TStartup>();
    }
}
