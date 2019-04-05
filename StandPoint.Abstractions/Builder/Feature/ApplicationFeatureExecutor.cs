using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StandPoint.Utilities;

namespace StandPoint.Abstractions.Builder.Feature
{
    /// <summary>
    /// Borrowed from asp.net
    /// </summary>
    public class ApplicationFeatureExecutor : IFeatureExecutor
    {
        private readonly IApplication _application;
        private readonly ILogger<Application> _logger;

        public ApplicationFeatureExecutor(IApplication application, ILogger<Application> logger)
        {
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(logger, nameof(logger));

            this._application = application;
            this._logger = logger;
        }

        public ApplicationFeatureExecutor(Application application, ILogger<Application> logger) 
            : this(application as IApplication, logger)
        {
            
        }

        public void Start()
        {
            try
            {
                Execute(service => service.Start());
            }
            catch (Exception e)
            {
                this._logger.LogError("An error occurred starting the application", e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                Execute(service => service.Stop());
            }
            catch (Exception e)
            {
                this._logger.LogError("An error occurred stopping the application", e);
                throw;
            }
        }

        private void Execute(Action<IFeature> callback)
        {
            List<Exception> exceptions = null;

            if (_application.Services != null)
            {
                foreach (var service in _application.Services.Features)
                {
                    try
                    {
                        callback(service);
                    }
                    catch (Exception e)
                    {
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }

                        exceptions.Add(e);
                    }
                }

                // Throw an aggregate exception if there were any exceptions
                if (exceptions != null)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }
    }
}
