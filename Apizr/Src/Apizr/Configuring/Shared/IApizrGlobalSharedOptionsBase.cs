﻿using Apizr.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Polly;

namespace Apizr.Configuring.Shared
{
    /// <summary>
    /// Options available at all (common, proper and request) levels and for all (static and extended) registration types
    /// </summary>
    public interface IApizrGlobalSharedOptionsBase
    {
        /// <summary>
        /// Http traffic tracing mode
        /// </summary>
        HttpTracerMode HttpTracerMode { get; }

        /// <summary>
        /// Http traffic tracing verbosity
        /// </summary>
        HttpMessageParts TrafficVerbosity { get; }

        /// <summary>
        /// Log levels while writing
        /// </summary>
        LogLevel[] LogLevels { get; }

        /// <summary>
        /// Catching potential exception if defined
        /// </summary>
        Action<ApizrException> OnException { get; }

        /// <summary>
        /// Let throw potential exception if there's no cached data to return
        /// </summary>
        bool LetThrowOnExceptionWithEmptyCache { get; }

        /// <summary>
        /// Parameters passed through delegating handlers
        /// </summary>
        IDictionary<string, object> HandlersParameters { get; }

        /// <summary>
        /// Headers to add to the request
        /// </summary>
        IList<string> Headers { get; }
    }
}
