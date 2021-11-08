﻿using System;
using System.Collections.Concurrent;
using Apizr.Optional.Requesting.Sending;
using Microsoft.Extensions.DependencyInjection;

namespace Apizr.Optional.Configuring.Registry
{
    public class ApizrOptionalMediationRegistry : ApizrOptionalMediationRegistryBase, IApizrOptionalMediationConcurrentRegistry
    {
        private IServiceProvider _serviceProvider;

        private ConcurrentDictionary<Type, Func<IApizrOptionalMediator>> ThrowIfNotConcurrentImplementation()
        {
            if (ConcurrentRegistry is ConcurrentDictionary<Type, Func<IApizrOptionalMediator>> concurrentRegistry)
            {
                return concurrentRegistry;
            }

            throw new InvalidOperationException($"This {nameof(ApizrOptionalMediationRegistry)} is not configured for concurrent operations.");
        }

        internal IApizrOptionalMediationRegistry GetInstance(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            return this;
        }

        public void AddOrUpdateFor(Type webApiType, Type mediatorType)
        {
            var registry = ThrowIfNotConcurrentImplementation();
            Func<IApizrOptionalMediator> mediatorFactory = () => _serviceProvider.GetRequiredService(mediatorType) as IApizrOptionalMediator;
            registry.AddOrUpdate(webApiType, k => mediatorFactory, (k, e) => mediatorFactory);
        }
    }
}