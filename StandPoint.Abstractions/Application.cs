using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StandPoint.Abstractions.Builder;
using StandPoint.Abstractions.Builder.Feature;
using StandPoint.Abstractions.Configuration;
using StandPoint.Utilities;

namespace StandPoint.Abstractions
{
    public class Application : IApplication
    {
        /// <summary>Application life cycle control - triggers when application shuts down.</summary>
        private ApplicationLifeTime _applicationLifeTime;

        /// <inheritdoc />
        public IApplicationLifeTime ApplicationLifeTime
        {
            get { return this._applicationLifeTime; }
            set { this._applicationLifeTime = (ApplicationLifeTime) value; }
        }

        /// <summary>Indicates whether the application has been stopped or is currently being stopped.</summary>
        internal bool Stopped;

        /// <summary>Indicates whether the application's instance has been disposed or is currently being disposed.</summary>
        public bool IsDisposed { get; private set; }

        /// <summary>Indicates whether the node's instance disposal has been finished.</summary>
        public bool HasExited { get; private set; }

        /// <summary>Component responsible for starting and stopping all the application's features.</summary>
        private ApplicationFeatureExecutor _applicationFeatureExecutor;

        /// <summary>List of disposable resources that the application uses.</summary>
        public List<IDisposable> Resources { get; private set; }

        /// <summary>Instance logger.</summary>
        private ILogger<Application> _logger;

        private IConfiguration _configuration;
        public IConfiguration Configuration
        {
            get
            {
                return this._configuration;
            }
        }

        private ApplicationOptions _options;
        internal ApplicationOptions Options
        {
            get
            {
                return this._options;
            }
        }

        /// <summary>Factory for creating and execution of asynchronous loops.</summary>
        public IAsyncLoopFactory AsyncLoopFactory { get; set; }

        /// <inheritdoc />
        public IApplicationServiceProvider Services { get; set; }

        public T ApplicationService<T>(bool faiWithDefault = false)
        {
            if (this.Services != null && this.Services.ServiceProvider != null)
            {
                var service = this.Services.ServiceProvider.GetService<T>();
                if (service != null)
                    return service;
            }

            if (faiWithDefault)
                return default(T);

            throw new InvalidOperationException($"The {typeof(T)} service is not supported");
        }

        public T ApplicationFeature<T>(bool faiWithDefault = false)
        {
            if (this.Services != null)
            {
                var feature = this.Services.Features.OfType<T>().FirstOrDefault();
                if (feature != null)
                    return feature;
            }

            if (faiWithDefault)
                return default(T);

            throw new InvalidOperationException($"The {typeof(T)} feature is not supported");
        }

        /// <summary>
        /// Initializes DI services that the node needs.
        /// </summary>
        /// <param name="serviceProvider">Provider of DI services.</param>
        /// <returns>Full node itself to allow fluent code.</returns>
        public Application Initialize(IApplicationServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            this.Services = serviceProvider;

            _logger = this.Services.ServiceProvider.GetRequiredService<ILogger<Application>>();

            _configuration = this.Services.ServiceProvider.GetService<IConfiguration>();
            _options = this.Services.ServiceProvider.GetService<ApplicationOptions>();

            this.AsyncLoopFactory = this.Services.ServiceProvider.GetService<IAsyncLoopFactory>();

            this._logger.LogInformation($"Application initialized.");

            return this;
        }

        /// <inheritdoc />
        public void Start()
        {
            if(IsDisposed)
                throw new ObjectDisposedException(nameof(Application));

            if (this.Resources != null)
                throw new InvalidOperationException("node has already started.");

            this.Resources = new List<IDisposable>();

            this._applicationLifeTime = this.Services?.ServiceProvider.GetRequiredService<IApplicationLifeTime>() as ApplicationLifeTime;
            if (this._applicationLifeTime == null)
                throw new InvalidOperationException($"{nameof(IApplicationLifeTime)} must be set.");

            this._applicationFeatureExecutor = this.Services?.ServiceProvider.GetRequiredService<ApplicationFeatureExecutor>();
            if (this._applicationFeatureExecutor == null)
                throw new InvalidOperationException($"{nameof(ApplicationFeatureExecutor)} must be set.");

            this._logger.LogInformation("Starting application...");

            //start all registered features
            this._applicationFeatureExecutor.Start();

            // Fire INodeLifetime.Started.
            this._applicationLifeTime.NotifyStarted();

            this.StartPeriodicLog();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if(this.Stopped) return;

            this.Stopped = true;

            this._logger.LogInformation("Closing application pending...");

            // Fire IApplicationLifeTime.Stopping.
            this._applicationLifeTime.StopApplication();

            foreach (IDisposable dispo in this.Resources)
                dispo.Dispose();

            // Fire the IHostedService.Stop
            this._applicationFeatureExecutor?.Stop();
            (this.Services.ServiceProvider as IDisposable)?.Dispose();

            // Fire IApplicationLifeTime.Stopped.
            this._applicationLifeTime.NotifyStopped();
        }

        /// <summary>
        /// Starts a loop to periodically log statistics about application's status very couple of seconds.
        /// <para>
        /// These logs are also displayed on the console.
        /// </para>
        /// </summary>
        private void StartPeriodicLog()
        {
            IAsyncLoop pereodicLogLoop = this.AsyncLoopFactory.Run("PereodicLog", (cancellation) =>
            {
                var benchLogs = new StringBuilder();

                benchLogs.AppendLine("======Application stats====== " + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

                // Display node stats grouped together.
                foreach (var feature in this.Services.Features.OfType<IApplicationStats>())
                    feature.AddApplicationStats(benchLogs);

                // Now display the other stats.
                foreach (var feature in this.Services.Features.OfType<IFeatureStats>())
                    feature.AddFeatureStats(benchLogs);

                benchLogs.AppendLine();

                this._logger.LogInformation(benchLogs.ToString());

                return Task.CompletedTask;
            },
            _applicationLifeTime.ApplicationStopping,
            repeatEvery: TimeSpans.FiveSeconds,
            startAfter: TimeSpans.FiveSeconds);

            this.Resources.Add(pereodicLogLoop);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if(this.IsDisposed) return;

            this.IsDisposed = true;

            if (!this.Stopped)
            {
                try
                {
                    this.Stop();
                }
                catch (Exception ex)
                {
                    this._logger?.LogError(ex.Message);
                }
            }

            this.HasExited = true;
        }
    }
}
