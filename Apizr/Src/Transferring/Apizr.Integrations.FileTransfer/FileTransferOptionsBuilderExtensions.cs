﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Apizr.Configuring.Common;
using Apizr.Configuring.Manager;
using Apizr.Configuring.Proper;
using Apizr.Configuring.Registry;
using Apizr.Configuring.Request;
using Apizr.Configuring.Shared;
using Apizr.Logging;
using Apizr.Progressing;
using Apizr.Transferring.Managing;
using Apizr.Transferring.Requesting;
using Refit;

namespace Apizr;

/// <summary>
/// File transfer builder extensions
/// </summary>
public static class FileTransferOptionsBuilderExtensions
{
    #region Transfer

    #region Single

    /// <summary>
    /// Create an upload manager for IUploadApi (you must at least provide a base url thanks to the options builder)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrUploadManager CreateUploadManager(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder) =>
        new ApizrUploadManager(
            builder.CreateManagerFor<IUploadApi>(optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody)));

    /// <summary>
    /// Create an upload manager for the provided upload api derived from IUploadApi
    /// </summary>
    /// <typeparam name="TUploadApi">The upload api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrUploadManager<TUploadApi> CreateUploadManagerFor<TUploadApi>(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null)
        where TUploadApi : IUploadApi =>
        new ApizrUploadManager<TUploadApi>(
            builder.CreateManagerFor<TUploadApi>(optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody)));

    /// <summary>
    /// Create a download manager for IDownloadApi (you must at least provide a base url thanks to the options builder)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrDownloadManager CreateDownloadManager(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null) =>
        new ApizrDownloadManager(
            builder.CreateManagerFor<IDownloadApi>(optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody)));

    /// <summary>
    /// Create a download manager for the provided download api derived from IDownloadApi
    /// </summary>
    /// <typeparam name="TDownloadApi">The download api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrDownloadManager<TDownloadApi> CreateDownloadManagerFor<TDownloadApi>(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null)
        where TDownloadApi : IDownloadApi =>
        new ApizrDownloadManager<TDownloadApi>(
            builder.CreateManagerFor<TDownloadApi>(optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody)));

    /// <summary>
    /// Create a download manager for the provided download api derived from IDownloadApi{TDownloadParams}
    /// </summary>
    /// <typeparam name="TDownloadApi">The download api interface to manage</typeparam>
    /// <typeparam name="TDownloadParams">The download query parameters type</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrDownloadManager<TDownloadApi, TDownloadParams> CreateDownloadManagerFor<TDownloadApi,
        TDownloadParams>(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null)
        where TDownloadApi : IDownloadApi<TDownloadParams> =>
        new ApizrDownloadManager<TDownloadApi, TDownloadParams>(
            builder.CreateManagerFor<TDownloadApi>(optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody)));

    /// <summary>
    /// Create a transfer manager for ITransferApi (you must at least provide a base url thanks to the options builder)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrTransferManager CreateTransferManager(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null) =>
        new ApizrTransferManager(
            builder.CreateDownloadManagerFor<ITransferApi>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.ResponseBody)),
            builder.CreateUploadManagerFor<ITransferApi>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.RequestBody)));

    /// <summary>
    /// Create a transfer manager for the provided transfer api derived from ITransferApi
    /// </summary>
    /// <typeparam name="TTransferApi">The transfer api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrTransferManager<TTransferApi> CreateTransferManagerFor<TTransferApi>(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null)
        where TTransferApi : ITransferApi =>
        new ApizrTransferManager<TTransferApi>(
            builder.CreateDownloadManagerFor<TTransferApi>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.ResponseBody)),
            builder.CreateUploadManagerFor<TTransferApi>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.RequestBody)));

    /// <summary>
    /// Create a transfer manager for the provided transfer api derived from ITransferApi{TDownloadParams}
    /// </summary>
    /// <typeparam name="TTransferApi">The transfer api interface to manage</typeparam>
    /// <typeparam name="TDownloadParams">The download query parameters type</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrTransferManager<TTransferApi, TDownloadParams> CreateTransferManagerFor<TTransferApi,
        TDownloadParams>(this IApizrBuilder builder,
        Action<IApizrManagerOptionsBuilder> optionsBuilder = null)
        where TTransferApi : ITransferApi<TDownloadParams> =>
        new ApizrTransferManager<TTransferApi, TDownloadParams>(
            builder.CreateDownloadManagerFor<TTransferApi, TDownloadParams>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.ResponseBody)),
            builder.CreateUploadManagerFor<TTransferApi>(IgnoreMessageParts(optionsBuilder,
                HttpMessageParts.RequestBody)));

    #endregion

    #region Multiple

    /// <summary>
    /// Add an upload manager for IUploadApi (you must at least provide a base url)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder AddUploadManager(this IApizrRegistryBuilder builder,
        Action<IApizrProperOptionsBuilder> optionsBuilder)
        => builder.AddUploadManagerFor<IUploadApi>(optionsBuilder);

    /// <summary>
    /// Add an upload manager for the provided upload api derived from IUploadApi
    /// </summary>
    /// <typeparam name="TUploadApi">The upload api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder AddUploadManagerFor<TUploadApi>(this IApizrRegistryBuilder builder, Action<IApizrProperOptionsBuilder> optionsBuilder = null)
        where TUploadApi : IUploadApi
    {
        if (builder is IApizrInternalRegistryBuilder<IApizrProperOptionsBuilder> internalBuilder)
        {
            if (typeof(TUploadApi) == typeof(IUploadApi))
            {
                internalBuilder.AddWrappingManagerFor<IUploadApi, IApizrUploadManager>(
                    apizrManager => new ApizrUploadManager(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody));

                internalBuilder.AddAliasingManagerFor<IApizrUploadManager<IUploadApi>, IApizrUploadManager>();
            }
            else
                internalBuilder.AddWrappingManagerFor<TUploadApi, IApizrUploadManager<TUploadApi>>(
                    apizrManager => new ApizrUploadManager<TUploadApi>(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody));
        }

        return builder;
    }

    /// <summary>
    /// Add a download manager for IDownloadApi (you must at least provide a base url)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder
        AddDownloadManager(
            this IApizrRegistryBuilder builder,
            Action<IApizrProperOptionsBuilder> optionsBuilder)
        => builder.AddDownloadManagerFor<IDownloadApi>(optionsBuilder);

    /// <summary>
    /// Add a download manager for the provided download api derived from IDownloadApi
    /// </summary>
    /// <typeparam name="TDownloadApi">The download api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder
        AddDownloadManagerFor<TDownloadApi>(
            this IApizrRegistryBuilder builder,
            Action<IApizrProperOptionsBuilder> optionsBuilder = null)
        where TDownloadApi : IDownloadApi
    {
        if (builder is IApizrInternalRegistryBuilder<IApizrProperOptionsBuilder> internalBuilder)
        {
            if (typeof(TDownloadApi) == typeof(IDownloadApi))
            {
                internalBuilder.AddWrappingManagerFor<IDownloadApi, IApizrDownloadManager>(
                    apizrManager => new ApizrDownloadManager(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));

                internalBuilder.AddAliasingManagerFor<IApizrDownloadManager<IDownloadApi>, IApizrDownloadManager>();
            }
            else
                internalBuilder.AddWrappingManagerFor<TDownloadApi, IApizrDownloadManager<TDownloadApi>>(
                    apizrManager => new ApizrDownloadManager<TDownloadApi>(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));
        }

        return builder;
    }

    /// <summary>
    /// Add a download manager for the provided download api derived from IDownloadApi{TDownloadParams}
    /// </summary>
    /// <typeparam name="TDownloadApi">The download api interface to manage</typeparam>
    /// <typeparam name="TDownloadParams">The download query parameters type</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder
        AddDownloadManagerFor<TDownloadApi, TDownloadParams>(
            this IApizrRegistryBuilder builder,
            Action<IApizrProperOptionsBuilder> optionsBuilder = null)
        where TDownloadApi : IDownloadApi<TDownloadParams>
    {
        if (builder is IApizrInternalRegistryBuilder<IApizrProperOptionsBuilder> internalBuilder)
        {
            internalBuilder?.AddWrappingManagerFor<TDownloadApi, IApizrDownloadManager<TDownloadApi, TDownloadParams>>(
                apizrManager => new ApizrDownloadManager<TDownloadApi, TDownloadParams>(apizrManager),
                optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));
        }

        return builder;
    }

    /// <summary>
    /// Add a transfer manager for ITransferApi (you must at least provide a base url)
    /// </summary>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder
        AddTransferManager(
            this IApizrRegistryBuilder builder,
            Action<IApizrProperOptionsBuilder> optionsBuilder) =>
        builder.AddTransferManagerFor<ITransferApi>(optionsBuilder);

    /// <summary>
    /// Add a transfer manager for the provided transfer api derived from ITransferApi
    /// </summary>
    /// <typeparam name="TTransferApi">The transfer api interface to manage</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder
        AddTransferManagerFor<TTransferApi>(
            this IApizrRegistryBuilder builder,
            Action<IApizrProperOptionsBuilder> optionsBuilder = null) 
        where TTransferApi : ITransferApi
    {
        if (builder is IApizrInternalRegistryBuilder<IApizrProperOptionsBuilder> internalBuilder)
        {
            if (typeof(TTransferApi) == typeof(ITransferApi))
            {
                // Upload
                internalBuilder.AddWrappingManagerFor<IUploadApi, IApizrUploadManager>(
                    apizrManager => new ApizrUploadManager(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody));

                internalBuilder.AddAliasingManagerFor<IApizrUploadManager<IUploadApi>, IApizrUploadManager>();

                // Download
                internalBuilder.AddWrappingManagerFor<IDownloadApi, IApizrDownloadManager>(
                    apizrManager => new ApizrDownloadManager(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));

                internalBuilder.AddAliasingManagerFor<IApizrDownloadManager<IDownloadApi>, IApizrDownloadManager>();

                // Transfer
                internalBuilder.AddWrappingManagerFor<ITransferApi, IApizrTransferManager>(
                    apizrManager => new ApizrTransferManager(
                        new ApizrDownloadManager<ITransferApi>(apizrManager),
                        new ApizrUploadManager<ITransferApi>(apizrManager)),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody | HttpMessageParts.ResponseBody));

                internalBuilder.AddAliasingManagerFor<IApizrTransferManager<ITransferApi>, IApizrTransferManager>();
            }
            else
            {
                // Upload
                internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrUploadManager<TTransferApi>>(
                    apizrManager => new ApizrUploadManager<TTransferApi>(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody));

                // Download
                internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrDownloadManager<TTransferApi>>(
                    apizrManager => new ApizrDownloadManager<TTransferApi>(apizrManager),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));

                // Transfer
                internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrTransferManager<TTransferApi>>(
                    apizrManager => new ApizrTransferManager<TTransferApi>(
                        new ApizrDownloadManager<TTransferApi>(apizrManager),
                        new ApizrUploadManager<TTransferApi>(apizrManager)),
                    optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody | HttpMessageParts.ResponseBody));
            }
        }

        return builder;
    }

    /// <summary>
    /// Add a transfer manager for the provided transfer api derived from ITransferApi{TDownloadParams}
    /// </summary>
    /// <typeparam name="TTransferApi">The transfer api interface to manage</typeparam>
    /// <typeparam name="TDownloadParams">The download query parameters type</typeparam>
    /// <param name="builder">The builder to create the manager from</param>
    /// <param name="optionsBuilder">The builder defining some options</param>
    /// <returns></returns>
    public static IApizrRegistryBuilder AddTransferManagerFor<TTransferApi, TDownloadParams>(
        this IApizrRegistryBuilder builder,
        Action<IApizrProperOptionsBuilder> optionsBuilder = null)
        where TTransferApi : ITransferApi<TDownloadParams>
    {
        if (builder is IApizrInternalRegistryBuilder<IApizrProperOptionsBuilder> internalBuilder)
        {
            // Upload
            internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrUploadManager<TTransferApi>>(
                apizrManager => new ApizrUploadManager<TTransferApi>(apizrManager),
                optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody));

            // Download
            internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrDownloadManager<TTransferApi, TDownloadParams>>(
                apizrManager => new ApizrDownloadManager<TTransferApi, TDownloadParams>(apizrManager),
                optionsBuilder.IgnoreMessageParts(HttpMessageParts.ResponseBody));

            // Transfer
            internalBuilder.AddWrappingManagerFor<TTransferApi, IApizrTransferManager<TTransferApi, TDownloadParams>>(
                apizrManager => new ApizrTransferManager<TTransferApi, TDownloadParams>(
                    new ApizrDownloadManager<TTransferApi, TDownloadParams>(apizrManager),
                    new ApizrUploadManager<TTransferApi>(apizrManager)),
                optionsBuilder.IgnoreMessageParts(HttpMessageParts.RequestBody | HttpMessageParts.ResponseBody));
        }

        return builder;
    }

    #endregion

    #endregion

    #region Progress

    /// <summary>
    /// Enables transfer progress reporting with Apizr
    /// (you should provide a progress callback or reporter at request time)
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TBuilder WithProgress<TBuilder>(this TBuilder builder)
        where TBuilder : IApizrGlobalSharedOptionsBuilderBase
    {
        if (builder is IApizrInternalRegistrationOptionsBuilder registrationBuilder)
            registrationBuilder.AddDelegatingHandler(_ =>
                new ApizrProgressHandler());

        return builder;
    }

    /// <summary>
    /// Tells Apizr to report any transfer progress
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="onProgress">The action called back on any progress</param>
    /// <returns></returns>
    public static TBuilder WithProgress<TBuilder>(this TBuilder builder, Action<ApizrProgressEventArgs> onProgress)
        where TBuilder : IApizrGlobalSharedOptionsBuilderBase
        => builder.WithProgress(new ApizrProgress(onProgress));

    /// <summary>
    /// Tells Apizr to report any transfer progress
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="progress">The progress reporter</param>
    /// <returns></returns>
    public static TBuilder WithProgress<TBuilder>(this TBuilder builder, IApizrProgress progress)
        where TBuilder : IApizrGlobalSharedOptionsBuilderBase
    {
        if (builder is IApizrInternalRegistrationOptionsBuilder registrationBuilder)
            registrationBuilder.AddDelegatingHandler(_ =>
                new ApizrProgressHandler());

        if (builder is IApizrInternalOptionsBuilder voidBuilder)
            voidBuilder.SetHandlerParameter(Constants.ApizrProgressKey, progress);

        return builder;
    }

    #endregion

    #region Path

    /// <summary>
    /// Tells Apizr to set the ending of the request uri with the provided path
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="dynamicPath">The path ending the request uri</param>
    /// <returns></returns>
    public static T WithDynamicPath<T>(this T builder, string dynamicPath)
        where T : IApizrRequestOptionsBuilderBase
    {
        if (builder is IApizrInternalOptionsBuilder voidBuilder)
            voidBuilder.SetHandlerParameter(Constants.ApizrDynamicPathKey, dynamicPath);

        return builder;
    }

    #endregion

    #region Internal

    internal static Action<T> IgnoreMessageParts<T>(this 
        Action<T> optionsBuilder, HttpMessageParts messageParts)
    where T : IApizrGlobalSharedOptionsBuilderBase
    {
        if (optionsBuilder == null)
            optionsBuilder = optionsBuilderInstance =>
                ((IApizrInternalOptionsBuilder)optionsBuilderInstance).SetHandlerParameter(Constants.ApizrIgnoreMessagePartsKey, messageParts);
        else
            optionsBuilder += optionsBuilderInstance =>
                ((IApizrInternalOptionsBuilder)optionsBuilderInstance).SetHandlerParameter(Constants.ApizrIgnoreMessagePartsKey, messageParts);

        return optionsBuilder;
    }

    #endregion
}