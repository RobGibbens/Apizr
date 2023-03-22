﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copied from https://github.com/aspnet/AspNetWebStack/tree/main/src/System.Net.Http.Formatting but without any external ref and adjusted to Apizr needs

using System.ComponentModel;
using System.Net.Http;

namespace Apizr.Progressing;

/// <summary>
/// Provides data for the events generated by <see cref="ApizrProgressHandler"/>.
/// </summary>
public class ApizrProgressEventArgs : ProgressChangedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApizrProgressEventArgs"/> with the parameters given.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="progressType">Type of progress (request or response)</param>
    /// <param name="progressPercentage">The percent completed of the overall exchange.</param>
    /// <param name="userToken">Any user state provided as part of reading or writing the data.</param>
    /// <param name="bytesTransferred">The current number of bytes either received or sent.</param>
    /// <param name="totalBytes">The total number of bytes expected to be received or sent.</param>
    public ApizrProgressEventArgs(HttpRequestMessage request, ApizrProgressType progressType, int progressPercentage, object userToken, long bytesTransferred, long? totalBytes)
        : base(progressPercentage, userToken)
    {
        Request = request;
        ProgressType = progressType;
        BytesTransferred = bytesTransferred;
        TotalBytes = totalBytes; 
    }

    /// <summary>
    /// Gets the request
    /// </summary>
    public HttpRequestMessage Request { get; }

    /// <summary>
    /// Gets the type of progress
    /// </summary>
    public ApizrProgressType ProgressType { get; }

    /// <summary>
    /// Gets the current number of bytes transferred.
    /// </summary>
    public long BytesTransferred { get; }

    /// <summary>
    /// Gets the total number of expected bytes to be sent or received. If the number is not known then this is null.
    /// </summary>
    public long? TotalBytes { get; }
}