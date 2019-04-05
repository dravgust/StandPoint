namespace StandPoint.Abstractions.Builder.Feature
{
    /// <summary>
    /// A feature is used to extend functionality into the application
    /// It can manage its life time or use the application disposable resources.

    /// If a feature is added functionality available to use by the application
    /// (but may be disabled/enabled by the configuration) the naming convention is
    /// Add[Feature](). Conversely, when a features is inclined to be used if included,
    /// the naming convention should be Use[Feature]()

    /// </summary>
    public abstract class ApplicationFeature : IFeature
    {
        public abstract void Start();

        public virtual void Stop()
        {

        }
    }
}
