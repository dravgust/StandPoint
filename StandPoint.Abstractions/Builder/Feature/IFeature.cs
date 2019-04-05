namespace StandPoint.Abstractions.Builder.Feature
{
    /// <summary>
    /// Defines methods for features that are managed by the Application.
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Triggered when the Application has fully started
        /// </summary>
        void Start();

        /// <summary>
        /// Triggered when the Application is performing a graceful shutdown.
        /// Requests may still be in flight. Shutdown will block until this event completes.
        /// </summary>
        void Stop();
    }
}
