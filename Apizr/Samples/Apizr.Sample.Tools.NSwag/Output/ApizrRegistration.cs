/***************************************************************************************************************
* This is a registration helper class generated by Apizr.Tools.NSwag with both the static builder and a service collection extension method .
* As you asked for both static and extended registration, make sure to get the Apizr.Extensions.Microsoft.DependencyInjection Nuget package installed.
* As you asked for following optional features, please make sure to install each corresponding Nuget packages:
* - Akavache cache provider:      Apizr.Integrations.Akavache
* - Logs:                         any Nuget package of your choice compatible with Microsoft Logging Extensions
* - Priority management:          Apizr.Integrations.Fusillade
* - Data mapping:                 Apizr.Integrations.AutoMapper
* - Mediation:                    Apizr.Integrations.MediatR
* - Optional mediation:           Apizr.Integrations.Optional
****************************************************************************************************************/

using Apizr;
using Microsoft.Extensions.DependencyInjection;
using System;
using Apizr.Policing;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Apizr.Logging.Attributes;
using Microsoft.Extensions.Logging;
using Apizr.Caching.Attributes;
using Akavache;
using AutoMapper;
using MediatR;
using System.Reflection;

[assembly:Policy("TransientHttpError")] // Adjust policies if needed
[assembly:Log] // Adjust log levels if needed
[assembly:Cache] // Adjust cache mode and duration if needed
namespace Apizr.Sample.Tools.NSwag
{
    public static class ApizrRegistration
    {
        // Define your PolicyRegistry if not done yet somewhere else
        public static PolicyRegistry ApizrPolicyRegistry = new PolicyRegistry
        {
            {
                "TransientHttpError", HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, LoggedPolicies.OnLoggedRetry).WithPolicyKey("TransientHttpError")
            }
        };

        // Static
        public static IApizrManager<IPetstoreOpenApi> Build()
        {
            // MediatR is not compatible with Apizr static registration (ignored)

            var mapperConfig = new MapperConfiguration(config =>
            {
                // todo: replace with your own mapping profiles
                config.AddProfile<YOUR_FIRST_PROFILE_HERE>();
                config.AddProfile<YOUR_SECOND_PROFILE_HERE>();
                // and so on...
            });

            var apizrManager = ApizrBuilder.Current.CreateManagerFor<IPetstoreOpenApi>(
                options => options.WithBaseAddress("/api/v3")
                    .WithLoggerFactory(() => LoggerFactory.Create(logging =>
                    {
                        // Add here whatever logger of your choice, like:
                        //logging.AddConsole();
                        //logging.AddDebug();
                        //logging.SetMinimumLevel(LogLevel.Trace);
                    }))
                    .WithPolicyRegistry(ApizrPolicyRegistry)
                    .WithPriority()
                    .WithAkavacheCacheHandler()
                    .WithAutoMapperMappingHandler(mapperConfig)
            );

            return apizrManager;
        }

        // Extended
        public static IServiceCollection AddApizr(this IServiceCollection services)
        {
            // Don't forget to setup your logging layer somewhere to get logs from Apizr

            services.AddPolicyRegistry(ApizrPolicyRegistry); // Remove this line if already registered somewhere else

            services.AddMediatR(Assembly.GetExecutingAssembly()); // Remove this line if already registered somewhere else

            services.AddAutoMapper(Assembly.GetExecutingAssembly()); // Remove this line if already registered somewhere else

            services.AddApizrManagerFor<IPetstoreOpenApi>(
                options => options.WithBaseAddress("/api/v3")
                    .WithPriority()
                    .WithAkavacheCacheHandler()
                    .WithMediation()
                    .WithOptionalMediation()
                    .WithAutoMapperMappingHandler()
            );

            return services;
        }
    }
}