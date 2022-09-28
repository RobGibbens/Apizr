﻿using System;
using System.Threading;
using Polly;

namespace Apizr.Configuring.Request;

public class ApizrRequestOptionsBuilder : IApizrRequestOptionsBuilder
{
    protected readonly ApizrRequestOptions Options;

    public ApizrRequestOptionsBuilder(ApizrRequestOptions options)
    {
        Options = options;
    }

    /// <inheritdoc />
    public IApizrRequestOptions ApizrOptions => Options;

    /// <inheritdoc />
    public IApizrRequestOptionsBuilder WithContext(Context context)
    {
        Options.Context = context;

        return this;
    }

    /// <inheritdoc />
    public IApizrRequestOptionsBuilder WithCancellationToken(CancellationToken cancellationToken)
    {
        Options.CancellationToken = cancellationToken;

        return this;
    }

    /// <param name="clearCache"></param>
    /// <inheritdoc />
    public IApizrRequestOptionsBuilder WithCacheCleared(bool clearCache)
    {
        Options.ClearCache = clearCache;

        return this;
    }

    /// <inheritdoc />
    public IApizrRequestOptionsBuilder WithExceptionCatcher(Action<Exception> onException)
    {
        Options.OnException = onException;

        return this;
    }
}