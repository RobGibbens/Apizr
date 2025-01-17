﻿using System.Net.Http;
using Apizr.Policing;
using Apizr.Progressing;

namespace Apizr.Extending;

public static class HttpRequestMessageExtensions
{
    public static bool ContainsApizrProgress(this HttpRequestMessage request) =>
        request.TryGetOptions(out var requestOptions) &&
        requestOptions.HandlersParameters.ContainsKey(Constants.ApizrProgressKey);

    public static IApizrProgress GetApizrProgress(this HttpRequestMessage request) =>
        request.TryGetOptions(out var requestOptions) &&
        requestOptions.HandlersParameters.TryGetValue(Constants.ApizrProgressKey, out var progressProperty) &&
        progressProperty is IApizrProgress progressValue
            ? progressValue
            : null;

    public static bool TryGetApizrProgress(this HttpRequestMessage request, out IApizrProgress apizrProgress)
    {
        apizrProgress = request.GetApizrProgress();
        return apizrProgress != null;
    }
}