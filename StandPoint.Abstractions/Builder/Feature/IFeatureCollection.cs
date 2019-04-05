using System.Collections.Generic;

namespace StandPoint.Abstractions.Builder.Feature
{
    public interface IFeatureCollection
    {
        List<IFeatureRegistration> FeatureRegistrations { get; }

        IFeatureRegistration AddFeature<TImplementation>() where TImplementation : class, IFeature;
    }
}
