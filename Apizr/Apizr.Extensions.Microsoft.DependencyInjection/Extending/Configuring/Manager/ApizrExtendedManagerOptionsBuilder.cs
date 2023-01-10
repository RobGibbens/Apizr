﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Apizr.Authenticating;
using Apizr.Caching;
using Apizr.Configuring;
using Apizr.Configuring.Manager;
using Apizr.Configuring.Shared;
using Apizr.Connecting;
using Apizr.Logging;
using Apizr.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;

namespace Apizr.Extending.Configuring.Manager
{
    /// <summary>
    /// Builder options available for extended registrations
    /// </summary>
    public class ApizrExtendedManagerOptionsBuilder : IApizrExtendedManagerOptionsBuilder, IApizrInternalRegistrationOptionsBuilder
    {
        protected readonly ApizrExtendedManagerOptions Options;

        public ApizrExtendedManagerOptionsBuilder(ApizrExtendedManagerOptions apizrOptions)
        {
            Options = apizrOptions;
        }

        /// <inheritdoc />
        IApizrExtendedManagerOptions IApizrExtendedManagerOptionsBuilder.ApizrOptions => Options;

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBaseAddress(string baseAddress)
            => WithBaseAddress(_ => baseAddress);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBaseAddress(Func<IServiceProvider, string> baseAddressFactory)
        {
            Options.BaseAddressFactory = baseAddressFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBaseAddress(Uri baseAddress)
            => WithBaseAddress(_ => baseAddress);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBaseAddress(Func<IServiceProvider, Uri> baseAddressFactory)
        {
            Options.BaseUriFactory = baseAddressFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBasePath(string basePath)
            => WithBasePath(_ => basePath);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithBasePath(Func<IServiceProvider, string> basePathFactory)
        {
            Options.BasePathFactory = basePathFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithHttpClientHandler(HttpClientHandler httpClientHandler)
            => WithHttpClientHandler(_ => httpClientHandler);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithHttpClientHandler(Func<IServiceProvider, HttpClientHandler> httpClientHandlerFactory)
        {
            Options.HttpClientHandlerFactory = httpClientHandlerFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder ConfigureHttpClientBuilder(Action<IHttpClientBuilder> httpClientBuilder)
        {
            Options.HttpClientBuilder = httpClientBuilder;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithAuthenticationHandler(
            Func<HttpRequestMessage, Task<string>> refreshTokenFactory)
            => AddDelegatingHandler((serviceProvider, options) =>
                new AuthenticationHandler(serviceProvider.GetService<ILogger>(), options, refreshTokenFactory));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithAuthenticationHandler<TAuthenticationHandler>(
            Func<IServiceProvider, IApizrManagerOptionsBase, TAuthenticationHandler> authenticationHandlerFactory)
            where TAuthenticationHandler : AuthenticationHandlerBase
            => AddDelegatingHandler(authenticationHandlerFactory);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithAuthenticationHandler<TSettingsService, TTokenService>(
            Expression<Func<TSettingsService, string>> tokenProperty,
            Expression<Func<TTokenService, HttpRequestMessage, Task<string>>> refreshTokenMethod)
            => AddDelegatingHandler((serviceProvider, options) =>
                new AuthenticationHandler<TSettingsService, TTokenService>(
                    serviceProvider.GetService<ILogger>(),
                    options,
                    serviceProvider.GetRequiredService<TSettingsService>, tokenProperty,
                    serviceProvider.GetRequiredService<TTokenService>, refreshTokenMethod));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithAuthenticationHandler<TSettingsService>(
            Expression<Func<TSettingsService, string>> tokenProperty,
            Func<HttpRequestMessage, Task<string>> refreshTokenFactory)
            => AddDelegatingHandler((serviceProvider, options) =>
                new AuthenticationHandler<TSettingsService>(
                    serviceProvider.GetService<ILogger>(),
                    options,
                    serviceProvider.GetRequiredService<TSettingsService>, tokenProperty, refreshTokenFactory));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder AddDelegatingHandler<THandler>(THandler delegatingHandler) where THandler : DelegatingHandler
            => AddDelegatingHandler((_, _) => delegatingHandler);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder AddDelegatingHandler<THandler>(
            Func<IServiceProvider, THandler> delegatingHandlerFactory) where THandler : DelegatingHandler
            => AddDelegatingHandler((serviceProvider, _) => delegatingHandlerFactory(serviceProvider));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder AddDelegatingHandler<THandler>(Func<IServiceProvider, IApizrManagerOptionsBase, THandler> delegatingHandlerFactory) where THandler : DelegatingHandler
        {
            Options.DelegatingHandlersExtendedFactories[typeof(THandler)] = delegatingHandlerFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithContext(Func<Context> contextFactory,
            ApizrDuplicateStrategy strategy = ApizrDuplicateStrategy.Merge)
        {
            switch (strategy)
            {
                case ApizrDuplicateStrategy.Ignore:
                    Options.ContextFactory ??= contextFactory;
                    break;
                case ApizrDuplicateStrategy.Replace:
                    Options.ContextFactory = contextFactory;
                    break;
                case ApizrDuplicateStrategy.Add:
                case ApizrDuplicateStrategy.Merge:
                    if (Options.ContextFactory == null)
                    {
                        Options.ContextFactory = contextFactory;
                    }
                    else
                    {
                        Options.ContextFactory = () => new Context(null,
                            Options.ContextFactory.Invoke().Concat(contextFactory.Invoke().ToList())
                                .ToDictionary(x => x.Key, x => x.Value));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithExCatching(Action<ApizrException> onException,
            bool letThrowOnExceptionWithEmptyCache = true, ApizrDuplicateStrategy strategy = ApizrDuplicateStrategy.Replace)
        {
            switch (strategy)
            {
                case ApizrDuplicateStrategy.Ignore:
                    Options.OnException ??= onException;
                    break;
                case ApizrDuplicateStrategy.Replace:
                    Options.OnException = onException;
                    break;
                case ApizrDuplicateStrategy.Add:
                case ApizrDuplicateStrategy.Merge:
                    if (Options.OnException == null)
                    {
                        Options.OnException = onException;
                    }
                    else
                    {
                        Options.OnException += onException.Invoke;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            Options.LetThrowOnExceptionWithEmptyCache = letThrowOnExceptionWithEmptyCache;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithExCatching<TResult>(Action<ApizrException<TResult>> onException,
            bool letThrowOnExceptionWithEmptyCache = true,
            ApizrDuplicateStrategy strategy = ApizrDuplicateStrategy.Replace)
            => WithExCatching(ex => onException.Invoke((ApizrException<TResult>)ex), letThrowOnExceptionWithEmptyCache,
                strategy);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithHandlerParameter(string key, object value)
        {
            Options.HandlersParameters[key] = value;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithLogging(HttpTracerMode httpTracerMode = HttpTracerMode.Everything,
            HttpMessageParts trafficVerbosity = HttpMessageParts.All,
            params LogLevel[] logLevels)
            => WithLogging(_ => httpTracerMode, _ => trafficVerbosity, _ => logLevels);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithLogging(
            Func<IServiceProvider, (HttpTracerMode, HttpMessageParts, LogLevel[])> loggingConfigurationFactory)
            => WithLogging(serviceProvider => loggingConfigurationFactory.Invoke(serviceProvider).Item1,
                serviceProvider => loggingConfigurationFactory.Invoke(serviceProvider).Item2,
                serviceProvider => loggingConfigurationFactory.Invoke(serviceProvider).Item3);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithLogging(Func<IServiceProvider, HttpTracerMode> httpTracerModeFactory,
            Func<IServiceProvider, HttpMessageParts> trafficVerbosityFactory,
            Func<IServiceProvider, LogLevel[]> logLevelFactory)
        {
            Options.HttpTracerModeFactory = httpTracerModeFactory;
            Options.TrafficVerbosityFactory = trafficVerbosityFactory;
            Options.LogLevelsFactory = logLevelFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithRefitSettings(RefitSettings refitSettings)
            => WithRefitSettings(_ => refitSettings);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler(IConnectivityHandler connectivityHandler)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithRefitSettings(
            Func<IServiceProvider, RefitSettings> refitSettingsFactory)
        {
            Options.RefitSettingsFactory = refitSettingsFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler(Func<IServiceProvider, IConnectivityHandler> connectivityHandlerFactory)
        {
            Options.ConnectivityHandlerFactory = connectivityHandlerFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler<TConnectivityHandler>(Expression<Func<TConnectivityHandler, bool>> factory)
        {
            Options.ConnectivityHandlerFactory = serviceProvider => new DefaultConnectivityHandler(() => factory.Compile()(serviceProvider.GetRequiredService<TConnectivityHandler>()));

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler(Func<bool> connectivityCheckingFunction)
        {
            Options.ConnectivityHandlerFactory = _ => new DefaultConnectivityHandler(connectivityCheckingFunction);

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler<TConnectivityHandler>()
            where TConnectivityHandler : class, IConnectivityHandler
            => WithConnectivityHandler(typeof(TConnectivityHandler));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithConnectivityHandler(Type connectivityHandlerType)
        {
            if (!typeof(IConnectivityHandler).IsAssignableFrom(connectivityHandlerType))
                throw new ArgumentException(
                    $"Your connectivity handler class must inherit from {nameof(IConnectivityHandler)} interface or derived");

            Options.ConnectivityHandlerType = connectivityHandlerType;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithCacheHandler(ICacheHandler cacheHandler)
            => WithCacheHandler(_ => cacheHandler);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithCacheHandler(Func<IServiceProvider, ICacheHandler> cacheHandlerFactory)
        {
            Options.CacheHandlerFactory = cacheHandlerFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithCacheHandler<TCacheHandler>()
            where TCacheHandler : class, ICacheHandler
            => WithCacheHandler(typeof(TCacheHandler));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithCacheHandler(Type cacheHandlerType)
        {
            if (!typeof(ICacheHandler).IsAssignableFrom(cacheHandlerType))
                throw new ArgumentException(
                    $"Your cache handler class must inherit from {nameof(ICacheHandler)} interface or derived");

            Options.CacheHandlerType = cacheHandlerType;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithMappingHandler(IMappingHandler mappingHandler)
            => WithMappingHandler(_ => mappingHandler);

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithMappingHandler(Func<IServiceProvider, IMappingHandler> mappingHandlerFactory)
        {
            Options.MappingHandlerFactory = mappingHandlerFactory;

            return this;
        }

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithMappingHandler<TMappingHandler>()
            where TMappingHandler : class, IMappingHandler
            => WithMappingHandler(typeof(TMappingHandler));

        /// <inheritdoc />
        public IApizrExtendedManagerOptionsBuilder WithMappingHandler(Type mappingHandlerType)
        {
            if (!typeof(IMappingHandler).IsAssignableFrom(mappingHandlerType))
                throw new ArgumentException(
                    $"Your mapping handler class must inherit from {nameof(IMappingHandler)} interface or derived");

            Options.MappingHandlerType = mappingHandlerType;

            return this;
        }

        #region Internal

        public void SetHandlerParameter(string key, object value) => WithHandlerParameter(key, value);

        public void SetPrimaryHttpMessageHandler(Func<DelegatingHandler, ILogger, IApizrManagerOptionsBase, HttpMessageHandler> primaryHandlerFactory)
        {
            Options.PrimaryHandlerFactory = primaryHandlerFactory;
        }

        /// <inheritdoc />
        public void AddDelegatingHandler<THandler>(Func<IApizrManagerOptionsBase, THandler> handlerFactory)
            where THandler : DelegatingHandler => AddDelegatingHandler((_, opt) => handlerFactory.Invoke(opt));

        #endregion
    }
}
