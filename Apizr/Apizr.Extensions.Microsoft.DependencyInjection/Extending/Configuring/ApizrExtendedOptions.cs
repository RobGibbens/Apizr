﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Apizr.Caching;
using Apizr.Configuring;
using Apizr.Connecting;
using Apizr.Extending.Configuring.Common;
using Apizr.Extending.Configuring.Proper;
using Apizr.Extending.Configuring.Registry;
using Apizr.Logging;
using Apizr.Mapping;
using Apizr.Requesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;

namespace Apizr.Extending.Configuring
{
    /// <inheritdoc cref="IApizrExtendedOptions"/>
    public class ApizrExtendedOptions : ApizrExtendedOptionsBase, IApizrExtendedOptions
    {
        /// <summary>
        /// The options constructor
        /// </summary>
        /// <param name="commonOptions">The common options</param>
        /// <param name="properOptions">The proper options</param>
        public ApizrExtendedOptions(IApizrExtendedCommonOptions commonOptions, IApizrExtendedProperOptions properOptions) : base(commonOptions, properOptions)
        {
            ApizrManagerType = properOptions.ApizrManagerType;
            BaseUriFactory = properOptions.BaseUriFactory;
            BaseAddressFactory = properOptions.BaseAddressFactory;
            BasePathFactory = properOptions.BasePathFactory;
            HttpTracerModeFactory = properOptions.HttpTracerModeFactory;
            TrafficVerbosityFactory = properOptions.TrafficVerbosityFactory;
            LogLevelsFactory = properOptions.LogLevelsFactory;
            LoggerFactory = properOptions.LoggerFactory;
            HttpClientHandlerFactory = properOptions.HttpClientHandlerFactory;
            RefitSettingsFactory = commonOptions.RefitSettingsFactory;
            ConnectivityHandlerType = commonOptions.ConnectivityHandlerType;
            ConnectivityHandlerFactory = commonOptions.ConnectivityHandlerFactory;
            CacheHandlerType = commonOptions.CacheHandlerType;
            CacheHandlerFactory = commonOptions.CacheHandlerFactory;
            MappingHandlerType = commonOptions.MappingHandlerType;
            MappingHandlerFactory = commonOptions.MappingHandlerFactory;
            DelegatingHandlersExtendedFactories = properOptions.DelegatingHandlersExtendedFactories;
            CrudEntities = commonOptions.CrudEntities;
            WebApis = commonOptions.WebApis;
            ObjectMappings = commonOptions.ObjectMappings;
            PostRegistries = commonOptions.PostRegistries;
            PostRegistrationActions = commonOptions.PostRegistrationActions;
        }

        /// <inheritdoc />
        public Type ApizrManagerType { get; }

        /// <inheritdoc />
        public Type ConnectivityHandlerType { get; set; }

        /// <inheritdoc />
        public Type CacheHandlerType { get; set; }

        /// <inheritdoc />
        public Type MappingHandlerType { get; set; }

        /// <inheritdoc />

        private Func<IServiceProvider, Uri> _baseUriFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, Uri> BaseUriFactory
        {
            get => _baseUriFactory;
            set => _baseUriFactory = value != null ? serviceProvider => BaseUri = value.Invoke(serviceProvider) : null;
        }

        private Func<IServiceProvider, string> _baseAddressFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, string> BaseAddressFactory
        {
            get => _baseAddressFactory;
            set => _baseAddressFactory = value != null ? serviceProvider => BaseAddress = value.Invoke(serviceProvider) : null;
        }

        private Func<IServiceProvider, string> _basePathFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, string> BasePathFactory
        {
            get => _basePathFactory;
            set => _basePathFactory = value != null ? serviceProvider => BasePath = value.Invoke(serviceProvider) : null;
        }

        private Func<IServiceProvider, HttpTracerMode> _httpTracerModeFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, HttpTracerMode> HttpTracerModeFactory
        {
            get => _httpTracerModeFactory;
            set => _httpTracerModeFactory = serviceProvider => HttpTracerMode = value.Invoke(serviceProvider);
        }

        private Func<IServiceProvider, HttpMessageParts> _trafficVerbosityFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, HttpMessageParts> TrafficVerbosityFactory
        {
            get => _trafficVerbosityFactory;
            set => _trafficVerbosityFactory = serviceProvider => TrafficVerbosity = value.Invoke(serviceProvider);
        }

        private Func<IServiceProvider, LogLevel[]> _logLevelsFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, LogLevel[]> LogLevelsFactory
        {
            get => _logLevelsFactory;
            set => _logLevelsFactory = serviceProvider => LogLevels = value.Invoke(serviceProvider);
        }

        private Func<IServiceProvider, string, ILogger> _loggerFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, string, ILogger> LoggerFactory
        {
            get => _loggerFactory;
            protected set => _loggerFactory = (serviceProvider, webApiFriendlyName) => Logger = value.Invoke(serviceProvider, webApiFriendlyName);
        }

        private Func<IServiceProvider, HttpClientHandler> _httpClientHandlerFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, HttpClientHandler> HttpClientHandlerFactory
        {
            get => _httpClientHandlerFactory;
            set => _httpClientHandlerFactory = serviceProvider => HttpClientHandler = value.Invoke(serviceProvider);
        }


        private Func<IServiceProvider, RefitSettings> _refitSettingsFactory;
        /// <inheritdoc />
        public Func<IServiceProvider, RefitSettings> RefitSettingsFactory
        {
            get => _refitSettingsFactory;
            set => _refitSettingsFactory = serviceProvider => RefitSettings = value.Invoke(serviceProvider);
        }

        /// <inheritdoc />
        public Func<IServiceProvider, IConnectivityHandler> ConnectivityHandlerFactory { get; set; }

        /// <inheritdoc />
        public Func<IServiceProvider, ICacheHandler> CacheHandlerFactory { get; set; }

        /// <inheritdoc />
        public Func<IServiceProvider, IMappingHandler> MappingHandlerFactory { get; set; }

        /// <inheritdoc />
        public Action<IHttpClientBuilder> HttpClientBuilder { get; set; }

        /// <inheritdoc />
        public IDictionary<Type, CrudEntityAttribute> CrudEntities { get; }

        /// <inheritdoc />
        public IDictionary<Type, WebApiAttribute> WebApis { get; }

        /// <inheritdoc />
        public IDictionary<Type, MappedWithAttribute> ObjectMappings { get; }

        /// <inheritdoc />
        public IDictionary<Type, IApizrExtendedConcurrentRegistryBase> PostRegistries { get; }

        /// <inheritdoc />
        public IList<Action<Type, IServiceCollection>> PostRegistrationActions { get; }
    }

    /// <inheritdoc cref="IApizrExtendedOptionsBase"/>
    public class ApizrExtendedOptions<TWebApi> : ApizrOptions<TWebApi>, IApizrExtendedOptionsBase
    {
        private readonly IApizrExtendedOptionsBase _apizrExtendedOptions;
        public ApizrExtendedOptions(IApizrExtendedOptionsBase apizrOptions) : base(apizrOptions)
        {
            _apizrExtendedOptions = apizrOptions;
        }

        /// <inheritdoc />
        public HttpClientHandler HttpClientHandler => _apizrExtendedOptions.HttpClientHandler;

        /// <inheritdoc />

        public IList<Func<IServiceProvider, IApizrOptionsBase, DelegatingHandler>>
            DelegatingHandlersExtendedFactories => _apizrExtendedOptions.DelegatingHandlersExtendedFactories;
    }
}
