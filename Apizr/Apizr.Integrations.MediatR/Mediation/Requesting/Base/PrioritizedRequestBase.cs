﻿using System;
using Apizr.Configuring.Request;
using Polly;

namespace Apizr.Mediation.Requesting.Base
{
    /// <summary>
    /// The base prioritized mediation request getting some <typeparamref name="TResultData"/> data
    /// </summary>
    /// <typeparam name="TResultData">The returned data</typeparam>
    public abstract class PrioritizedRequestBase<TResultData, TApizrRequestOptions, TApizrRequestOptionsBuilder> : RequestBase<TResultData, TApizrRequestOptions, TApizrRequestOptionsBuilder> 
        where TApizrRequestOptions : IApizrRequestOptionsBase
        where TApizrRequestOptionsBuilder : IApizrRequestOptionsBuilderBase<TApizrRequestOptions, TApizrRequestOptionsBuilder>
    {
        /// <summary>
        /// The base prioritized mediation query constructor
        /// </summary>
        /// <param name="optionsBuilder">Options provided to the request</param>
        protected PrioritizedRequestBase(Action<TApizrRequestOptionsBuilder> optionsBuilder = null) : this(-1, optionsBuilder)
        {

        }

        /// <summary>
        /// The base prioritized mediation query constructor
        /// </summary>
        /// <param name="priority">The execution priority to apply</param>
        /// <param name="optionsBuilder">Options provided to the request</param>
        protected PrioritizedRequestBase(int priority, Action<TApizrRequestOptionsBuilder> optionsBuilder = null) : base(optionsBuilder)
        {
            Priority = priority;
        }

        /// <summary>
        /// The execution priority to apply
        /// </summary>
        public int Priority { get; }
    }
}
