//using System;
//using System.Collections.Generic;
//using Codersband.Abstractions;
//using Codersband.Abstractions.Builder;
//using Codersband.Abstractions.Builder.Feature;
//using Microsoft.Extensions.DependencyInjection;

//namespace ApplicationHost.Test
//{
//    public class BaseFeature : ApplicationFeature
//    {
//        private readonly List<IDisposable> _disposableResources = new List<IDisposable>();

//        public BaseFeature()
//        {

//        }

//        public override void Start()
//        {
            
//        }

//        public override void Stop()
//        {
//            foreach (var disposable in _disposableResources)
//            {
//                disposable.Dispose();
//            }
//        }
//    }

//    internal static class BaseFeatureBuilderExtension
//    {
//        public static IApplicationBuilder UseBaseFeature(this IApplicationBuilder applicationBuilder)
//        {
//            applicationBuilder.ConfigureFeature(features =>
//            {
//                features
//                    .AddFeature<BaseFeature>()
//                    .FeatureServices(services =>
//                    {
//                        services.AddSingleton<IApplicationLifeTime, ApplicationLifeTime>();
//                        services.AddSingleton<Application>();
//                    });
//            });

//            return applicationBuilder;
//        }
//    }
//}
